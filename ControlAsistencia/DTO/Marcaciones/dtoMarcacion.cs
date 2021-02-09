using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Marcaciones
{
    [DataContract]
    [Serializable]
    public class dtoMarcacion : IDisposable
    {
        [DataMember]
        public int USERID { get; set; }
        [DataMember]
        public object _CHECKTIME { get; set; }
        [DataMember]
        public DateTime CHECKTIME
        {
            get
            {
                return DateTime.Parse(DateTime.Parse(this._CHECKTIME.ToString()).ToString("yyyy-MM-dd HH:mm:00"));
            }
        }
        [DataMember]
        public string DIA
        {
            get
            {
                switch (this.CHECKTIME.DayOfWeek)
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
        public string CHECKTYPE { get; set; }
        public void Dispose()
        {
        }
    }
}
