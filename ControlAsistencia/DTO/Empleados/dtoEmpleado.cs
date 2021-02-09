using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Empleados
{
    [DataContract]
    [Serializable]
    public class dtoEmpleado : IDisposable
    {
        [DataMember]
        public int USERID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public object _DEFAULTDEPTID { get; set; }
        [DataMember]
        public int DEFAULTDEPTID
        {
            get
            {
                if (this._DEFAULTDEPTID.Equals((object)null))
                    this._DEFAULTDEPTID = (object)0;
                return int.Parse(this._DEFAULTDEPTID.ToString());
            }
        }
        [DataMember]
        public short _OVERTIME { get; set; }
        [DataMember]
        public bool OVERTIME
        {
            get
            {
                return this._OVERTIME != (short)0;
            }
        }
        [DataMember]
        public short _HOLIDAY { get; set; }
        [DataMember]
        public bool HOLIDAY
        {
            get
            {
                return this._HOLIDAY != (short)0;
            }
        }
        [DataMember]
        public short INLATE { get; set; }
        [DataMember]
        public short OUTEARLY { get; set; }
        public void Dispose()
        {

        }
    }
}
