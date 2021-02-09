using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DTO.Empleados;
using DTO.Marcaciones;
using DTO.Turnos;
using BLL.Empleados;
using BLL.Marcaciones;
using BLL.Turnos;
using System.Configuration;
using NLog;

namespace SVC
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ControlAsistencia : IControlAsistencia,IDisposable
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string strProveedor
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["db"].ProviderName;
            }
        }

        private string strConn
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            }
        }

        private int intDuracionJornada
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["duration_work"]);
            }
        }

        private int intDuracionLunch
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["duration_lunch"]);
            }
        }

        private TimeSpan tsHoraInicio25
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["start_time_25"]);
            }
        }

        private TimeSpan tsHoraFin25
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["end_time_25"]);
            }
        }

        private TimeSpan tsHoraFin50
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["end_time_50"]);
            }
        }

        private TimeSpan tsHoraInicio100
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["start_time_100"]);
            }
        }

        private TimeSpan tsHoraFin100
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["end_time_100"]);
            }
        }

        private bool booFinSemana
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["weekend"]);
            }
        }

        private bool booInicioHorario
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["start_overtime"]);
            }
        }

        private TimeSpan tsInicioHorarioDesde
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["start_overtime_time"]);
            }
        }

        private bool booFinHorario
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["end_overtime"]);
            }
        }

        private TimeSpan tsSalidaHorarioDesde
        {
            get
            {
                return TimeSpan.Parse(ConfigurationManager.AppSettings["end_overtime_time"]);
            }
        }

        private string strUnidad {
            get
            {
                return ConfigurationManager.AppSettings["unity"];
            }
        }

        private string strSerial
        {
            get
            {
                return ConfigurationManager.AppSettings["serial"];
            }
        }

        public List<dtoTipoPermiso> extraerTipoPermiso()
        {
            List<dtoTipoPermiso> dtos = new List<dtoTipoPermiso>();
            try
            {
                using (bllPermiso bll = new bllPermiso(this.strConn))
                    dtos =  bll.execQueryTipoPermiso();
            }
            catch (Exception ex)
            {
                ControlAsistencia.logger.Error<Exception>(ex);
            }
            return dtos;
        }

        public List<dtoDepartamento> getDepartamento(string _str)
        {
            List<dtoDepartamento> dtos = new List<dtoDepartamento>();
            try
            {
                using (bllDepartamento bll = new bllDepartamento(this.strConn))
                dtos = bll.execQuery(new string[1]
                                    {
                                        _str
                                    });
            }
            catch (Exception ex)
            {
                ControlAsistencia.logger.Error<Exception>(ex);
            }
            return dtos;
        }

        public void procesarTurno(string[] _strIdDepartamento, int[] _intIdEmpleado ,  string _strEmpleado,DateTime _dtFechaInicio, DateTime _dtFechaFin)
        {
            try
            {
                List<dtoEmpleado> dtosEmpleado = new List<dtoEmpleado>();
                using (bllEmpleado bll = new bllEmpleado(this.strConn))
                    dtosEmpleado = bll.execQuery(_strIdDepartamento, _strEmpleado);

                List<dtoDepartamento> dtosDepartamento = new List<dtoDepartamento>();
                using (bllDepartamento bll = new bllDepartamento(this.strConn))
                    dtosDepartamento = bll.execQuery(new string[1]
                                        {
                                            string.Empty
                                        });

                var inner = from  e in dtosEmpleado 
                            join d in dtosDepartamento on e.DEFAULTDEPTID equals d.DEPTID 
                            where _intIdEmpleado.Contains(e.USERID)
                            select new
                            {
                                ID_EMPLEADO = e.USERID,
                                EMPLEADO = e.Name,
                                ID_DEPARTAMENTO = d.DEPTID,
                                DEPARTAMENTO = d.DEPTNAME,
                                CONTROL_ENTRADA_EMPLEADO = e.INLATE,
                                CONTROL_SALIDA_EMPLEADO = e.OUTEARLY,
                                FERIADO_EMPLEADO = e.HOLIDAY,
                                HORA_EXTRA_EMPLEADO = e.OVERTIME
                            };
                
                List<dtoFeriado> dtosFeriado = new List<dtoFeriado>();
                using (bllFeriado bll = new bllFeriado(this.strConn))
                    dtosFeriado = bll.execQuery(_dtFechaInicio, _dtFechaFin);
         
                List<dtoMarcacion> dtosMarcacion = new List<dtoMarcacion>();
                using (bllMarcacion bll = new bllMarcacion(this.strConn))
                    dtosMarcacion = bll.execQuery(_dtFechaInicio, _dtFechaFin, _intIdEmpleado);

                List<dtoPermiso> dtosPermiso = new List<dtoPermiso>();
                using (bllPermiso bll = new bllPermiso(this.strConn))
                    dtosPermiso = bll.execQueryPermiso(_dtFechaInicio, _dtFechaFin, _intIdEmpleado);

                List<dtoRegla> dtosRegla = new List<dtoRegla>();
                using (bllRegla bll = new bllRegla(this.strConn))
                    dtosRegla = bll.execQuery(_dtFechaInicio, _dtFechaFin);

                List<dtoHorario> dtosHorario = new List<dtoHorario>();
                using (bllTurno bll = new bllTurno(this.strConn))
                    dtosHorario = bll.execQueryHorario();

                List<dtoTurnoHorario> dtosTurnoHorario = new List<dtoTurnoHorario>();
                using (bllTurno bll = new bllTurno(this.strConn))
                    dtosTurnoHorario = bll.procesarTipoTurno(dtosMarcacion, dtosEmpleado, _intIdEmpleado, dtosHorario, _dtFechaInicio, _dtFechaFin);

                var outer = dtosTurnoHorario.Join(dtosHorario, (tt => tt.ID_HORARIO), (h => h.SCHCLASSID), (tt, h) => new
                {
                    tt = tt,
                    h = h
                }).OrderBy(_param1 => _param1.tt.FECHA_HORA_ENTRADA).ThenBy(_param1 => _param1.tt.ID_EMPLEADO).ThenBy(_param1 => _param1.tt.ID_TURNO).ThenBy(_param1 => _param1.h.SCHCLASSID).Select(_param1 =>
                {
                    int idEmpleado = _param1.tt.ID_EMPLEADO;
                    int idTurno = _param1.tt.ID_TURNO;
                    string turno = _param1.tt.TURNO;
                    string tipo = _param1.tt.TIPO;
                    string dia = _param1.tt.DIA;
                    int schclassid = _param1.h.SCHCLASSID;
                    string schname = _param1.h.SCHNAME;
                    DateTime dateTime1;
                    DateTime dateTime2;
                    if (!(_param1.h.CHECKINTIME1 <= _param1.h.STARTTIME))
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                        dateTime1 = dateTime1.AddDays(-1.0);
                        dateTime2 = dateTime1.Add(_param1.h.CHECKINTIME1);
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                        dateTime2 = dateTime1.Add(_param1.h.CHECKINTIME1);
                    }
                    dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                    DateTime dateTime3 = dateTime1.Add(_param1.h.STARTTIME);
                    DateTime dateTime4;
                    if (!(_param1.h.CHECKINTIME2 >= _param1.h.STARTTIME))
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                        dateTime1 = dateTime1.AddDays(1.0);
                        dateTime4 = dateTime1.Add(_param1.h.CHECKINTIME2);
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                        dateTime4 = dateTime1.Add(_param1.h.CHECKINTIME2);
                    }
                    int lateminutes = _param1.h.LATEMINUTES;
                    int num1 = _param1.h.CHECKIN ? 1 : 0;
                    DateTime dateTime5;
                    if (!(_param1.h.CHECKOUTTIME1 <= _param1.h.ENDTIME))
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                        dateTime1 = dateTime1.AddDays(-1.0);
                        dateTime5 = dateTime1.Add(_param1.h.CHECKOUTTIME1);
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                        dateTime5 = dateTime1.Add(_param1.h.CHECKOUTTIME1);
                    }
                    dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                    DateTime dateTime6 = dateTime1.Add(_param1.h.ENDTIME);
                    DateTime dateTime7;
                    if (!(_param1.h.CHECKOUTTIME2 >= _param1.h.ENDTIME))
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                        dateTime1 = dateTime1.AddDays(1.0);
                        dateTime7 = dateTime1.Add(_param1.h.CHECKOUTTIME2);
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                        dateTime7 = dateTime1.Add(_param1.h.CHECKOUTTIME2);
                    }
                    int earlyminutes = _param1.h.EARLYMINUTES;
                    int num2 = _param1.h.CHECKOUT ? 1 : 0;
                    int num3;
                    if (!this.booFinSemana)
                    {
                        num3 = dtosFeriado.Where((f =>
                        {
                            if (_param1.tt.FECHA_HORA_ENTRADA >= f.STARTTIME && _param1.tt.FECHA_HORA_ENTRADA <= f.ENDTIME)
                                return true;
                            return _param1.tt.FECHA_HORA_SALIDA >= f.STARTTIME && _param1.tt.FECHA_HORA_SALIDA <= f.ENDTIME;
                        })).Select((f => f.HOLIDAYNAME)).Count<string>() == 0 ? 0 : 1;
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                        if (dateTime1.DayOfWeek != DayOfWeek.Sunday)
                        {
                            dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                            if (dateTime1.DayOfWeek != DayOfWeek.Saturday)
                            {
                                num3 = dtosFeriado.Where((f =>
                                {
                                    if (_param1.tt.FECHA_HORA_ENTRADA >= f.STARTTIME && _param1.tt.FECHA_HORA_ENTRADA <= f.ENDTIME)
                                        return true;
                                    return _param1.tt.FECHA_HORA_SALIDA >= f.STARTTIME && _param1.tt.FECHA_HORA_SALIDA <= f.ENDTIME;
                                })).Select(f => f.HOLIDAYNAME).Count() == 0 ? 0 : 1;
                                goto label_18;
                            }
                        }
                        num3 = 1;
                    }
                label_18:
                    double workDay = _param1.h.WorkDay;
                    double workMins = _param1.h.WorkMins;
                    int num4 = _param1.h.STARTTIME >= this.tsHoraInicio25 || _param1.h.STARTTIME <= this.tsHoraFin25 ? 1 : (_param1.h.ENDTIME >= this.tsHoraInicio25 || _param1.h.ENDTIME <= this.tsHoraFin25 ? 1 : 0);
                    DateTime dateTime8;
                    if (!(_param1.h.STARTTIME >= this.tsHoraInicio25))
                    {
                        if (!(_param1.h.STARTTIME <= this.tsHoraFin25))
                        {
                            dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                            dateTime8 = dateTime1.Add(this.tsHoraInicio25);
                        }
                        else
                        {
                            dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                            dateTime8 = dateTime1.Add(_param1.h.STARTTIME);
                        }
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_ENTRADA;
                        dateTime8 = dateTime1.Add(_param1.h.STARTTIME);
                    }
                    DateTime dateTime9;
                    if (!(_param1.h.ENDTIME >= this.tsHoraFin25))
                    {
                        if (!(_param1.h.ENDTIME <= this.tsHoraFin25))
                        {
                            dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                            dateTime9 = dateTime1.Add(this.tsHoraFin25);
                        }
                        else
                        {
                            dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                            dateTime9 = dateTime1.Add(this.tsHoraFin25);
                        }
                    }
                    else
                    {
                        dateTime1 = _param1.tt.FECHA_HORA_SALIDA;
                        dateTime9 = dateTime1.Add(_param1.h.ENDTIME);
                    }
                    int num5 = _param1.tt.HORA_EXTRA ? 1 : 0;
                    return new
                    {
                        ID_EMPLEADO = idEmpleado,
                        ID_TURNO = idTurno,
                        TURNO = turno,
                        TIPO = tipo,
                        DIA = dia,
                        ID_HORARIO = schclassid,
                        HORARIO = schname,
                        FECHA_HORA_INICIO_ENTRADA = dateTime2,
                        FECHA_HORA_ENTRADA = dateTime3,
                        FECHA_HORA_FIN_ENTRADA = dateTime4,
                        MINS_ENTRADA_TARDE = lateminutes,
                        CONTROL_ENTRADA_HORARIO = num1 != 0,
                        FECHA_HORA_INICIO_SALIDA = dateTime5,
                        FECHA_HORA_SALIDA = dateTime6,
                        FECHA_HORA_FIN_SALIDA = dateTime7,
                        MINS_SALIDA_TEMPRANO = earlyminutes,
                        CONTROL_SALIDA_HORARIO = num2 != 0,
                        FERIADO = num3 != 0,
                        JORNADA = workDay,
                        MINS_TRABAJO = workMins,
                        HORA_NOCTURNA = num4 != 0,
                        FECHA_HORA_ENTRADA_JORNADA_NOCTURNA = dateTime8,
                        FECHA_HORA_SALIDA_JORNADA_NOCTURNA = dateTime9,
                        HORA_EXTRA = num5 != 0
                    };
                });
              
                string[] _strTipoTurno = new string[2] { "T", "R" };
                var datas = outer.Join(inner, t => t.ID_EMPLEADO, ed => ed.ID_EMPLEADO, (t, ed) => new
                {
                    t = t,
                    ed = ed
                }).SelectMany(_param1 => dtosRegla, (_param1, r) => new
                {
                    _param1 = _param1,
                    r = r
                }).Where(_param1 => _param1._param1.t.TIPO == "F" && _param1._param1.t.ID_TURNO == _param1._param1.t.ID_TURNO || (_strTipoTurno).Contains<string>(_param1._param1.t.TIPO)).Select(_param1 => new
                {
                    ID_EMPLEADO = _param1._param1.ed.ID_EMPLEADO,
                    EMPLEADO = _param1._param1.ed.EMPLEADO,
                    ID_DEPARTAMENTO = _param1._param1.ed.ID_DEPARTAMENTO,
                    DEPARTAMENTO = _param1._param1.ed.DEPARTAMENTO,
                    ID_TURNO = _param1._param1.t.ID_TURNO,
                    TURNO = _param1._param1.t.TURNO,
                    TIPO = _param1._param1.t.TIPO,
                    DIA = _param1._param1.t.DIA,
                    ID_HORARIO = _param1._param1.t.ID_HORARIO,
                    HORARIO = _param1._param1.t.HORARIO,
                    FECHA_HORA_INICIO_ENTRADA = _param1._param1.t.FECHA_HORA_INICIO_ENTRADA,
                    FECHA_HORA_ENTRADA = _param1._param1.t.FECHA_HORA_ENTRADA,
                    FECHA_HORA_FIN_ENTRADA = _param1._param1.t.FECHA_HORA_FIN_ENTRADA,
                    MINS_ENTRADA_TARDE = _param1._param1.t.MINS_ENTRADA_TARDE,
                    FECHA_HORA_INICIO_SALIDA = _param1._param1.t.FECHA_HORA_INICIO_SALIDA,
                    FECHA_HORA_SALIDA = _param1._param1.t.FECHA_HORA_SALIDA,
                    FECHA_HORA_FIN_SALIDA = _param1._param1.t.FECHA_HORA_FIN_SALIDA,
                    MINS_SALIDA_TEMPRANO = _param1._param1.t.MINS_SALIDA_TEMPRANO,
                    JORNADA = _param1._param1.t.JORNADA,
                    JORNADA_NOCTURNA = _param1._param1.t.HORA_NOCTURNA,
                    FECHA_HORA_ENTRADA_JORNADA_NOCTURNA = _param1._param1.t.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA,
                    FECHA_HORA_SALIDA_JORNADA_NOCTURNA = _param1._param1.t.FECHA_HORA_SALIDA_JORNADA_NOCTURNA,
                    MINS_LUNCH = (_param1._param1.t.FECHA_HORA_SALIDA - _param1._param1.t.FECHA_HORA_ENTRADA).TotalMinutes > _param1._param1.t.MINS_TRABAJO ? (_param1._param1.t.FECHA_HORA_SALIDA - _param1._param1.t.FECHA_HORA_ENTRADA).TotalMinutes - _param1._param1.t.MINS_TRABAJO : this.intDuracionLunch,
                    REGLA_ENTRADA = _param1.r.REGLA_ENTRADA,
                    FALTA_ENTRADA = _param1.r.FALTA_ENTRADA,
                    ENTRADA_TARDE = _param1.r.ENTRADA_TARDE,
                    MINS_ENTRADA = _param1.r.MINS_ENTRADA,
                    REGLA_SALIDA = _param1.r.REGLA_SALIDA,
                    FALTA_SALIDA = _param1.r.FALTA_SALIDA,
                    SALIDA_TEMPRANO = _param1.r.SALIDA_TEMPRANO,
                    MINS_SALIDA = _param1.r.MINS_SALIDA,
                    HORA_EXTRA = _param1._param1.t.FERIADO && _param1._param1.ed.FERIADO_EMPLEADO || (_param1._param1.t.HORA_EXTRA || _param1._param1.ed.HORA_EXTRA_EMPLEADO),
                    CONTROL_ENTRADA = _param1._param1.ed.CONTROL_ENTRADA_EMPLEADO,
                    CONTROL_SALIDA = _param1._param1.ed.CONTROL_SALIDA_EMPLEADO,
                    FERIADO = _param1._param1.t.FERIADO,
                    PERMISO_VACACION = dtosPermiso.Where((p => (_param1._param1.t.FECHA_HORA_ENTRADA >= p.STARTSPECDAY && _param1._param1.t.FECHA_HORA_ENTRADA <= p.ENDSPECDAY || _param1._param1.t.FECHA_HORA_SALIDA >= p.STARTSPECDAY && _param1._param1.t.FECHA_HORA_SALIDA <= p.ENDSPECDAY) && p.USERID == _param1._param1.ed.ID_EMPLEADO)).Select((p => p.USERID)).Count() != 0
                }).Select(t => new
                {
                    ID_EMPLEADO = t.ID_EMPLEADO,
                    EMPLEADO = t.EMPLEADO,
                    ID_DEPARTAMENTO = t.ID_DEPARTAMENTO,
                    DEPARTAMENTO = t.DEPARTAMENTO,
                    ID_TURNO = t.ID_TURNO,
                    TURNO = t.TURNO,
                    TIPO = t.TIPO,
                    DIA = t.DIA,
                    ID_HORARIO = t.ID_HORARIO,
                    HORARIO = t.HORARIO,
                    FECHA_HORA_INICIO_ENTRADA = t.FECHA_HORA_INICIO_ENTRADA,
                    FECHA_HORA_ENTRADA = t.FECHA_HORA_ENTRADA,
                    FECHA_HORA_FIN_ENTRADA = t.FECHA_HORA_FIN_ENTRADA,
                    MINS_ENTRADA_TARDE = t.MINS_ENTRADA_TARDE,
                    MARCACION_ENTRADA = t.CONTROL_ENTRADA == 0 ? dtosMarcacion.Where((m => m.CHECKTIME >= t.FECHA_HORA_INICIO_ENTRADA && m.CHECKTIME <= t.FECHA_HORA_FIN_ENTRADA && m.USERID == t.ID_EMPLEADO)).Select((m => m.CHECKTIME)).DefaultIfEmpty().Min() : (t.CONTROL_ENTRADA == 1 ? dtosMarcacion.Where((m => m.CHECKTIME >= t.FECHA_HORA_INICIO_ENTRADA && m.CHECKTIME <= t.FECHA_HORA_FIN_ENTRADA && m.CHECKTYPE == "I" && m.USERID == t.ID_EMPLEADO)).Select((m => m.CHECKTIME)).DefaultIfEmpty().Min() : t.FECHA_HORA_ENTRADA),
                    MARCACION_SALIDA = t.CONTROL_SALIDA == 0 ? dtosMarcacion.Where((m => m.CHECKTIME >= t.FECHA_HORA_INICIO_SALIDA && m.CHECKTIME <= t.FECHA_HORA_FIN_SALIDA && m.USERID == t.ID_EMPLEADO && m.CHECKTIME > dtosMarcacion.Where((mm => mm.CHECKTIME >= t.FECHA_HORA_INICIO_ENTRADA && mm.CHECKTIME <= t.FECHA_HORA_FIN_ENTRADA && mm.USERID == t.ID_EMPLEADO)).Select((mm => mm.CHECKTIME)).DefaultIfEmpty().Min())).Select((m => m.CHECKTIME)).DefaultIfEmpty().Max() : (t.CONTROL_SALIDA == 1 ? dtosMarcacion.Where((m => m.CHECKTIME >= t.FECHA_HORA_INICIO_SALIDA && m.CHECKTIME <= t.FECHA_HORA_FIN_SALIDA && (m.CHECKTYPE == "O" && m.USERID == t.ID_EMPLEADO) && m.CHECKTIME > dtosMarcacion.Where((mm => mm.CHECKTIME >= t.FECHA_HORA_INICIO_ENTRADA && mm.CHECKTIME <= t.FECHA_HORA_FIN_ENTRADA && mm.CHECKTYPE == "I" && mm.USERID == t.ID_EMPLEADO)).Select((mm => mm.CHECKTIME)).DefaultIfEmpty().Min())).Select((m => m.CHECKTIME)).DefaultIfEmpty().Max() : t.FECHA_HORA_SALIDA),
                    FECHA_HORA_INICIO_SALIDA = t.FECHA_HORA_INICIO_SALIDA,
                    FECHA_HORA_SALIDA = t.FECHA_HORA_SALIDA,
                    FECHA_HORA_FIN_SALIDA = t.FECHA_HORA_FIN_SALIDA,
                    MINS_SALIDA_TEMPRANO = t.MINS_SALIDA_TEMPRANO,
                    JORNADA = t.JORNADA,
                    JORNADA_NOCTURNA = t.JORNADA_NOCTURNA,
                    FECHA_HORA_ENTRADA_JORNADA_NOCTURNA = t.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA,
                    FECHA_HORA_SALIDA_JORNADA_NOCTURNA = t.FECHA_HORA_SALIDA_JORNADA_NOCTURNA,
                    REGLA_ENTRADA = t.REGLA_ENTRADA,
                    FALTA_ENTRADA = t.FALTA_ENTRADA,
                    ENTRADA_TARDE = t.ENTRADA_TARDE,
                    MINS_ENTRADA = t.MINS_ENTRADA,
                    REGLA_SALIDA = t.REGLA_SALIDA,
                    FALTA_SALIDA = t.FALTA_SALIDA,
                    SALIDA_TEMPRANO = t.SALIDA_TEMPRANO,
                    MINS_SALIDA = t.MINS_SALIDA,
                    HORA_EXTRA = t.HORA_EXTRA,
                    MINS_LUNCH = t.MINS_LUNCH,
                    FERIADO = t.FERIADO,
                    PERMISO_VACACION = t.PERMISO_VACACION
                }).Select(etmr =>
                {
                    int idEmpleado = etmr.ID_EMPLEADO;
                    string empleado = etmr.EMPLEADO;
                    int idDepartamento = etmr.ID_DEPARTAMENTO;
                    string departamento = etmr.DEPARTAMENTO;
                    int idTurno = etmr.ID_TURNO;
                    string turno = etmr.TURNO;
                    string tipo = etmr.TIPO;
                    string dia = etmr.DIA;
                    int idHorario = etmr.ID_HORARIO;
                    string horario = etmr.HORARIO;
                    DateTime dateTime1;
                    DateTime dateTime2;
                    if (etmr.MARCACION_ENTRADA.Year == 1)
                    {
                        dateTime1 = etmr.MARCACION_SALIDA;
                        if (dateTime1.Year == 1)
                        {
                            dateTime2 = etmr.MARCACION_SALIDA;
                            goto label_12;
                        }
                    }
                    dateTime1 = etmr.MARCACION_ENTRADA;
                    if (dateTime1.Year == 1)
                    {
                        if (etmr.REGLA_ENTRADA)
                        {
                            if (!etmr.FALTA_ENTRADA)
                            {
                                if (!etmr.ENTRADA_TARDE)
                                {
                                    dateTime2 = etmr.MARCACION_ENTRADA;
                                }
                                else
                                {
                                    dateTime1 = etmr.FECHA_HORA_ENTRADA;
                                    dateTime2 = dateTime1.AddMinutes(etmr.MINS_ENTRADA);
                                }
                            }
                            else
                                dateTime2 = etmr.MARCACION_ENTRADA;
                        }
                        else
                            dateTime2 = etmr.MARCACION_ENTRADA;
                    }
                    else
                        dateTime2 = etmr.MARCACION_ENTRADA;
                    label_12:
                    DateTime horaInicioEntrada = etmr.FECHA_HORA_INICIO_ENTRADA;
                    DateTime fechaHoraEntrada = etmr.FECHA_HORA_ENTRADA;
                    DateTime fechaHoraFinEntrada = etmr.FECHA_HORA_FIN_ENTRADA;
                    int minsEntradaTarde = etmr.MINS_ENTRADA_TARDE;
                    dateTime1 = etmr.MARCACION_ENTRADA;
                    DateTime dateTime3;
                    if (dateTime1.Year == 1)
                    {
                        dateTime1 = etmr.MARCACION_SALIDA;
                        if (dateTime1.Year == 1)
                        {
                            dateTime3 = etmr.MARCACION_SALIDA;
                            goto label_24;
                        }
                    }
                    dateTime1 = etmr.MARCACION_SALIDA;
                    if (dateTime1.Year == 1)
                    {
                        if (etmr.REGLA_SALIDA)
                        {
                            if (!etmr.FALTA_SALIDA)
                            {
                                if (!etmr.SALIDA_TEMPRANO)
                                {
                                    dateTime3 = etmr.MARCACION_SALIDA;
                                }
                                else
                                {
                                    dateTime1 = etmr.FECHA_HORA_SALIDA;
                                    dateTime3 = dateTime1.AddMinutes(etmr.MINS_SALIDA);
                                }
                            }
                            else
                                dateTime3 = etmr.MARCACION_SALIDA;
                        }
                        else
                            dateTime3 = etmr.MARCACION_SALIDA;
                    }
                    else
                        dateTime3 = etmr.MARCACION_SALIDA;
                    label_24:
                    DateTime horaInicioSalida = etmr.FECHA_HORA_INICIO_SALIDA;
                    DateTime fechaHoraSalida = etmr.FECHA_HORA_SALIDA;
                    DateTime fechaHoraFinSalida = etmr.FECHA_HORA_FIN_SALIDA;
                    int minsSalidaTemprano = etmr.MINS_SALIDA_TEMPRANO;
                    int num1 = etmr.HORA_EXTRA ? 1 : 0;
                    int num2 = etmr.FERIADO ? 1 : 0;
                    int num3;
                    if (etmr.PERMISO_VACACION)
                    {
                        num3 = 0;
                    }
                    else
                    {
                        dateTime1 = etmr.MARCACION_ENTRADA;
                        if (dateTime1.Year == 1)
                        {
                            dateTime1 = etmr.MARCACION_SALIDA;
                            if (dateTime1.Year == 1)
                            {
                                num3 = 1;
                                goto label_30;
                            }
                        }
                        num3 = 0;
                    }
                label_30:
                    int num4;
                    if (etmr.PERMISO_VACACION)
                    {
                        num4 = 0;
                    }
                    else
                    {
                        dateTime1 = etmr.MARCACION_ENTRADA;
                        if (dateTime1.Year != 1)
                        {
                            dateTime1 = etmr.MARCACION_SALIDA;
                            if (dateTime1.Year != 1)
                            {
                                num4 = 0;
                                goto label_36;
                            }
                        }
                        num4 = 1;
                    }
                label_36:
                    int num5 = etmr.PERMISO_VACACION ? 1 : 0;
                    double jornada = etmr.JORNADA;
                    double minsLunch = etmr.MINS_LUNCH;
                    int num6 = etmr.JORNADA_NOCTURNA ? 1 : 0;
                    DateTime entradaJornadaNocturna = etmr.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA;
                    DateTime salidaJornadaNocturna = etmr.FECHA_HORA_SALIDA_JORNADA_NOCTURNA;
                    return new
                    {
                        ID_EMPLEADO = idEmpleado,
                        EMPLEADO = empleado,
                        ID_DEPARTAMENTO = idDepartamento,
                        DEPARTAMENTO = departamento,
                        ID_TURNO = idTurno,
                        TURNO = turno,
                        TIPO = tipo,
                        DIA = dia,
                        ID_HORARIO = idHorario,
                        HORARIO = horario,
                        MARCACION_ENTRADA = dateTime2,
                        FECHA_HORA_INICIO_ENTRADA = horaInicioEntrada,
                        FECHA_HORA_ENTRADA = fechaHoraEntrada,
                        FECHA_HORA_FIN_ENTRADA = fechaHoraFinEntrada,
                        MINS_ENTRADA_TARDE = minsEntradaTarde,
                        MARCACION_SALIDA = dateTime3,
                        FECHA_HORA_INICIO_SALIDA = horaInicioSalida,
                        FECHA_HORA_SALIDA = fechaHoraSalida,
                        FECHA_HORA_FIN_SALIDA = fechaHoraFinSalida,
                        MINS_SALIDA_TEMPRANO = minsSalidaTemprano,
                        HORA_EXTRA = num1 != 0,
                        FERIADO = num2 != 0,
                        FALTA = num3 != 0,
                        FALTA_PARCIAL = num4 != 0,
                        PERMISO_VACACION = num5 != 0,
                        JORNADA = jornada,
                        MINS_LUNCH = minsLunch,
                        JORNADA_NOCTURNA = num6 != 0,
                        FECHA_HORA_ENTRADA_JORNADA_NOCTURNA = entradaJornadaNocturna,
                        FECHA_HORA_SALIDA_JORNADA_NOCTURNA = salidaJornadaNocturna
                    };
                }).Select(he =>
                {
                    int idEmpleado = he.ID_EMPLEADO;
                    string empleado = he.EMPLEADO;
                    int idDepartamento = he.ID_DEPARTAMENTO;
                    string departamento = he.DEPARTAMENTO;
                    int idTurno = he.ID_TURNO;
                    string turno = he.TURNO;
                    string tipo = he.TIPO;
                    int idHorario = he.ID_HORARIO;
                    string horario = he.HORARIO;
                    string dia = he.DIA;
                    DateTime marcacionEntrada1 = he.MARCACION_ENTRADA;
                    DateTime horaInicioEntrada = he.FECHA_HORA_INICIO_ENTRADA;
                    DateTime fechaHoraEntrada1 = he.FECHA_HORA_ENTRADA;
                    DateTime fechaHoraFinEntrada = he.FECHA_HORA_FIN_ENTRADA;
                    int minsEntradaTarde = he.MINS_ENTRADA_TARDE;
                    DateTime dateTime1;
                    TimeSpan timeSpan;
                    double num1;
                    if (he.MARCACION_ENTRADA.Year != 1)
                    {
                        DateTime marcacionEntrada2 = he.MARCACION_ENTRADA;
                        dateTime1 = he.FECHA_HORA_ENTRADA;
                        DateTime dateTime2 = dateTime1.AddMinutes(he.MINS_ENTRADA_TARDE);
                        if (!(marcacionEntrada2 > dateTime2))
                        {
                            num1 = 0.0;
                        }
                        else
                        {
                            timeSpan = he.MARCACION_ENTRADA - he.FECHA_HORA_ENTRADA;
                            num1 = timeSpan.TotalMinutes;
                        }
                    }
                    else
                        num1 = 0.0;
                    double num2 = Math.Abs(num1);
                    dateTime1 = he.MARCACION_ENTRADA;
                    double num3;
                    if (dateTime1.Year != 1 && this.booInicioHorario)
                    {
                        if (!(he.MARCACION_ENTRADA < he.FECHA_HORA_ENTRADA))
                        {
                            num3 = 0.0;
                        }
                        else
                        {
                            timeSpan = he.MARCACION_ENTRADA - he.FECHA_HORA_ENTRADA;
                            num3 = timeSpan.TotalMinutes;
                        }
                    }
                    else
                        num3 = 0.0;
                    double num4 = Math.Abs(num3);
                    DateTime marcacionSalida1 = he.MARCACION_SALIDA;
                    DateTime horaInicioSalida = he.FECHA_HORA_INICIO_SALIDA;
                    DateTime fechaHoraSalida = he.FECHA_HORA_SALIDA;
                    DateTime fechaHoraFinSalida = he.FECHA_HORA_FIN_SALIDA;
                    int minsSalidaTemprano = he.MINS_SALIDA_TEMPRANO;
                    dateTime1 = he.MARCACION_SALIDA;
                    double num5;
                    if (dateTime1.Year != 1)
                    {
                        dateTime1 = he.MARCACION_SALIDA;
                        if (!(dateTime1.AddMinutes(he.MINS_SALIDA_TEMPRANO) < he.FECHA_HORA_SALIDA))
                        {
                            num5 = 0.0;
                        }
                        else
                        {
                            timeSpan = he.MARCACION_SALIDA - he.FECHA_HORA_SALIDA;
                            num5 = timeSpan.TotalMinutes;
                        }
                    }
                    else
                        num5 = 0.0;
                    double num6 = Math.Abs(num5);
                    dateTime1 = he.MARCACION_SALIDA;
                    double num7;
                    if (dateTime1.Year != 1 && this.booFinHorario)
                    {
                        if (!(he.MARCACION_SALIDA > he.FECHA_HORA_SALIDA))
                        {
                            num7 = 0.0;
                        }
                        else
                        {
                            timeSpan = he.MARCACION_SALIDA - he.FECHA_HORA_SALIDA;
                            num7 = timeSpan.TotalMinutes;
                        }
                    }
                    else
                        num7 = 0.0;
                    double num8 = Math.Abs(num7);
                    double minsLunch = he.MINS_LUNCH;
                    DateTime dateTime3 = he.MINS_LUNCH > 0.0 ? dtosMarcacion.Where((m => m.CHECKTIME > he.MARCACION_ENTRADA && m.CHECKTIME < he.MARCACION_SALIDA && m.USERID == he.ID_EMPLEADO)).Select((m => m.CHECKTIME)).DefaultIfEmpty().Min() : DateTime.MinValue;
                    DateTime dateTime4 = he.MINS_LUNCH > 0.0 ? dtosMarcacion.Where((m => m.CHECKTIME > he.MARCACION_ENTRADA && m.CHECKTIME < he.MARCACION_SALIDA && m.USERID == he.ID_EMPLEADO && m.CHECKTIME > dtosMarcacion.Where((mm => mm.CHECKTIME > he.MARCACION_ENTRADA && mm.CHECKTIME < he.MARCACION_SALIDA && mm.USERID == he.ID_EMPLEADO)).Select((mm => mm.CHECKTIME)).DefaultIfEmpty().Min())).Select((m => m.CHECKTIME)).DefaultIfEmpty().Max() : DateTime.MinValue;
                    double jornada = he.JORNADA;
                    double num9;
                    if (he.FALTA)
                        num9 = 0.0;
                    else if (!he.JORNADA_NOCTURNA)
                        num9 = 0.0;
                    else if (!((he.MARCACION_ENTRADA >= he.FECHA_HORA_ENTRADA ? he.MARCACION_ENTRADA : he.FECHA_HORA_ENTRADA) >= he.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA) || !((he.MARCACION_ENTRADA >= he.FECHA_HORA_ENTRADA ? he.MARCACION_ENTRADA : he.FECHA_HORA_ENTRADA) <= he.FECHA_HORA_SALIDA_JORNADA_NOCTURNA))
                    {
                        timeSpan = he.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA - (he.MARCACION_ENTRADA >= he.FECHA_HORA_ENTRADA ? he.MARCACION_ENTRADA : he.FECHA_HORA_ENTRADA);
                        num9 = timeSpan.TotalMinutes;
                    }
                    else
                        num9 = 0.0;
                    double num10;
                    if (he.FALTA)
                        num10 = 0.0;
                    else if (!he.JORNADA_NOCTURNA)
                        num10 = 0.0;
                    else if (!((he.MARCACION_SALIDA >= he.FECHA_HORA_SALIDA ? he.FECHA_HORA_SALIDA : he.MARCACION_SALIDA) >= he.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA) || !((he.MARCACION_SALIDA >= he.FECHA_HORA_SALIDA ? he.FECHA_HORA_SALIDA : he.MARCACION_SALIDA) <= he.FECHA_HORA_SALIDA_JORNADA_NOCTURNA))
                    {
                        timeSpan = (he.MARCACION_SALIDA >= he.FECHA_HORA_SALIDA ? he.FECHA_HORA_SALIDA : he.MARCACION_SALIDA) - he.FECHA_HORA_SALIDA_JORNADA_NOCTURNA;
                        num10 = timeSpan.TotalMinutes;
                    }
                    else
                        num10 = 0.0;
                    double num11;
                    if (!he.FALTA)
                    {
                        dateTime1 = he.MARCACION_SALIDA;
                        if (dateTime1.Year != 1)
                        {
                            dateTime1 = he.MARCACION_ENTRADA;
                            if (dateTime1.Year != 1)
                            {
                                if (!he.JORNADA_NOCTURNA)
                                {
                                    timeSpan = (he.MARCACION_SALIDA <= he.FECHA_HORA_SALIDA ? he.MARCACION_SALIDA : he.FECHA_HORA_SALIDA) - (he.MARCACION_ENTRADA >= he.FECHA_HORA_ENTRADA ? he.MARCACION_ENTRADA : he.FECHA_HORA_ENTRADA);
                                    num11 = timeSpan.TotalMinutes;
                                    goto label_41;
                                }
                                else
                                {
                                    num11 = 0.0;
                                    goto label_41;
                                }
                            }
                        }
                    }
                    num11 = 0.0;
                label_41:
                    double num12;
                    if (!he.FALTA)
                    {
                        dateTime1 = he.MARCACION_SALIDA;
                        if (dateTime1.Year != 1)
                        {
                            dateTime1 = he.MARCACION_ENTRADA;
                            if (dateTime1.Year != 1 && he.JORNADA_NOCTURNA)
                            {
                                timeSpan = (he.MARCACION_SALIDA <= he.FECHA_HORA_SALIDA_JORNADA_NOCTURNA ? he.MARCACION_SALIDA : he.FECHA_HORA_SALIDA_JORNADA_NOCTURNA) - (he.MARCACION_ENTRADA >= he.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA ? he.MARCACION_ENTRADA : he.FECHA_HORA_ENTRADA_JORNADA_NOCTURNA);
                                num12 = timeSpan.TotalMinutes;
                                goto label_46;
                            }
                        }
                    }
                    num12 = 0.0;
                label_46:
                    int num13 = he.HORA_EXTRA ? 1 : 0;
                    int num14 = he.FERIADO ? 1 : 0;
                    int num15 = he.FALTA ? 1 : 0;
                    int num16 = he.FALTA_PARCIAL ? 1 : 0;
                    int num17 = he.PERMISO_VACACION ? 1 : 0;
                    dateTime1 = he.FECHA_HORA_ENTRADA;
                    ref DateTime local1 = ref dateTime1;
                    DateTime dateTime5;
                    double num18;
                    if (!this.booInicioHorario)
                    {
                        num18 = 0.0;
                    }
                    else
                    {
                        dateTime5 = he.MARCACION_ENTRADA;
                        double num19;
                        if (dateTime5.Year != 1 && this.booInicioHorario)
                        {
                            if (!(he.MARCACION_ENTRADA < he.FECHA_HORA_ENTRADA))
                            {
                                num19 = 0.0;
                            }
                            else
                            {
                                timeSpan = he.MARCACION_ENTRADA - he.FECHA_HORA_ENTRADA;
                                num19 = timeSpan.TotalMinutes;
                            }
                        }
                        else
                            num19 = 0.0;
                        double num20 = Math.Abs(num19);
                        dateTime5 = he.MARCACION_SALIDA;
                        double num21;
                        if (dateTime5.Year != 1)
                        {
                            dateTime5 = he.MARCACION_SALIDA;
                            if (!(dateTime5.AddMinutes((double)he.MINS_SALIDA_TEMPRANO) < he.FECHA_HORA_SALIDA))
                            {
                                num21 = 0.0;
                            }
                            else
                            {
                                timeSpan = he.MARCACION_SALIDA - he.FECHA_HORA_SALIDA;
                                num21 = timeSpan.TotalMinutes;
                            }
                        }
                        else
                            num21 = 0.0;
                        double num22 = Math.Abs(num21);
                        double num23;
                        if (num20 <= num22)
                        {
                            num23 = 0.0;
                        }
                        else
                        {
                            dateTime5 = he.MARCACION_ENTRADA;
                            double num24;
                            if (dateTime5.Year != 1 && this.booInicioHorario)
                            {
                                if (!(he.MARCACION_ENTRADA < he.FECHA_HORA_ENTRADA))
                                {
                                    num24 = 0.0;
                                }
                                else
                                {
                                    timeSpan = he.MARCACION_ENTRADA - he.FECHA_HORA_ENTRADA;
                                    num24 = timeSpan.TotalMinutes;
                                }
                            }
                            else
                                num24 = 0.0;
                            double num25 = Math.Abs(num24);
                            dateTime5 = he.MARCACION_SALIDA;
                            double num26;
                            if (dateTime5.Year != 1)
                            {
                                dateTime5 = he.MARCACION_SALIDA;
                                if (!(dateTime5.AddMinutes((double)he.MINS_SALIDA_TEMPRANO) < he.FECHA_HORA_SALIDA))
                                {
                                    num26 = 0.0;
                                }
                                else
                                {
                                    timeSpan = he.MARCACION_SALIDA - he.FECHA_HORA_SALIDA;
                                    num26 = timeSpan.TotalMinutes;
                                }
                            }
                            else
                                num26 = 0.0;
                            double num27 = Math.Abs(num26);
                            num23 = num25 - num27;
                        }
                        num18 = num23 * -1.0;
                    }
                    DateTime dateTime6 = local1.AddMinutes(num18);
                    DateTime fechaHoraEntrada2 = he.FECHA_HORA_ENTRADA;
                    dateTime1 = he.MARCACION_SALIDA;
                    ref DateTime local2 = ref dateTime1;
                    double num28;
                    if (!this.booFinHorario)
                    {
                        num28 = 0.0;
                    }
                    else
                    {
                        dateTime5 = he.MARCACION_SALIDA;
                        double num19;
                        if (dateTime5.Year != 1 && this.booFinHorario)
                        {
                            if (!(he.MARCACION_SALIDA > he.FECHA_HORA_SALIDA))
                            {
                                num19 = 0.0;
                            }
                            else
                            {
                                timeSpan = he.MARCACION_SALIDA - he.FECHA_HORA_SALIDA;
                                num19 = timeSpan.TotalMinutes;
                            }
                        }
                        else
                            num19 = 0.0;
                        double num20 = Math.Abs(num19);
                        dateTime5 = he.MARCACION_ENTRADA;
                        double num21;
                        if (dateTime5.Year != 1)
                        {
                            DateTime marcacionEntrada2 = he.MARCACION_ENTRADA;
                            dateTime5 = he.FECHA_HORA_ENTRADA;
                            DateTime dateTime2 = dateTime5.AddMinutes((double)he.MINS_ENTRADA_TARDE);
                            if (!(marcacionEntrada2 > dateTime2))
                            {
                                num21 = 0.0;
                            }
                            else
                            {
                                timeSpan = he.MARCACION_ENTRADA - he.FECHA_HORA_ENTRADA;
                                num21 = timeSpan.TotalMinutes;
                            }
                        }
                        else
                            num21 = 0.0;
                        double num22 = Math.Abs(num21);
                        double num23;
                        if (num20 <= num22)
                        {
                            num23 = 0.0;
                        }
                        else
                        {
                            dateTime5 = he.MARCACION_SALIDA;
                            double num24;
                            if (dateTime5.Year != 1 && this.booFinHorario)
                            {
                                if (!(he.MARCACION_SALIDA > he.FECHA_HORA_SALIDA))
                                {
                                    num24 = 0.0;
                                }
                                else
                                {
                                    timeSpan = he.MARCACION_SALIDA - he.FECHA_HORA_SALIDA;
                                    num24 = timeSpan.TotalMinutes;
                                }
                            }
                            else
                                num24 = 0.0;
                            double num25 = Math.Abs(num24);
                            dateTime5 = he.MARCACION_ENTRADA;
                            double num26;
                            if (dateTime5.Year != 1)
                            {
                                DateTime marcacionEntrada2 = he.MARCACION_ENTRADA;
                                dateTime5 = he.FECHA_HORA_ENTRADA;
                                DateTime dateTime2 = dateTime5.AddMinutes((double)he.MINS_ENTRADA_TARDE);
                                if (!(marcacionEntrada2 > dateTime2))
                                {
                                    num26 = 0.0;
                                }
                                else
                                {
                                    timeSpan = he.MARCACION_ENTRADA - he.FECHA_HORA_ENTRADA;
                                    num26 = timeSpan.TotalMinutes;
                                }
                            }
                            else
                                num26 = 0.0;
                            double num27 = Math.Abs(num26);
                            num23 = num25 - num27;
                        }
                        num28 = num23 * -1.0;
                    }
                    DateTime dateTime7 = local2.AddMinutes(num28);
                    DateTime marcacionSalida2 = he.MARCACION_SALIDA;
                    dateTime1 = he.MARCACION_ENTRADA;
                    double num29;
                    if (dateTime1.Year != 1)
                    {
                        dateTime1 = he.MARCACION_SALIDA;
                        if (dateTime1.Year != 1)
                        {
                            timeSpan = he.MARCACION_SALIDA - he.MARCACION_ENTRADA;
                            num29 = timeSpan.TotalMinutes;
                            goto label_102;
                        }
                    }
                    num29 = 0.0;
                label_102:
                    return new
                    {
                        ID_EMPLEADO = idEmpleado,
                        EMPLEADO = empleado,
                        ID_DEPARTAMENTO = idDepartamento,
                        DEPARTAMENTO = departamento,
                        ID_TURNO = idTurno,
                        TURNO = turno,
                        TIPO = tipo,
                        ID_HORARIO = idHorario,
                        HORARIO = horario,
                        DIA = dia,
                        MARCACION_ENTRADA = marcacionEntrada1,
                        FECHA_HORA_INICIO_ENTRADA = horaInicioEntrada,
                        FECHA_HORA_ENTRADA = fechaHoraEntrada1,
                        FECHA_HORA_FIN_ENTRADA = fechaHoraFinEntrada,
                        MINS_ENTRADA_TARDE = minsEntradaTarde,
                        ENTRADA_TARDE = num2,
                        ENTRADA_TEMPRANO = num4,
                        MARCACION_SALIDA = marcacionSalida1,
                        FECHA_HORA_INICIO_SALIDA = horaInicioSalida,
                        FECHA_HORA_SALIDA = fechaHoraSalida,
                        FECHA_HORA_FIN_SALIDA = fechaHoraFinSalida,
                        MINS_SALIDA_TEMPRANO = minsSalidaTemprano,
                        SALIDA_TEMPRANO = num6,
                        SALIDA_TARDE = num8,
                        MINS_LUNCH = minsLunch,
                        MARCACION_ENTRADA_LUNCH = dateTime3,
                        MARCACION_SALIDA_LUNCH = dateTime4,
                        JORNADA = jornada,
                        MINS_ENTRADA_JORNADA_NORMAL = num9,
                        MINS_SALIDA_JORNADA_NORMAL = num10,
                        MINS_JORNADA_NORMAL = num11,
                        JORNADA_NOCTURNA = num12,
                        HORA_EXTRA = num13 != 0,
                        FERIADO = num14 != 0,
                        FALTA = num15 != 0,
                        FALTA_PARCIAL = num16 != 0,
                        PERMISO_VACACION = num17 != 0,
                        FECHA_HORA_EXTRA_INICIO_ENTRADA = dateTime6,
                        FECHA_HORA_EXTRA_FIN_ENTRADA = fechaHoraEntrada2,
                        FECHA_HORA_EXTRA_INICIO_SALIDA = dateTime7,
                        FECHA_HORA_EXTRA_FIN_SALIDA = marcacionSalida2,
                        TOTAL_ASISTIDO = num29
                    };
                }).Select(c =>
                {
                    int idEmpleado = c.ID_EMPLEADO;
                    string empleado = c.EMPLEADO;
                    int idDepartamento = c.ID_DEPARTAMENTO;
                    string departamento = c.DEPARTAMENTO;
                    int idTurno = c.ID_TURNO;
                    string turno = c.TURNO;
                    string tipo = c.TIPO;
                    int idHorario = c.ID_HORARIO;
                    string horario = c.HORARIO;
                    string dia = c.DIA;
                    DateTime marcacionEntrada = c.MARCACION_ENTRADA;
                    DateTime horaInicioEntrada = c.FECHA_HORA_INICIO_ENTRADA;
                    DateTime fechaHoraEntrada1 = c.FECHA_HORA_ENTRADA;
                    DateTime fechaHoraFinEntrada = c.FECHA_HORA_FIN_ENTRADA;
                    int minsEntradaTarde = c.MINS_ENTRADA_TARDE;
                    double entradaTarde = c.ENTRADA_TARDE;
                    double entradaTemprano = c.ENTRADA_TEMPRANO;
                    DateTime marcacionSalida1 = c.MARCACION_SALIDA;
                    DateTime horaInicioSalida = c.FECHA_HORA_INICIO_SALIDA;
                    DateTime fechaHoraSalida = c.FECHA_HORA_SALIDA;
                    DateTime fechaHoraFinSalida = c.FECHA_HORA_FIN_SALIDA;
                    int minsSalidaTemprano = c.MINS_SALIDA_TEMPRANO;
                    double salidaTemprano = c.SALIDA_TEMPRANO;
                    double salidaTarde = c.SALIDA_TARDE;
                    double minsLunch = c.MINS_LUNCH;
                    DateTime marcacionEntradaLunch = c.MARCACION_ENTRADA_LUNCH;
                    DateTime marcacionSalidaLunch = c.MARCACION_SALIDA_LUNCH;
                    DateTime dateTime;
                    TimeSpan timeSpan1;
                    double num1;
                    if (c.MARCACION_SALIDA_LUNCH.Year != 1)
                    {
                        dateTime = c.MARCACION_ENTRADA_LUNCH;
                        if (dateTime.Year != 1)
                        {
                            timeSpan1 = c.MARCACION_SALIDA_LUNCH - c.MARCACION_ENTRADA_LUNCH;
                            num1 = timeSpan1.TotalMinutes;
                            goto label_4;
                        }
                    }
                    num1 = 0.0;
                label_4:
                    double jornada = c.JORNADA;
                    double num2 = c.MINS_ENTRADA_JORNADA_NORMAL + c.MINS_SALIDA_JORNADA_NORMAL + c.MINS_JORNADA_NORMAL >= (double)this.intDuracionJornada ? (double)this.intDuracionJornada : c.MINS_ENTRADA_JORNADA_NORMAL + c.MINS_SALIDA_JORNADA_NORMAL + c.MINS_JORNADA_NORMAL;
                    double num3 = c.JORNADA_NOCTURNA >= (double)this.intDuracionJornada ? (double)this.intDuracionJornada : c.JORNADA_NOCTURNA;
                    int num4 = c.HORA_EXTRA ? 1 : 0;
                    int num5 = c.FERIADO ? 1 : 0;
                    int num6 = c.FALTA ? 1 : 0;
                    int num7 = c.PERMISO_VACACION ? 1 : 0;
                    double num8;
                    if (c.FALTA_PARCIAL || c.TOTAL_ASISTIDO <= (double)this.intDuracionJornada)
                    {
                        num8 = 0.0;
                    }
                    else
                    {
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                        double num9;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraFin100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin50)
                            {
                                timeSpan1 = c.FECHA_HORA_EXTRA_FIN_ENTRADA - c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                                num9 = timeSpan1.TotalMinutes;
                                goto label_16;
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraFin100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin50)
                            {
                                TimeSpan tsHoraFin50 = this.tsHoraFin50;
                                dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                                TimeSpan timeSpan2 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                timeSpan1 = tsHoraFin50 - timeSpan2;
                                num9 = timeSpan1.TotalMinutes;
                                goto label_16;
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin50)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraFin100)
                            {
                                dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                                timeSpan1 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) - this.tsHoraFin100;
                                num9 = timeSpan1.TotalMinutes;
                                goto label_16;
                            }
                        }
                        num9 = 0.0;
                    label_16:
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                        double num10;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraFin100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin50)
                            {
                                dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                                TimeSpan timeSpan2 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                                TimeSpan timeSpan3 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                if (!(timeSpan2 < timeSpan3))
                                {
                                    timeSpan1 = c.FECHA_HORA_EXTRA_FIN_SALIDA - c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                                    num10 = timeSpan1.TotalMinutes;
                                    goto label_28;
                                }
                                else
                                {
                                    TimeSpan tsHoraFin50 = this.tsHoraFin50;
                                    dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                                    TimeSpan timeSpan4 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                    timeSpan1 = tsHoraFin50 - timeSpan4;
                                    num10 = timeSpan1.TotalMinutes;
                                    goto label_28;
                                }
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraFin100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin50)
                            {
                                TimeSpan tsHoraFin50 = this.tsHoraFin50;
                                dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                                TimeSpan timeSpan2 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                timeSpan1 = tsHoraFin50 - timeSpan2;
                                num10 = timeSpan1.TotalMinutes;
                                goto label_28;
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin50)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraFin100)
                            {
                                dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                                timeSpan1 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) - this.tsHoraFin100;
                                num10 = timeSpan1.TotalMinutes;
                                goto label_28;
                            }
                        }
                        num10 = 0.0;
                    label_28:
                        num8 = num9 + num10;
                    }
                    double num11;
                    if (c.FALTA_PARCIAL || c.TOTAL_ASISTIDO <= (double)this.intDuracionJornada)
                    {
                        num11 = 0.0;
                    }
                    else
                    {
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                        double num9;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraInicio100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin100)
                            {
                                timeSpan1 = c.FECHA_HORA_EXTRA_FIN_ENTRADA - c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                                num9 = timeSpan1.TotalMinutes;
                                goto label_41;
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraInicio100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin100)
                            {
                                TimeSpan tsHoraFin100 = this.tsHoraFin100;
                                dateTime = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                                TimeSpan timeSpan2 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                timeSpan1 = tsHoraFin100 - timeSpan2;
                                num9 = timeSpan1.TotalMinutes;
                                goto label_41;
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraInicio100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin100)
                            {
                                dateTime = c.FECHA_HORA_EXTRA_FIN_ENTRADA;
                                timeSpan1 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) - this.tsHoraInicio100;
                                num9 = timeSpan1.TotalMinutes;
                                goto label_41;
                            }
                        }
                        num9 = 0.0;
                    label_41:
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                        double num10;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraInicio100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin100)
                            {
                                if (!(c.FECHA_HORA_EXTRA_FIN_SALIDA > c.FECHA_HORA_EXTRA_INICIO_SALIDA))
                                {
                                    timeSpan1 = c.FECHA_HORA_EXTRA_FIN_SALIDA - c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                                    num10 = timeSpan1.TotalMinutes;
                                    goto label_53;
                                }
                                else
                                {
                                    dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                                    timeSpan1 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) - this.tsHoraInicio100;
                                    num10 = timeSpan1.TotalMinutes;
                                    goto label_53;
                                }
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraInicio100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin100)
                            {
                                TimeSpan tsHoraFin100 = this.tsHoraFin100;
                                dateTime = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                                TimeSpan timeSpan2 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                                timeSpan1 = tsHoraFin100 - timeSpan2;
                                num10 = timeSpan1.TotalMinutes;
                                goto label_53;
                            }
                        }
                        dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                        if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) >= this.tsHoraInicio100)
                        {
                            dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                            if (TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) <= this.tsHoraFin100)
                            {
                                dateTime = c.FECHA_HORA_EXTRA_FIN_SALIDA;
                                timeSpan1 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss")) - this.tsHoraInicio100;
                                num10 = timeSpan1.TotalMinutes;
                                goto label_53;
                            }
                        }
                        num10 = 0.0;
                    label_53:
                        num11 = num9 + num10;
                    }
                    DateTime extraInicioEntrada = c.FECHA_HORA_EXTRA_INICIO_ENTRADA;
                    DateTime fechaHoraEntrada2 = c.FECHA_HORA_ENTRADA;
                    DateTime extraInicioSalida = c.FECHA_HORA_EXTRA_INICIO_SALIDA;
                    DateTime marcacionSalida2 = c.MARCACION_SALIDA;
                    double totalAsistido = c.TOTAL_ASISTIDO;
                    return new
                    {
                        ID_EMPLEADO = idEmpleado,
                        EMPLEADO = empleado,
                        ID_DEPARTAMENTO = idDepartamento,
                        DEPARTAMENTO = departamento,
                        ID_TURNO = idTurno,
                        TURNO = turno,
                        TIPO = tipo,
                        ID_HORARIO = idHorario,
                        HORARIO = horario,
                        DIA = dia,
                        MARCACION_ENTRADA = marcacionEntrada,
                        FECHA_HORA_INICIO_ENTRADA = horaInicioEntrada,
                        FECHA_HORA_ENTRADA = fechaHoraEntrada1,
                        FECHA_HORA_FIN_ENTRADA = fechaHoraFinEntrada,
                        MINS_ENTRADA_TARDE = minsEntradaTarde,
                        ENTRADA_TARDE = entradaTarde,
                        ENTRADA_TEMPRANO = entradaTemprano,
                        MARCACION_SALIDA = marcacionSalida1,
                        FECHA_HORA_INICIO_SALIDA = horaInicioSalida,
                        FECHA_HORA_SALIDA = fechaHoraSalida,
                        FECHA_HORA_FIN_SALIDA = fechaHoraFinSalida,
                        MINS_SALIDA_TEMPRANO = minsSalidaTemprano,
                        SALIDA_TEMPRANO = salidaTemprano,
                        SALIDA_TARDE = salidaTarde,
                        MINS_LUNCH = minsLunch,
                        MARCACION_ENTRADA_LUNCH = marcacionEntradaLunch,
                        MARCACION_SALIDA_LUNCH = marcacionSalidaLunch,
                        TOTAL_LUNCH = num1,
                        JORNADA = jornada,
                        JORNADA_NORMAL = num2,
                        JORNADA_NOCTURNA = num3,
                        HORA_EXTRA = num4 != 0,
                        FERIADO = num5 != 0,
                        FALTA = num6 != 0,
                        PERMISO_VACACION = num7 != 0,
                        HORA_EXTRA_50 = num8,
                        HORA_EXTRA_100 = num11,
                        FECHA_HORA_EXTRA_INICIO_ENTRADA = extraInicioEntrada,
                        FECHA_HORA_EXTRA_FIN_ENTRADA = fechaHoraEntrada2,
                        FECHA_HORA_EXTRA_INICIO_SALIDA = extraInicioSalida,
                        FECHA_HORA_EXTRA_FIN_SALIDA = marcacionSalida2,
                        TOTAL_ASISTIDO = totalAsistido
                    };
                }).Select(g => new
                {
                    ID_EMPLEADO = g.ID_EMPLEADO,
                    EMPLEADO = g.EMPLEADO,
                    ID_DEPARTAMENTO = g.ID_DEPARTAMENTO,
                    DEPARTAMENTO = g.DEPARTAMENTO,
                    ID_TURNO = g.ID_TURNO,
                    TURNO = g.TURNO,
                    TIPO = g.TIPO,
                    ID_HORARIO = g.ID_HORARIO,
                    HORARIO = g.HORARIO,
                    DIA = g.DIA,
                    MARCACION_ENTRADA = g.MARCACION_ENTRADA,
                    FECHA_HORA_INICIO_ENTRADA = g.FECHA_HORA_INICIO_ENTRADA,
                    FECHA_HORA_ENTRADA = g.FECHA_HORA_ENTRADA,
                    FECHA_HORA_FIN_ENTRADA = g.FECHA_HORA_FIN_ENTRADA,
                    ENTRADA_TARDE = g.ENTRADA_TARDE,
                    ENTRADA_TEMPRANO = g.ENTRADA_TEMPRANO,
                    MARCACION_SALIDA = g.MARCACION_SALIDA,
                    FECHA_HORA_INICIO_SALIDA = g.FECHA_HORA_INICIO_SALIDA,
                    FECHA_HORA_SALIDA = g.FECHA_HORA_SALIDA,
                    FECHA_HORA_FIN_SALIDA = g.FECHA_HORA_FIN_SALIDA,
                    SALIDA_TEMPRANO = g.SALIDA_TEMPRANO,
                    SALIDA_TARDE = g.SALIDA_TARDE,
                    LUNCH = g.MINS_LUNCH,
                    MARCACION_ENTRADA_LUNCH = g.MARCACION_ENTRADA_LUNCH,
                    MARCACION_SALIDA_LUNCH = g.MARCACION_SALIDA_LUNCH,
                    SOBRETIEMPO_LUNCH = g.TOTAL_LUNCH > g.MINS_LUNCH,
                    DESCUENTO_LUNCH = g.TOTAL_LUNCH - g.MINS_LUNCH,
                    TOTAL_LUNCH = g.TOTAL_LUNCH,
                    JORNADA_NORMAL = g.HORA_EXTRA ? 0.0 : g.JORNADA_NORMAL,
                    JORNADA_NOCTURNA = g.HORA_EXTRA ? 0.0 : g.JORNADA_NOCTURNA,
                    FERIADO = g.FERIADO,
                    FALTA = g.FALTA,
                    PERMISO_VACACION = g.PERMISO_VACACION,
                    HORA_EXTRA_50 = g.HORA_EXTRA ? 0.0 : g.HORA_EXTRA_50,
                    HORA_EXTRA_100 = g.HORA_EXTRA ? g.JORNADA_NORMAL + g.JORNADA_NOCTURNA + g.HORA_EXTRA_50 + g.HORA_EXTRA_100 : g.HORA_EXTRA_100,
                    TOTAL_JORNADA = (g.HORA_EXTRA ? 0.0 : g.JORNADA_NORMAL) + (g.HORA_EXTRA ? 0.0 : g.JORNADA_NOCTURNA),
                    TOTAL_HORA_EXTRA = g.HORA_EXTRA_50 + (g.HORA_EXTRA ? g.JORNADA_NORMAL + g.JORNADA_NOCTURNA + g.HORA_EXTRA_100 : g.HORA_EXTRA_100),
                    TOTAL_ASISTIDO = g.TOTAL_ASISTIDO
                }).Select(g => new
                {
                    ID_EMPLEADO = g.ID_EMPLEADO,
                    EMPLEADO = g.EMPLEADO,
                    ID_DEPARTAMENTO = g.ID_DEPARTAMENTO,
                    DEPARTAMENTO = g.DEPARTAMENTO,
                    ID_TURNO = g.ID_TURNO,
                    TURNO = g.TURNO,
                    TIPO = g.TIPO,
                    ID_HORARIO = g.ID_HORARIO,
                    HORARIO = g.HORARIO,
                    DIA = g.DIA,
                    MARCACION_ENTRADA = g.MARCACION_ENTRADA,
                    FECHA_HORA_INICIO_ENTRADA = g.FECHA_HORA_INICIO_ENTRADA,
                    FECHA_HORA_ENTRADA = g.FECHA_HORA_ENTRADA,
                    FECHA_HORA_FIN_ENTRADA = g.FECHA_HORA_FIN_ENTRADA,
                    ENTRADA_TARDE = g.ENTRADA_TARDE,
                    ENTRADA_TEMPRANO = g.ENTRADA_TEMPRANO,
                    MARCACION_SALIDA = g.MARCACION_SALIDA,
                    FECHA_HORA_INICIO_SALIDA = g.FECHA_HORA_INICIO_SALIDA,
                    FECHA_HORA_SALIDA = g.FECHA_HORA_SALIDA,
                    FECHA_HORA_FIN_SALIDA = g.FECHA_HORA_FIN_SALIDA,
                    SALIDA_TEMPRANO = g.SALIDA_TEMPRANO,
                    SALIDA_TARDE = g.SALIDA_TARDE,
                    LUNCH = g.LUNCH,
                    MARCACION_ENTRADA_LUNCH = g.MARCACION_ENTRADA_LUNCH,
                    MARCACION_SALIDA_LUNCH = g.MARCACION_SALIDA_LUNCH,
                    SOBRETIEMPO_LUNCH = g.SOBRETIEMPO_LUNCH,
                    TOTAL_LUNCH = g.TOTAL_LUNCH,
                    JORNADA_NORMAL = g.JORNADA_NORMAL,
                    JORNADA_NOCTURNA = g.JORNADA_NOCTURNA,
                    FERIADO = g.FERIADO,
                    FALTA = g.FALTA,
                    PERMISO_VACACION = g.PERMISO_VACACION,
                    HORA_EXTRA_50 = g.SOBRETIEMPO_LUNCH ? (g.DESCUENTO_LUNCH >= g.HORA_EXTRA_50 ? 0.0 : g.HORA_EXTRA_50 - g.DESCUENTO_LUNCH) : g.HORA_EXTRA_50,
                    HORA_EXTRA_100 = g.SOBRETIEMPO_LUNCH ? (g.DESCUENTO_LUNCH - (g.DESCUENTO_LUNCH >= g.HORA_EXTRA_50 ? g.DESCUENTO_LUNCH - g.HORA_EXTRA_50 : 0.0) >= g.HORA_EXTRA_100 ? 0.0 : g.HORA_EXTRA_100 - (g.DESCUENTO_LUNCH - (g.DESCUENTO_LUNCH >= g.HORA_EXTRA_50 ? g.DESCUENTO_LUNCH - g.HORA_EXTRA_50 : g.DESCUENTO_LUNCH))) : g.HORA_EXTRA_100,
                    TOTAL_JORNADA = g.TOTAL_JORNADA,
                    TOTAL_HORA_EXTRA = g.SOBRETIEMPO_LUNCH ? (g.DESCUENTO_LUNCH >= g.TOTAL_HORA_EXTRA ? 0.0 : g.TOTAL_HORA_EXTRA - g.DESCUENTO_LUNCH) : g.TOTAL_HORA_EXTRA,
                    TOTAL_ASISTIDO = g.TOTAL_ASISTIDO
                });
                //using (lbReporte lbReporte = new lbReporte())
                //{
                //    List<EntReporte> _entReporte = new List<EntReporte>();
                //    foreach (var data in datas)
                //    {
                //        using (EntReporte entReporte1 = new EntReporte())
                //        {
                //            entReporte1.EMPLEADO = data.ID_EMPLEADO.ToString() + " - " + data.EMPLEADO;
                //            entReporte1.DEPARTAMENTO = data.ID_DEPARTAMENTO.ToString() + " - " + data.DEPARTAMENTO;
                //            EntReporte entReporte2 = entReporte1;
                //            string str;
                //            if (!(data.TIPO == "F"))
                //                str = data.TURNO;
                //            else
                //                str = data.ID_TURNO.ToString() + " - " + data.TURNO;
                //            entReporte2.TURNO = str;
                //            entReporte1.HORARIO = data.ID_HORARIO.ToString() + " - " + data.HORARIO;
                //            entReporte1.DIA = data.DIA;
                //            entReporte1.MARCACION_ENTRADA = data.MARCACION_ENTRADA;
                //            entReporte1.FECHA_HORA_INICIO_ENTRADA = data.FECHA_HORA_INICIO_ENTRADA;
                //            entReporte1.FECHA_HORA_ENTRADA = data.FECHA_HORA_ENTRADA;
                //            entReporte1.FECHA_HORA_FIN_ENTRADA = data.FECHA_HORA_FIN_ENTRADA;
                //            entReporte1.ENTRADA_TARDE = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.ENTRADA_TARDE / 60.0, 0)) : TimeSpan.FromMinutes(data.ENTRADA_TARDE);
                //            entReporte1.ENTRADA_TEMPRANO = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.ENTRADA_TEMPRANO / 60.0, 0)) : TimeSpan.FromMinutes(data.ENTRADA_TEMPRANO);
                //            entReporte1.MARCACION_SALIDA = data.MARCACION_SALIDA;
                //            entReporte1.FECHA_HORA_INICIO_SALIDA = data.FECHA_HORA_INICIO_SALIDA;
                //            entReporte1.FECHA_HORA_SALIDA = data.FECHA_HORA_SALIDA;
                //            entReporte1.FECHA_HORA_FIN_SALIDA = data.FECHA_HORA_FIN_SALIDA;
                //            entReporte1.SALIDA_TEMPRANO = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.SALIDA_TEMPRANO / 60.0, 0)) : TimeSpan.FromMinutes(data.SALIDA_TEMPRANO);
                //            entReporte1.SALIDA_TARDE = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.SALIDA_TARDE / 60.0, 0)) : TimeSpan.FromMinutes(data.SALIDA_TARDE);
                //            entReporte1.LUNCH = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.LUNCH / 60.0, 0)) : TimeSpan.FromMinutes(data.LUNCH);
                //            entReporte1.MARCACION_ENTRADA_LUNCH = data.MARCACION_ENTRADA_LUNCH;
                //            entReporte1.MARCACION_SALIDA_LUNCH = data.MARCACION_SALIDA_LUNCH;
                //            entReporte1.SOBRETIEMPO_LUNCH = data.SOBRETIEMPO_LUNCH;
                //            entReporte1.TOTAL_LUNCH = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.TOTAL_LUNCH / 60.0, 0)) : TimeSpan.FromMinutes(data.TOTAL_LUNCH);
                //            entReporte1.JORNADA_NORMAL = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.JORNADA_NORMAL / 60.0, 0)) : TimeSpan.FromMinutes(data.JORNADA_NORMAL);
                //            entReporte1.JORNADA_NOCTURNA = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.JORNADA_NOCTURNA / 60.0, 0)) : TimeSpan.FromMinutes(data.JORNADA_NOCTURNA);
                //            entReporte1.FERIADO = data.FERIADO;
                //            entReporte1.FALTA = data.FALTA;
                //            entReporte1.PERMISO_VACACION = data.PERMISO_VACACION;
                //            entReporte1.HORA_EXTRA_50 = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.HORA_EXTRA_50 / 60.0, 0)) : TimeSpan.FromMinutes(data.HORA_EXTRA_50);
                //            entReporte1.HORA_EXTRA_100 = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.HORA_EXTRA_100 / 60.0, 0)) : TimeSpan.FromMinutes(data.HORA_EXTRA_100);
                //            entReporte1.TOTAL_JORNADA = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.TOTAL_JORNADA / 60.0, 0)) : TimeSpan.FromMinutes(data.TOTAL_JORNADA);
                //            entReporte1.TOTAL_HORA_EXTRA = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.TOTAL_HORA_EXTRA / 60.0, 0)) : TimeSpan.FromMinutes(data.TOTAL_HORA_EXTRA);
                //            entReporte1.TOTAL_ASISTIDO = this.strUnidad == "H" ? TimeSpan.FromHours(Math.Round(data.TOTAL_ASISTIDO / 60.0, 0)) : TimeSpan.FromMinutes(data.TOTAL_ASISTIDO);
                //            _entReporte.Add(entReporte1);
                //        }
                //    }
                //    dataSet = lbReporte.generarReporte(_entReporte, entEmpleadoList, _entDepartamento, _entMarcacion);
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {

        }
    }
}
