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
    public class dtoPermiso : IDisposable
    {
        [DataMember]
        public int USERID { get; set; }
        [DataMember]
        public object _STARTSPECDAY { get; set; }
        [DataMember]
        public DateTime STARTSPECDAY
        {
            get
            {
                return DateTime.Parse(this._STARTSPECDAY.ToString());
            }
        }
        [DataMember]
        public object _ENDSPECDAY { get; set; }
        [DataMember]
        public DateTime ENDSPECDAY
        {
            get
            {
                return DateTime.Parse(this._ENDSPECDAY.ToString());
            }
        }
        [DataMember]
        public short DATEID { get; set; }
        [DataMember]
        public string YUANYING { get; set; }
        public void Dispose()
        {

        }
    }
    [DataContract]
    [Serializable]
    public class dtoTipoPermiso : IDisposable
    {
        [DataMember]
        public int LEAVEID { get; set; }
        [DataMember]
        public string LEAVENAME { get; set; }
        [DataMember]
        public string REPORTSYMBOL { get; set; }
        public void Dispose()
        {

        }
    }
}
