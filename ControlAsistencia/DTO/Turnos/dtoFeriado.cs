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
    public class dtoFeriado : IDisposable
    {
        [DataMember]
        public int HOLIDAYID { get; set; }
        [DataMember]
        public string HOLIDAYNAME { get; set; }
        [DataMember]
        public object _STARTTIME { get; set; }
        [DataMember]
        public DateTime STARTTIME
        {
            get
            {
                return DateTime.Parse(this._STARTTIME.ToString()).Add(TimeSpan.Parse("00:00:00"));
            }
        }
        [DataMember]
        public short DURATION { get; set; }
        [DataMember]
        public DateTime ENDTIME
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.Parse("23:59:59");
                DateTime dateTime = this.STARTTIME;
                dateTime = dateTime.AddDays((double)this.DURATION);
                return dateTime.Add(timeSpan);
            }
        }
        public void Dispose()
        {

        }
    }
}
