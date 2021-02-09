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
    public class dtoFecha : IDisposable
    {
        [DataMember]
        public DateTime FECHA { get; set; }
        [DataMember]
        public int DIA_SEMANA
        {
            get
            {
                int dayOfWeek = (int)this.FECHA.DayOfWeek;
                return dayOfWeek == 0 ? 7 : dayOfWeek;
            }
        }
        [DataMember]
        public int DIA_MES
        {
            get
            {
                return this.FECHA.Day;
            }
        }
        public void Dispose()
        {

        }
    }
}
