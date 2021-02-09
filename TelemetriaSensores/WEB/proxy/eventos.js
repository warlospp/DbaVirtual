function get() {
    $.ajax({
        type: "GET",
        url: "http://APPNODE01:8081/Telemetria.svc/consultarAlertaDetalle/1.M",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            del();
            set(result);
        },
        error: function (e) {
            console.error(e.responseText);
        }
    });

    function set(result) {
        var myWidth = 0, myHeight = 0;
        if (typeof (window.innerWidth) == 'number') {
            //No-IE 
            myWidth = window.innerWidth;
            myHeight = window.innerHeight;
        } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
            //IE 6+ 
            myWidth = document.documentElement.clientWidth;
            myHeight = document.documentElement.clientHeight;
        } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
            //IE 4 compatible 
            myWidth = document.body.clientWidth;
            myHeight = document.body.clientHeight;
        }
        var arr = [];
        if (result !== undefined && result != null) {
            for (var i = 0; i < result.length; i++) {
                var douMetrica = result[i].douMetrica;
                var strColor = result[i].strColor;
                var strFecha = result[i].strFecha;
                var strDispositivo = result[i].strDispositivo;
                var strIP = result[i].strIP;
                var strSensor = result[i].strSensor;
                var strTipo = result[i].strTipo;
                var strUmbral = result[i].strUmbral;
                var strUnidadMedida = result[i].strUnidadMedida;
                arr[i] = [strDispositivo, strIP, strSensor, strTipo, douMetrica + ' ' + strUnidadMedida, strFecha];
            }
        }
        $('#tabla').DataTable({
            data: arr,
            columns: [
                { title: "Dispositivo" },
                { title: "IP" },
                { title: "Sensor" },
                { title: "Tipo" },
                { title: "Métrica" },
                { title: "Fecha" }
            ],
            "bDestroy": true,
            "order": [[5, "desc"]],
            responsive: true,
            fixedHeader: {
                header: true,
                footer: true
            }
        });
    }

    function del() {
        var tableHeaderRowCount = 1;
        var table = document.getElementById('tabla');
        var rowCount = table.rows.length;
        for (var i = tableHeaderRowCount; i < rowCount; i++) {
            table.deleteRow(tableHeaderRowCount);
        }
    }
}


var ok = 0, error = 0;
function getTotalTrxAtms() {
    $.ajax({
        type: "GET",
        url: "http://APPNODE01:8081/Telemetria.svc/consultarAlertaDetalle/1.M",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result !== undefined && result != null) {
                for (var i = 0; i < result.length; i++) {
                    if (result[i].strTipo == "Temperatura") {
                        if (result[i].strSensor == "Sensor01") {
                            ok = result[i].douMetrica;
                        }
                        if (result[i].strSensor == "Sensor03") {
                            error = result[i].douMetrica;
                        }
                    }
                }
            }
        },
        error: function (e) {
            ok = 0;
            error = 0;
            console.error(e.responseText);
        }
    });
}


var arrOk = [], arrError = [];
var dataset;
var totalPoints = 100;
var updateInterval = 10000;
var h = new Date();

var now = h.setTime(h.getTime() - 1050000);

var options = {
    series: {
        lines: {
            fillColor: { colors: [{ opacity: 0 }, { opacity: 1 }] },
            lineWidth: 3
        }
    },
    xaxis: {
        mode: "time",
        tickSize: [1, "minute"],
        tickFormatter: function (v, axis) {
            var date = new Date(v);
            if (date.getSeconds() % 10 == 0) {
                var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                return hours + ":" + minutes; 
            } else {
                return "";
            }
        },
    },
    yaxes: [
        {
            position: "left",
            axisLabelUseCanvas: true,
            axisLabelFontSizePixels: 10,
            axisLabelFontFamily: 'Verdana, Arial',
            axisLabelPadding: 6
        }
    ],
    legend: {
        noColumns: 0,
        position: "nw"
    },
    grid: {
        backgroundColor: "#F5F5F5",
        tickColor: "#99A3A4"
    }
};

function initData() {
    for (var i = 0; i < totalPoints; i++) {
        var temp = [now += updateInterval, 0];
        arrOk.push(temp);
        arrError.push(temp);
    }
}

var temp;

function update() {
    getTotalTrxAtms();
    arrOk.shift();
    arrError.shift();

    now += updateInterval
    temp = [now, ok];
    arrOk.push(temp);
    temp = [now, error];
    arrError.push(temp);
    dataset = [
        { label: "Ok", data: arrOk, lines: { fill: false, lineWidth: 3  }, color: "#00FF00" },
        { label: "Error", data: arrError, lines: { fill: false, lineWidth: 3  }, color: "#ff0000" }
    ];
    $.plot($("#flot-Temperatura"), dataset, options);
    setTimeout(update, updateInterval);
}

function cargar_trx() {
    initData();
    dataset = [
        { label: "Ok", data: arrOk, lines: { fill: false, lineWidth: 3 }, color: "#00FF00" },
        { label: "Error", data: arrError, lines: { fill: false, lineWidth: 3 }, color: "#ff0000" }
    ];
    $.plot($("#flot-Temperatura"), dataset, options);
    setTimeout(update, updateInterval);
}