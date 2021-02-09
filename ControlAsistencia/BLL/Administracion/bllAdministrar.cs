using BLL.Marcaciones;
using BLL.Turnos;
using DTO.Empleados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Administracion
{
    public class bllAdministrar : IDisposable
    {
        //public DataTable dtPermiso = new DataTable(nameof(dtPermiso));
        //public DataTable dtFeriado = new DataTable(nameof(dtFeriado));
        //public DataTable dtMarcacionManual = new DataTable(nameof(dtMarcacionManual));

        private string strConn { get; set; }

        public bllAdministrar(string _strConn)
        {
            this.strConn = _strConn;
            this.estructurasTablas();
        }

        private void estructurasTablas()
        {
            //this.dtPermiso.Columns.Add("EMPLEADO", typeof(string));
            //this.dtPermiso.Columns.Add("FECHA_INICO", typeof(DateTime));
            //this.dtPermiso.Columns.Add("FECHA_FIN", typeof(DateTime));
            //this.dtPermiso.Columns.Add("PERMISO", typeof(string));
            //this.dtPermiso.Columns.Add("MOTIVO", typeof(string));
            //this.dtFeriado.Columns.Add("FERIADO", typeof(string));
            //this.dtFeriado.Columns.Add("FECHA_INICIO", typeof(DateTime));
            //this.dtFeriado.Columns.Add("DURACION", typeof(int));
            //this.dtMarcacionManual.Columns.Add("EMPLEADO", typeof(string));
            //this.dtMarcacionManual.Columns.Add("FECHA_HORA", typeof(DateTime));
            //this.dtMarcacionManual.Columns.Add("TIPO", typeof(string));
        }

        //public bool insertarMarcacionManual(
        //  int USERID,
        //  DateTime CHECKTIME,
        //  string CHECKTYPE,
        //  string[] _strDepartamento)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        using (bllMarcacion bll = new bllMarcacion(this.strConn))
        //        {
        //            bll.execQueryMarcacion("INSERT INTO CHECKINOUT (USERID,CHECKTIME,CHECKTYPE,VERIFYCODE,SENSORID,WorkCode,sn,UserExtFmt) VALUES (@USERID,@CHECKTIME,@CHECKTYPE,@VERIFYCODE,@SENSORID,@WorkCode,@sn,@UserExtFmt);", new Dictionary<string, string>()
        //            {
        //                { "@USERID", USERID.ToString()  },
        //                { "@CHECKTIME", CHECKTIME.ToString("yyyy-MM-dd HH:mm:ss") },
        //                { "@CHECKTYPE", CHECKTYPE.ToString() },
        //                { "@VERIFYCODE", "1" },
        //                { "@SENSORID", "1" },
        //                { "@WorkCode", "0" },
        //                { "@sn", string.Empty },
        //                { "@UserExtFmt", "0" }
        //            });
        //            flag = true;
        //        }
        //    }
        //    catch (Exception )
        //    {
        //        throw;
        //    }
        //    if (flag)
        //    {
        //        try
        //        {
        //            using (bllExtraer bll = new bllExtraer(this.strConn, _strDepartamento))
        //            {
        //                var list = bll.extraerEmpleado()
        //                        .Where(e => e.USERID == USERID)
        //                        .Select(e => new
        //                        {
        //                            EMPLEADO = e.USERID.ToString() + " - " + e.Name
        //                        }).ToList();
        //                string _strEmpleado = string.Empty;
        //                foreach (var data in list)
        //                    _strEmpleado = data.EMPLEADO;
        //                string _strTipo = CHECKTYPE.ToString() == "O" ? "SALIDA" : "ENTRADA";
        //                //this.reporteMarcacionManual(_strEmpleado, CHECKTIME, _strTipo);
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //    return flag;
        //}

        //public bool insertarFeriado(string HOLIDAYNAME, DateTime STARTTIME, int DURATION)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        using (bllFeriado bll = new bllFeriado(this.strConn))
        //        {
        //            bll.execQueryFeriado("INSERT INTO HOLIDAYS (HOLIDAYNAME,STARTTIME,DURATION,DeptID) VALUES (@HOLIDAYNAME,@STARTTIME,@DURATION,@DeptID); ", new Dictionary<string, string>()
        //            {
        //                {"@HOLIDAYNAME",HOLIDAYNAME},
        //                {"@STARTTIME",STARTTIME.ToString("yyyy-MM-dd")},
        //                {"@DURATION",DURATION.ToString()},
        //                {"@DeptID","0"}
        //            });
        //            flag = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message == "No se puede encontrar la tabla 0.")
        //        {
        //            flag = true;
        //        }
        //    }
        //    //if (flag)
        //        //this.reporteFeriado(HOLIDAYNAME, STARTTIME, DURATION);
        //    return flag;
        //}

        //public bool insertarPermiso(int USERID,DateTime STARTSPECDAY,DateTime ENDSPECDAY,int DATEID,string YUANYING,string[] _strDepartamento)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        using (bllFeriado bll = new bllFeriado(this.strConn))
        //        {
        //            bll.execQueryFeriado("INSERT INTO USER_SPEDAY (USERID,STARTSPECDAY,ENDSPECDAY,DATEID,YUANYING,[DATE]) VALUES (@USERID,@STARTSPECDAY,@ENDSPECDAY,@DATEID,@YUANYING,@DATE);", new Dictionary<string, string>()
        //            {
        //                {"@USERID",USERID.ToString().ToUpper()},
        //                { "@STARTSPECDAY",STARTSPECDAY.ToString("yyyy-MM-dd HH:mm:00")},
        //                { "@ENDSPECDAY",ENDSPECDAY.ToString("yyyy-MM-dd HH:mm:59")},
        //                {"@DATEID",DATEID.ToString()},
        //                {"@YUANYING",YUANYING},
        //                {"@DATE",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
        //            });
        //                flag = true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    if (flag)
        //    {
        //        try
        //        {
        //            using (bllExtraer bll = new bllExtraer(this.strConn, _strDepartamento))
        //            {
        //                var list1 = bll.extraerEmpleado()
        //                        .Where(e => e.USERID == USERID)
        //                        .Select(e => new
        //                        {
        //                            EMPLEADO = e.USERID.ToString() + " - " + e.Name
        //                        })
        //                        .ToList();
        //                var list2 = bll.extraerTipoPermiso()
        //                        .Where(p => p.LEAVEID == DATEID)
        //                        .Select(p => new
        //                        {
        //                            PERMISO = ("(" + p.REPORTSYMBOL + ") " + p.LEAVENAME).ToUpper()
        //                        }).ToList();
        //                string _strEmpleado = string.Empty;
        //                foreach (var data in list1)
        //                    _strEmpleado = data.EMPLEADO;
        //                string str = string.Empty;
        //                foreach (var data in list2)
        //                    str = data.PERMISO;
        //                //this.reportePermiso(_strEmpleado, STARTSPECDAY, ENDSPECDAY, str.ToString(), YUANYING);
        //            }
        //        }
        //        catch (Exception )
        //        {
        //            throw ;
        //        }
        //    }
        //    return flag;
        //}

        //private void reportePermiso(string _strEmpleado,DateTime STARTSPECDAY,DateTime ENDSPECDAY,string _strPermiso,string YUANYING)
        //{
        //    DataRow row = this.dtPermiso.NewRow();
        //    row["EMPLEADO"] = (object)_strEmpleado;
        //    row["FECHA_INICO"] = (object)STARTSPECDAY;
        //    row["FECHA_FIN"] = (object)ENDSPECDAY;
        //    row["PERMISO"] = (object)_strPermiso;
        //    row["MOTIVO"] = (object)YUANYING;
        //    this.dtPermiso.Rows.Add(row);
        //}

        //private void reporteFeriado(string HOLIDAYNAME, DateTime STARTTIME, int DURATION)
        //{
        //    DataRow row = this.dtFeriado.NewRow();
        //    row["FERIADO"] = (object)HOLIDAYNAME;
        //    row["FECHA_INICIO"] = (object)STARTTIME;
        //    row["DURACION"] = (object)DURATION;
        //    this.dtFeriado.Rows.Add(row);
        //}

        //private void reporteMarcacionManual(string _strEmpleado, DateTime CHECKTIME, string _strTipo)
        //{
        //    DataRow row = this.dtMarcacionManual.NewRow();
        //    row["EMPLEADO"] = (object)_strEmpleado;
        //    row["FECHA_HORA"] = (object)CHECKTIME;
        //    row["TIPO"] = (object)_strTipo;
        //    this.dtMarcacionManual.Rows.Add(row);
        //}

        public void Dispose()
        {
        }
    }
}
