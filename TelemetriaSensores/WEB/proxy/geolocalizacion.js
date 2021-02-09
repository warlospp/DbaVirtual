var map;
var markers = [];

function initMap() {
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
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 20,
        center: new google.maps.LatLng(-0.088202, -78.476591)
    });
    get();
}

function get() {
    $.ajax({
        type: "GET",
        url: "http://APPNODE01:8081/Telemetria.svc/consultarAlertaxSensor/1.M",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            set(result);
        },
        error: function (e) {
            console.error(e.responseText);
        }
    });

    function set(result) {        
        del();
        if (result !== undefined && result != null) {
            for (var i = 0; i < result.length; i++) {
                var douLatLng = new google.maps.LatLng(result[i].douLatitud, result[i].douLongitud);
                var douMetrica = result[i].douMetrica;
                var strColor = result[i].strColor;
                var strDescripcion = result[i].strDescripcion;
                var strDireccion = result[i].strDireccion;
                var strDispositivo = result[i].strDispositivo;
                var strIP = result[i].strIP;
                var strSensor = result[i].strSensor;
                var strTipo = result[i].strTipo;
                var strUmbral = result[i].strUmbral;
                var strUnidadMedida = result[i].strUnidadMedida;
                add(douLatLng, douMetrica, strColor, strDescripcion, strDireccion, strDispositivo, strIP, strSensor, strTipo, strUmbral, strUnidadMedida);
            }
        }
    }

    function add(douLatLng, douMetrica, strColor, strDescripcion, strDireccion, strDispositivo, strIP, strSensor, strTipo, strUmbral, strUnidadMedida) {
        var infowindow = new google.maps.InfoWindow({
            content: '<div id="content">' +
                '<div id="siteNotice">' +
                '</div>' +
                '<h2 id="firstHeading" class="firstHeading">@' + strDispositivo + '</h2>' +
                '<h4 id="firstHeading" class="firstHeading">' + strSensor + '</h4>' +
                '<div id="bodyContent">' +
                '<p><b>IP: </b>' + strIP + '</p>' +
                '<p><b>Tipo: </b>' + strTipo + '</p>' +
                '<p><b>Descripción: </b>' + strDescripcion + '</p>' +
                '<p><b>Dirección: </b>' + strDireccion + '</p>' +
                '<p><b>Métrica: </b>' + douMetrica + ' ' + strUnidadMedida + '</p>' +
                '</div>' +
                '</div>',
            maxWidth: 480
        });
        window.setTimeout(function () {
            var marker = new google.maps.Marker({
                icon: {
                    path: strUmbral === "SOB" ? google.maps.SymbolPath.FORWARD_CLOSED_ARROW : strUmbral === "BAJ" ? google.maps.SymbolPath.BACKWARD_CLOSED_ARROW : google.maps.SymbolPath.CIRCLE,
                    strokeColor: "#566573",
                    scale: strUmbral === "SOB" ? 7 : strUmbral === "BAJ" ? 7 : 10,
                    fillOpacity: 1,
                    strokeOpacity: 1,
                    fillColor: strColor,
                    strokeWeight: 3
                },
                position: douLatLng,
                map: map,
                animation: strUmbral === "SOB" ? google.maps.Animation.BOUNCE : strUmbral === "BAJ" ? google.maps.Animation.BOUNCE : null,
                title: strSensor + "(" + strTipo +")"
            });

            marker.addListener('click', function () {
                infowindow.open(map, marker);
                map.setZoom(21);
                map.setCenter(marker.getPosition());
            });
            markers.push(marker);
        }, 0);
    }

    function del() {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers = [];
    }
}


