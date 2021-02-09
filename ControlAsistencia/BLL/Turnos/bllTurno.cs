using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Turnos;
using DTO.Turnos;
using DTO.Empleados;
using DTO.Marcaciones;

namespace BLL.Turnos
{
    public class bllTurno : IDisposable
    {
        private string strConn { get; set; }

        public bllTurno(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoHorario> execQueryHorario()
        {
            try
            {
                using (dalTurno dal = new dalTurno(this.strConn))
                    return dal.execQueryHorario("select * from schclass ORDER BY SCHCLASSID;");
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<dtoTurnoCabecera> execQueryTurnoCabecera()
        {
            try
            {
                using (dalTurno dal = new dalTurno(this.strConn))
                    return dal.execQueryTurnoCabecera("select * from NUM_RUN ORDER BY NUM_RUNID;");
            }
            catch (Exception)
            {
                throw ;
            }
        }

        private List<dtoTurnoDetalle> execQueryTurnoDetalle()
        {
            try
            {
                using (dalTurno dal = new dalTurno(this.strConn))
                    return dal.execQueryTurnoDetalle("select * from NUM_RUN_DEIL ORDER BY NUM_RUNID;");
            }
            catch (Exception )
            {
                throw ;
            }
        }

        private List<dtoTurnoFijo> execQueryTurnoFijo(DateTime _dtFechaInicio, DateTime _dtFechaFin, int[] _intIdEmpleado)
        {
            try
            {
                using (dalTurno dal = new dalTurno(this.strConn))
                    return dal.execQueryTurnoFijo("SELECT * FROM USER_OF_RUN ORDER BY USERID;")
                        .Where(et => (_dtFechaInicio >= et.STARTDATE
                                            || _dtFechaInicio <= et.ENDDATE
                                            || (_dtFechaFin >= et.STARTDATE || _dtFechaFin <= et.ENDDATE)
                                        )
                                        && _intIdEmpleado.Contains(et.USERID))
                        .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<dtoTurnoTemporal> execQueryTurnoTemporal(int[] _intIdEmpleado)
        {
            try
            {
                using (dalTurno dal = new dalTurno(this.strConn))
                    return dal.execQueryTurnoTemporal("select * from USER_TEMP_SCH ORDER BY USERID;")
                        .Where(tt => _intIdEmpleado.Contains(tt.USERID))
                        .ToList();
            }
            catch (Exception)
            {
                throw ;
            }
        }

        private List<dtoTurnoRotativo> execQueryTurnoRotativo(int[] _intIdEmpleado)
        {
            try
            {
                using (dalTurno dal = new dalTurno(this.strConn))
                    return dal.execQueryTurnoRotativo("select * from UserUsedSClasses ORDER BY UserId;")
                        .Where(tr => _intIdEmpleado.Contains(tr.UserId))
                        .OrderBy(tr => tr.UserId)
                        .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<dtoTurnoRotativo> validarTurnoRotativo(
         List<dtoTurnoRotativo> _dtosTurnoRotativo,
         List<dtoHorario> _dtosHorario,
         List<dtoMarcacion> _dtosMarcacion,
         List<dtoEmpleado> _dtosEmpleado)
        {
            List<dtoTurnoRotativo> dtos = new List<dtoTurnoRotativo>();
            try
            {
                foreach (var data in _dtosTurnoRotativo
                        .Join(_dtosHorario, (tr => tr.SchId), (h => h.SCHCLASSID), (tr, h) =>
                        {
                            int userId = tr.UserId;
                            int schId = tr.SchId;
                            DateTime cometime = tr.COMETIME;
                            DateTime leavetime = tr.LEAVETIME;
                            string name = tr.NAME;
                            int num1 = tr.OVERTIME ? 1 : 0;
                            DateTime dateTime1;
                            DateTime dateTime2;
                            if (!(h.CHECKINTIME1 <= h.STARTTIME))
                            {
                                dateTime1 = tr.COMETIME;
                                dateTime1 = dateTime1.AddDays(-1.0);
                                dateTime2 = dateTime1.Add(h.CHECKINTIME1);
                            }
                            else
                            {
                                dateTime1 = tr.COMETIME;
                                dateTime2 = dateTime1.Add(h.CHECKINTIME1);
                            }
                            dateTime1 = tr.COMETIME;
                            DateTime dateTime3 = dateTime1.Add(h.STARTTIME);
                            DateTime dateTime4;
                            if (!(h.CHECKINTIME2 >= h.STARTTIME))
                            {
                                dateTime1 = tr.COMETIME;
                                dateTime1 = dateTime1.AddDays(1.0);
                                dateTime4 = dateTime1.Add(h.CHECKINTIME2);
                            }
                            else
                            {
                                dateTime1 = tr.COMETIME;
                                dateTime4 = dateTime1.Add(h.CHECKINTIME2);
                            }
                            int lateminutes = h.LATEMINUTES;
                            int num2 = h.CHECKIN ? 1 : 0;
                            TimeSpan endtime = h.ENDTIME;
                            TimeSpan checkouttimE1 = h.CHECKOUTTIME1;
                            DateTime dateTime5;
                            if (!(h.ENDTIME <= h.STARTTIME))
                            {
                                dateTime1 = tr.LEAVETIME;
                                dateTime5 = dateTime1.Add(h.ENDTIME);
                            }
                            else
                            {
                                dateTime1 = tr.LEAVETIME;
                                dateTime1 = dateTime1.Add(h.ENDTIME);
                                dateTime5 = dateTime1.AddDays(1.0);
                            }
                            TimeSpan checkouttimE2 = h.CHECKOUTTIME2;
                            int earlyminutes = h.EARLYMINUTES;
                            int num3 = h.CHECKOUT ? 1 : 0;
                            return new
                            {
                                ID_EMPLEADO = userId,
                                ID_HORARIO = schId,
                                FECHA_ENTRADA = cometime,
                                FECHA_SALIDA = leavetime,
                                TURNO = name,
                                HORA_EXTRA = num1 != 0,
                                FECHA_HORA_INICIO_ENTRADA = dateTime2,
                                FECHA_HORA_ENTRADA = dateTime3,
                                FECHA_HORA_FIN_ENTRADA = dateTime4,
                                MINS_ENTRADA_TARDE = lateminutes,
                                CONTROL_ENTRADA_H = num2 != 0,
                                ENDTIME = endtime,
                                CHECKOUTTIME1 = checkouttimE1,
                                FECHA_HORA_SALIDA = dateTime5,
                                CHECKOUTTIME2 = checkouttimE2,
                                MINS_SALIDA_TEMPRANO = earlyminutes,
                                CONTROL_SALIDA_H = num3 != 0
                            };
                        })
                .Select(tr =>
                {
                    int idEmpleado = tr.ID_EMPLEADO;
                    int idHorario = tr.ID_HORARIO;
                    DateTime fechaEntrada = tr.FECHA_ENTRADA;
                    DateTime fechaSalida = tr.FECHA_SALIDA;
                    string turno = tr.TURNO;
                    int num1 = tr.HORA_EXTRA ? 1 : 0;
                    DateTime horaInicioEntrada = tr.FECHA_HORA_INICIO_ENTRADA;
                    DateTime fechaHoraEntrada = tr.FECHA_HORA_ENTRADA;
                    DateTime fechaHoraFinEntrada = tr.FECHA_HORA_FIN_ENTRADA;
                    int minsEntradaTarde = tr.MINS_ENTRADA_TARDE;
                    int num2 = tr.CONTROL_ENTRADA_H ? 1 : 0;
                    DateTime dateTime1;
                    DateTime dateTime2;
                    if (!(tr.CHECKOUTTIME1 <= tr.ENDTIME))
                    {
                        dateTime1 = tr.FECHA_HORA_SALIDA;
                        dateTime1 = DateTime.Parse(dateTime1.ToString("yyy-MM-dd"));
                        dateTime1 = dateTime1.AddDays(-1.0);
                        dateTime2 = dateTime1.Add(tr.CHECKOUTTIME1);
                    }
                    else
                    {
                        dateTime1 = tr.FECHA_HORA_SALIDA;
                        dateTime1 = DateTime.Parse(dateTime1.ToString("yyy-MM-dd"));
                        dateTime2 = dateTime1.Add(tr.CHECKOUTTIME1);
                    }
                    DateTime fechaHoraSalida = tr.FECHA_HORA_SALIDA;
                    DateTime dateTime3;
                    if (!(tr.CHECKOUTTIME2 >= tr.ENDTIME))
                    {
                        dateTime1 = tr.FECHA_HORA_SALIDA;
                        dateTime1 = DateTime.Parse(dateTime1.ToString("yyy-MM-dd"));
                        dateTime1 = dateTime1.AddDays(1.0);
                        dateTime3 = dateTime1.Add(tr.CHECKOUTTIME2);
                    }
                    else
                    {
                        dateTime1 = tr.FECHA_HORA_SALIDA;
                        dateTime1 = DateTime.Parse(dateTime1.ToString("yyy-MM-dd"));
                        dateTime3 = dateTime1.Add(tr.CHECKOUTTIME2);
                    }
                    int minsSalidaTemprano = tr.MINS_SALIDA_TEMPRANO;
                    int num3 = tr.CONTROL_SALIDA_H ? 1 : 0;
                    return new
                    {
                        ID_EMPLEADO = idEmpleado,
                        ID_HORARIO = idHorario,
                        FECHA_ENTRADA = fechaEntrada,
                        FECHA_SALIDA = fechaSalida,
                        TURNO = turno,
                        HORA_EXTRA = num1 != 0,
                        FECHA_HORA_INICIO_ENTRADA = horaInicioEntrada,
                        FECHA_HORA_ENTRADA = fechaHoraEntrada,
                        FECHA_HORA_FIN_ENTRADA = fechaHoraFinEntrada,
                        MINS_ENTRADA_TARDE = minsEntradaTarde,
                        CONTROL_ENTRADA_H = num2 != 0,
                        FECHA_HORA_INICIO_SALIDA = dateTime2,
                        FECHA_HORA_SALIDA = fechaHoraSalida,
                        FECHA_HORA_FIN_SALIDA = dateTime3,
                        MINS_SALIDA_TEMPRANO = minsSalidaTemprano,
                        CONTROL_SALIDA_H = num3 != 0
                    };
                }).Join(_dtosEmpleado, th => th.ID_EMPLEADO, (e => e.USERID), (th, e) => new
                {
                    ID_EMPLEADO = th.ID_EMPLEADO,
                    ID_HORARIO = th.ID_HORARIO,
                    FECHA_ENTRADA = th.FECHA_ENTRADA,
                    FECHA_SALIDA = th.FECHA_SALIDA,
                    TURNO = th.TURNO,
                    HORA_EXTRA = th.HORA_EXTRA,
                    MARCACION_ENTRADA = e.INLATE == 0
                        ? (from m in _dtosMarcacion
                           where m.CHECKTIME >= th.FECHA_HORA_INICIO_ENTRADA
                             && m.CHECKTIME <= th.FECHA_HORA_FIN_ENTRADA
                             && m.USERID == e.USERID
                           select m.CHECKTIME).DefaultIfEmpty().Min()
                        : (e.INLATE == 1 ? _dtosMarcacion
                            .Where(m => m.CHECKTIME >= th.FECHA_HORA_INICIO_ENTRADA
                            && m.CHECKTIME <= th.FECHA_HORA_FIN_ENTRADA
                            && (m.CHECKTYPE == "I" && th.CONTROL_ENTRADA_H || !th.CONTROL_ENTRADA_H)
                            && m.USERID == e.USERID)
                            .Select(m => m.CHECKTIME).DefaultIfEmpty().Min() : DateTime.MinValue),
                    MARCACION_SALIDA = e.OUTEARLY == 0
                        ? _dtosMarcacion.Where(m => m.CHECKTIME >= th.FECHA_HORA_INICIO_SALIDA
                            && m.CHECKTIME <= th.FECHA_HORA_FIN_SALIDA
                            && m.USERID == e.USERID)
                            .Select(m => m.CHECKTIME).DefaultIfEmpty().Max()
                        : (e.OUTEARLY == 1
                            ? _dtosMarcacion.Where(m => m.CHECKTIME >= th.FECHA_HORA_INICIO_SALIDA
                                && m.CHECKTIME <= th.FECHA_HORA_FIN_SALIDA
                                && (m.CHECKTYPE == "O" && th.CONTROL_SALIDA_H || !th.CONTROL_SALIDA_H)
                                && m.USERID == e.USERID)
                                .Select(m => m.CHECKTIME).DefaultIfEmpty().Max() : DateTime.MinValue)
                }))
                {
                    if (data.MARCACION_ENTRADA != DateTime.MinValue && data.MARCACION_SALIDA != DateTime.MinValue)
                    {
                        dtos.Add(new dtoTurnoRotativo()
                        {
                            UserId = data.ID_EMPLEADO,
                            _COMETIME = data.FECHA_ENTRADA,
                            _LEAVETIME = data.FECHA_SALIDA,
                            SchId = data.ID_HORARIO
                        });
                    }
                }
            }
            catch (Exception )
            {
                throw;
            }
            return dtos;
        }
        private List<dtoTurnoHorario> procesarFechasTurno(
         List<dtoTurnoCabecera> _dtosTurnoCabecera,
         List<dtoTurnoDetalle> _dtosTurnoDetalle,
         List<dtoTurnoTemporal> _dtosTurnoTemporal,
         List<dtoTurnoRotativo> _dtosTurnoRotativo,
         List<dtoHorario> _dtosHorario,
         List<dtoMarcacion> _dtosMarcacion,
         List<dtoEmpleado> _dtosEmpleado,
         DateTime _dtFechaInicio,
         DateTime _dtFechaFin)
        {
            List<dtoTurnoHorario> dtos = new List<dtoTurnoHorario>();
            try
            {
                List<dtoFecha> _dtoFechas = new List<dtoFecha>();
                for (DateTime dt = _dtFechaInicio; dt <= _dtFechaFin; dt = dt.AddDays(1.0))
                {
                    _dtoFechas.Add(new dtoFecha()
                    {
                        FECHA = dt
                    });
                }
                List<dtoTurnoRotativo> dto = new List<dtoTurnoRotativo>();
                foreach (var data in from tr in _dtosTurnoRotativo
                                     from f in _dtoFechas
                                     select new
                                     {
                                        ID_EMPLEADO = tr.UserId,
                                        FECHA_ENTRADA = f.FECHA,
                                        FECHA_SALIDA = f.FECHA,
                                        ID_HORARIO = tr.SchId
                                     })
                {
                    dto.Add(new dtoTurnoRotativo()
                    {
                        UserId = data.ID_EMPLEADO,
                        _COMETIME = data.FECHA_ENTRADA,
                        _LEAVETIME = data.FECHA_SALIDA,
                        SchId = data.ID_HORARIO
                    });

                }
                foreach (dtoTurnoRotativo tr in this.validarTurnoRotativo(dto, _dtosHorario, _dtosMarcacion, _dtosEmpleado))
                {
                    dtos.Add(new dtoTurnoHorario()
                    {
                        ID_EMPLEADO = tr.UserId,
                        ID_TURNO = 0,
                        TURNO = tr.NAME,
                        FECHA_HORA_ENTRADA = tr.COMETIME,
                        FECHA_HORA_SALIDA = tr.LEAVETIME,
                        ID_HORARIO = tr.SchId,
                        TIPO = "R",
                        HORA_EXTRA = tr.OVERTIME
                    });
                }
                foreach (var data in from tt in _dtosTurnoTemporal
                                     where _dtFechaInicio >= tt.COMETIME
                                            || _dtFechaInicio <= tt.LEAVETIME
                                            || _dtFechaFin >= tt.COMETIME
                                            || _dtFechaFin <= tt.LEAVETIME
                                     select new
                                     {
                                         ID_EMPLEADO = tt.USERID,
                                         ID_TURNO = 0,
                                         TURNO = tt.NAME,
                                         FECHA_INICIO = tt.COMETIME,
                                         FECHA_FIN = tt.LEAVETIME,
                                         ID_HORARIO = tt.SCHCLASSID,
                                         HORA_EXTRA = tt.OVERTIME
                                     })
                {
                    dtos.Add(new dtoTurnoHorario()
                    {
                        ID_EMPLEADO = data.ID_EMPLEADO,
                        ID_TURNO = 0,
                        TURNO = data.TURNO,
                        FECHA_HORA_ENTRADA = data.FECHA_INICIO,
                        FECHA_HORA_SALIDA = data.FECHA_FIN,
                        ID_HORARIO = data.ID_HORARIO,
                        TIPO = "T",
                        HORA_EXTRA = data.HORA_EXTRA
                    });
                }
                foreach (var data in from tc in _dtosTurnoCabecera
                                     join td in _dtosTurnoDetalle on tc.NUM_RUNID equals td.NUM_RUNID
                                     where _dtFechaInicio >= tc.STARTDATE
                                            || _dtFechaInicio <= tc.ENDDATE
                                            || _dtFechaFin >= tc.STARTDATE
                                            || _dtFechaFin <= tc.ENDDATE
                                     select new
                                     {
                                         ID_TURNO = tc.NUM_RUNID,
                                         TURNO = tc.NAME,
                                         CICLO = tc.CYLE,
                                         UNIDAD = tc.UNITS,
                                         DIA_INICIO = td.SDAYS,
                                         DIA_FIN = td.EDAYS,
                                         ID_HORARIO = td.SCHCLASSID,
                                         HORA_EXTRA = td.Overtime
                                     })
                {
                    foreach (dtoFecha f in _dtoFechas)
                    {
                        using (dtoTurnoHorario th = new dtoTurnoHorario())
                        {
                            DateTime now1 = DateTime.Now;
                            DateTime now2 = DateTime.Now;
                            string unidad = data.UNIDAD;
                            if (!(unidad == "DIA"))
                            {
                                if (!(unidad == "SEMANA"))
                                {
                                    if (unidad == "MES")
                                    {
                                        if (f.DIA_MES == data.DIA_INICIO + 1)
                                        {
                                            DateTime fecha = f.FECHA;
                                            DateTime dateTime = data.DIA_INICIO != data.DIA_FIN
                                                                ? f.FECHA.AddDays(1.0)
                                                                : f.FECHA;
                                            th.ID_TURNO = data.ID_TURNO;
                                            th.TURNO = data.TURNO;
                                            th.FECHA_HORA_ENTRADA = fecha;
                                            th.FECHA_HORA_SALIDA = dateTime;
                                            th.ID_HORARIO = data.ID_HORARIO;
                                            th.TIPO = "F";
                                            th.HORA_EXTRA = data.HORA_EXTRA;
                                            dtos.Add(th);
                                        }
                                    }
                                }
                                else if (f.DIA_SEMANA == data.DIA_INICIO)
                                {
                                    DateTime fecha = f.FECHA;
                                    DateTime dateTime = data.DIA_INICIO != data.DIA_FIN
                                                                            ? f.FECHA.AddDays(1.0)
                                                                            : f.FECHA;
                                    th.ID_TURNO = data.ID_TURNO;
                                    th.TURNO = data.TURNO;
                                    th.FECHA_HORA_ENTRADA = fecha;
                                    th.FECHA_HORA_SALIDA = dateTime;
                                    th.ID_HORARIO = data.ID_HORARIO;
                                    th.TIPO = "F";
                                    th.HORA_EXTRA = data.HORA_EXTRA;
                                    dtos.Add(th);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception )
            {
                throw;
            }
            return dtos;
        }

        public List<dtoTurnoHorario> procesarTipoTurno(
          List<dtoMarcacion> _dtosMarcacion,
          List<dtoEmpleado> _dtosEmpleado,
          int[] _intIdEmpleado,
          List<dtoHorario> _dtosHorario,
          DateTime _dtFechaInicio,
          DateTime _dtFechaFin)
        {
            List<dtoTurnoHorario> dtos = new List<dtoTurnoHorario>();
            try
            {
                List<dtoTurnoCabecera> dtosTurnoCabecera = this.execQueryTurnoCabecera();

                List<dtoTurnoDetalle> dtosTurnoDetalle = this.execQueryTurnoDetalle();

                List<dtoTurnoFijo> dtosTurnoFijo = this.execQueryTurnoFijo(_dtFechaInicio, _dtFechaFin, _intIdEmpleado);

                List<dtoTurnoRotativo> dtosTurnoRotativo = this.execQueryTurnoRotativo(_intIdEmpleado);

                List<dtoTurnoTemporal> dtosTurnoTemporal = this.execQueryTurnoTemporal(_intIdEmpleado);

                List<dtoTurnoHorario> dtosTurnoHorario = this.procesarFechasTurno(dtosTurnoCabecera, dtosTurnoDetalle, dtosTurnoTemporal, dtosTurnoRotativo, _dtosHorario, _dtosMarcacion, _dtosEmpleado, _dtFechaInicio, _dtFechaFin);

                List<dtoTurnoHorario> tmp_turnoTemporal = (from ft in dtosTurnoHorario
                                                           where ft.TIPO == "T"
                                                           select new dtoTurnoHorario()
                                                           {
                                                               ID_EMPLEADO = ft.ID_EMPLEADO,
                                                               ID_TURNO = 0,
                                                               TURNO = ft.TURNO,
                                                               ID_HORARIO = ft.ID_HORARIO,
                                                               FECHA_HORA_ENTRADA = ft.FECHA_HORA_ENTRADA,
                                                               FECHA_HORA_SALIDA = ft.FECHA_HORA_SALIDA,
                                                               TIPO = ft.TIPO,
                                                               HORA_EXTRA = ft.HORA_EXTRA
                                                           }).ToList();

                var tmp_turnoFijo = from th in dtosTurnoHorario
                                    join tf in dtosTurnoFijo on th.ID_TURNO equals tf.NUM_OF_RUN_ID
                                    where th.TIPO == "F"
                                    select new
                                    {
                                        ID_EMPLEADO = tf.USERID,
                                        ID_TURNO = th.ID_TURNO,
                                        TURNO = th.TURNO,
                                        ID_HORARIO = th.ID_HORARIO,
                                        FECHA_HORA_ENTRADA = th.FECHA_HORA_ENTRADA,
                                        FECHA_HORA_SALIDA = th.FECHA_HORA_SALIDA,
                                        TIPO = th.TIPO,
                                        HORA_EXTRA = th.HORA_EXTRA,
                                        TEMPORAL = (from tt
                                                    in tmp_turnoTemporal
                                                    where ((th.FECHA_HORA_ENTRADA >= tt.FECHA_HORA_ENTRADA && th.FECHA_HORA_ENTRADA <= tt.FECHA_HORA_SALIDA)
                                                           || (th.FECHA_HORA_SALIDA >= tt.FECHA_HORA_ENTRADA && th.FECHA_HORA_SALIDA <= tt.FECHA_HORA_SALIDA)) &&
                                                          tf.USERID == tt.ID_EMPLEADO
                                                    select tt.ID_EMPLEADO).Count() != 0
                                    };

                foreach (var data in from th in dtosTurnoHorario
                                     where th.TIPO == "R"
                                     select new
                                     {
                                         ID_EMPLEADO = th.ID_EMPLEADO,
                                         ID_TURNO = 0,
                                         TURNO = th.TURNO,
                                         ID_HORARIO = th.ID_HORARIO,
                                         FECHA_HORA_ENTRADA = th.FECHA_HORA_ENTRADA,
                                         FECHA_HORA_SALIDA = th.FECHA_HORA_SALIDA,
                                         TIPO = th.TIPO,
                                         HORA_EXTRA = th.HORA_EXTRA,
                                         TEMPORAL = (from tt in tmp_turnoTemporal
                                                     where ((th.FECHA_HORA_ENTRADA >= tt.FECHA_HORA_ENTRADA && th.FECHA_HORA_ENTRADA <= tt.FECHA_HORA_SALIDA)
                                                          ||
                                                          (th.FECHA_HORA_SALIDA >= tt.FECHA_HORA_ENTRADA && th.FECHA_HORA_SALIDA <= tt.FECHA_HORA_SALIDA))
                                                          && th.ID_EMPLEADO == tt.ID_EMPLEADO
                                                     select tt.ID_EMPLEADO).Count() != 0,
                                         FIJO = (from tf in tmp_turnoFijo
                                                 where ((th.FECHA_HORA_ENTRADA >= tf.FECHA_HORA_ENTRADA && th.FECHA_HORA_ENTRADA <= tf.FECHA_HORA_SALIDA)
                                                        ||
                                                        (th.FECHA_HORA_SALIDA >= tf.FECHA_HORA_ENTRADA && th.FECHA_HORA_SALIDA <= tf.FECHA_HORA_SALIDA))
                                                        && th.ID_EMPLEADO == tf.ID_EMPLEADO
                                                 select tf.ID_EMPLEADO).Count() != 0
                                     })
                {
                    var tr = data;
                    if (!tr.TEMPORAL && !tr.FIJO && (from th in dtos
                                                     where ((tr.FECHA_HORA_ENTRADA >= th.FECHA_HORA_ENTRADA && tr.FECHA_HORA_ENTRADA <= th.FECHA_HORA_SALIDA)
                                                             ||
                                                             (tr.FECHA_HORA_SALIDA >= th.FECHA_HORA_ENTRADA && tr.FECHA_HORA_SALIDA <= th.FECHA_HORA_SALIDA)
                                                             && tr.ID_EMPLEADO == th.ID_EMPLEADO)
                                                     select tr.ID_EMPLEADO).Count() == 0)
                    {
                        dtos.Add(new dtoTurnoHorario()
                        {
                            ID_EMPLEADO = tr.ID_EMPLEADO,
                            ID_TURNO = tr.ID_TURNO,
                            TURNO = tr.TURNO,
                            ID_HORARIO = tr.ID_HORARIO,
                            FECHA_HORA_ENTRADA = tr.FECHA_HORA_ENTRADA,
                            FECHA_HORA_SALIDA = tr.FECHA_HORA_SALIDA,
                            TIPO = tr.TIPO,
                            HORA_EXTRA = tr.HORA_EXTRA
                        });
                    }
                }
                foreach (var data in tmp_turnoTemporal)
                {
                    dtos.Add(new dtoTurnoHorario()
                    {
                        ID_EMPLEADO = data.ID_EMPLEADO,
                        ID_TURNO = data.ID_TURNO,
                        TURNO = data.TURNO,
                        ID_HORARIO = data.ID_HORARIO,
                        FECHA_HORA_ENTRADA = data.FECHA_HORA_ENTRADA,
                        FECHA_HORA_SALIDA = data.FECHA_HORA_SALIDA,
                        TIPO = data.TIPO,
                        HORA_EXTRA = data.HORA_EXTRA
                    });
                }
                foreach (var data in tmp_turnoFijo)
                {
                    if (!data.TEMPORAL)
                    {
                        dtos.Add(new dtoTurnoHorario()
                        {
                            ID_EMPLEADO = data.ID_EMPLEADO,
                            ID_TURNO = data.ID_TURNO,
                            TURNO = data.TURNO,
                            ID_HORARIO = data.ID_HORARIO,
                            FECHA_HORA_ENTRADA = data.FECHA_HORA_ENTRADA,
                            FECHA_HORA_SALIDA = data.FECHA_HORA_SALIDA,
                            TIPO = data.TIPO,
                            HORA_EXTRA = data.HORA_EXTRA
                        });
                    }
                }
            }
            catch (Exception )
            {
                throw;
            }
            return dtos;
        }

        public void Dispose()
        {
        }
    }
}
