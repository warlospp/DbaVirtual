using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Turnos
{

    [DataContract]
    [Serializable]
    public class dtoHorario : IDisposable
    {
        [DataMember]
        public int SCHCLASSID { get; set; }
        [DataMember]
        public string SCHNAME { get; set; }
        [DataMember]
        public object _STARTTIME { get; set; }
        [DataMember]
        public TimeSpan STARTTIME
        {
            get
            {
                return TimeSpan.Parse(DateTime.Parse(this._STARTTIME.ToString()).ToString("HH:mm:ss"));
            }
        }
        [DataMember]
        public object _ENDTIME { get; set; }
        [DataMember]
        public TimeSpan ENDTIME
        {
            get
            {
                return TimeSpan.Parse(DateTime.Parse(this._ENDTIME.ToString()).ToString("HH:mm:ss"));
            }
        }
        [DataMember]
        public int LATEMINUTES { get; set; }
        [DataMember]
        public int EARLYMINUTES { get; set; }
        [DataMember]
        public int _CHECKIN { get; set; }
        [DataMember]
        public bool CHECKIN
        {
            get
            {
                return this._CHECKIN != 0;
            }
        }
        [DataMember]
        public int _CHECKOUT { get; set; }
        [DataMember]
        public bool CHECKOUT
        {
            get
            {
                return this._CHECKOUT != 0;
            }
        }
        [DataMember]
        public object _CHECKINTIME1 { get; set; }
        [DataMember]
        public TimeSpan CHECKINTIME1
        {
            get
            {
                return TimeSpan.Parse(DateTime.Parse(this._CHECKINTIME1.ToString()).ToString("HH:mm:ss"));
            }
        }
        [DataMember]
        public object _CHECKINTIME2 { get; set; }
        [DataMember]
        public TimeSpan CHECKINTIME2
        {
            get
            {
                return TimeSpan.Parse(DateTime.Parse(this._CHECKINTIME2.ToString()).ToString("HH:mm:ss"));
            }
        }
        [DataMember]
        public object _CHECKOUTTIME1 { get; set; }
        [DataMember]
        public TimeSpan CHECKOUTTIME1
        {
            get
            {
                return TimeSpan.Parse(DateTime.Parse(this._CHECKOUTTIME1.ToString()).ToString("HH:mm:ss"));
            }
        }
        [DataMember]
        public object _CHECKOUTTIME2 { get; set; }
        [DataMember]
        public TimeSpan CHECKOUTTIME2
        {
            get
            {
                return TimeSpan.Parse(DateTime.Parse(this._CHECKOUTTIME2.ToString()).ToString("HH:mm:ss"));
            }
        }
        [DataMember]
        public double WorkDay { get; set; }
        [DataMember]
        public double _WorkMins { get; set; }
        [DataMember]
        public double WorkMins
        {
            get
            {
                DateTime dateTime = DateTime.Parse(this._ENDTIME.ToString());
                TimeSpan timeSpan1 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                dateTime = DateTime.Parse(this._STARTTIME.ToString());
                TimeSpan timeSpan2 = TimeSpan.Parse(dateTime.ToString("HH:mm:ss"));
                TimeSpan timeSpan3 = timeSpan1 - timeSpan2;
                return this._WorkMins <= 1.0 ? Math.Abs(timeSpan3.TotalMinutes) : this._WorkMins;
            }
        }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoTurnoHorario : IDisposable
    {
        [DataMember]
        public int ID_TURNO { get; set; }
        [DataMember]
        public int ID_EMPLEADO { get; set; }
        [DataMember]
        public string TURNO { get; set; }
        [DataMember]
        public DateTime FECHA_HORA_ENTRADA { get; set; }
        [DataMember]
        public DateTime FECHA_HORA_SALIDA { get; set; }
        [DataMember]
        public string DIA
        {
            get
            {
                switch (this.FECHA_HORA_ENTRADA.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return "DOMINGO";
                    case DayOfWeek.Tuesday:
                        return "MARTES";
                    case DayOfWeek.Wednesday:
                        return "MIERCOLES";
                    case DayOfWeek.Thursday:
                        return "JUEVES";
                    case DayOfWeek.Friday:
                        return "VIERNES";
                    case DayOfWeek.Saturday:
                        return "SABADO";
                    default:
                        return "LUNES";
                }
            }
        }
        [DataMember]
        public int ID_HORARIO { get; set; }
        [DataMember]
        public string TIPO { get; set; }
        [DataMember]
        public bool HORA_EXTRA { get; set; }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoTurnoCabecera : IDisposable
    {
        [DataMember]
        public int NUM_RUNID { get; set; }
        [DataMember]
        public string NAME { get; set; }
        [DataMember]
        public object _STARTDATE { get; set; }
        [DataMember]
        public DateTime STARTDATE
        {
            get
            {
                return DateTime.Parse(this._STARTDATE.ToString()).Add(TimeSpan.Parse("00:00:00"));
            }
        }
        [DataMember]
        public object _ENDDATE { get; set; }
        [DataMember]
        public DateTime ENDDATE
        {
            get
            {
                return DateTime.Parse(this._ENDDATE.ToString()).Add(TimeSpan.Parse("23:59:59"));
            }
        }
        [DataMember]
        public short CYLE { get; set; }
        [DataMember]
        public short _UNITS { get; set; }
        [DataMember]
        public string UNITS
        {
            get
            {
                string str = string.Empty;
                switch (this._UNITS)
                {
                    case 0:
                        str = "DIA";
                        break;
                    case 1:
                        str = "SEMANA";
                        break;
                    case 2:
                        str = "MES";
                        break;
                }
                return str;
            }
        }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoTurnoDetalle : IDisposable
    {
        [DataMember]
        public short NUM_RUNID { get; set; }
        [DataMember]
        public short SDAYS { get; set; }
        [DataMember]
        public short EDAYS { get; set; }
        [DataMember]
        public int SCHCLASSID { get; set; }
        [DataMember]
        public object _Overtime { get; set; }
        [DataMember]
        public bool Overtime
        {
            get
            {
                bool flag = false;
                if (this._Overtime != null)
                    flag = int.Parse(this._Overtime.ToString()) != 0;
                return flag;
            }
        }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoTurnoFijo : IDisposable
    {
        [DataMember]
        public int USERID { get; set; }
        [DataMember]
        public int NUM_OF_RUN_ID { get; set; }
        [DataMember]
        public object _STARTDATE { get; set; }
        [DataMember]
        public DateTime STARTDATE
        {
            get
            {
                return DateTime.Parse(this._STARTDATE.ToString()).Add(TimeSpan.Parse("00:00:00"));
            }
        }
        [DataMember]
        public object _ENDDATE { get; set; }
        [DataMember]
        public DateTime ENDDATE
        {
            get
            {
                return DateTime.Parse(this._ENDDATE.ToString()).Add(TimeSpan.Parse("23:59:59"));
            }
        }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoTurnoTemporal : IDisposable
    {
        [DataMember]
        public int USERID { get; set; }
        [DataMember]
        public string NAME
        {
            get
            {
                return "<< Turno Temporal >>";
            }
        }
        [DataMember]
        public object _COMETIME { get; set; }
        [DataMember]
        public DateTime COMETIME
        {
            get
            {
                return DateTime.Parse(DateTime.Parse(this._COMETIME.ToString()).ToString("yyyy-MM-dd"));
            }
        }
        [DataMember]
        public object _LEAVETIME { get; set; }
        [DataMember]
        public DateTime LEAVETIME
        {
            get
            {
                DateTime dateTime1;
                if (!(DateTime.Parse(this._LEAVETIME.ToString()) <= DateTime.Parse(this._COMETIME.ToString())))
                {
                    dateTime1 = DateTime.Parse(DateTime.Parse(this._LEAVETIME.ToString()).ToString("yyyy-MM-dd"));
                }
                else
                {
                    DateTime dateTime2 = DateTime.Parse(this._LEAVETIME.ToString());
                    dateTime2 = dateTime2.AddDays(1.0);
                    dateTime1 = DateTime.Parse(dateTime2.ToString("yyyy-MM-dd"));
                }
                return dateTime1;
            }
        }
        [DataMember]
        public int _OVERTIME { get; set; }
        [DataMember]
        public bool OVERTIME
        {
            get
            {
                return this._OVERTIME != 0;
            }
        }
        [DataMember]
        public int SCHCLASSID { get; set; }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoTurnoRotativo : IDisposable
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string NAME
        {
            get
            {
                return "<< Turno Rotativo >>";
            }
        }
        [DataMember]
        public object _COMETIME { get; set; }
        [DataMember]
        public DateTime COMETIME
        {
            get
            {
                return DateTime.Parse(DateTime.Parse(this._COMETIME.ToString()).ToString("yyyy-MM-dd"));
            }
        }
        [DataMember]
        public object _LEAVETIME { get; set; }
        [DataMember]
        public DateTime LEAVETIME
        {
            get
            {
                DateTime dateTime1;
                if (!(DateTime.Parse(this._LEAVETIME.ToString()) < DateTime.Parse(this._COMETIME.ToString())))
                {
                    dateTime1 = DateTime.Parse(DateTime.Parse(this._LEAVETIME.ToString()).ToString("yyyy-MM-dd"));
                }
                else
                {
                    DateTime dateTime2 = DateTime.Parse(this._LEAVETIME.ToString());
                    dateTime2 = dateTime2.AddDays(1.0);
                    dateTime1 = DateTime.Parse(dateTime2.ToString("yyyy-MM-dd"));
                }
                return dateTime1;
            }
        }
        [DataMember]
        public bool OVERTIME
        {
            get
            {
                return false;
            }
        }
        [DataMember]
        public int SchId { get; set; }
        public void Dispose()
        {

        }
    }
}
