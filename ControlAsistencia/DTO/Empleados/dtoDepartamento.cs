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
    public class dtoDepartamento : IDisposable
    {
        [DataMember]
        public int DEPTID { get; set; }
        [DataMember]
        public string DEPTNAME { get; set; }
        [DataMember]
        public int SUPDEPTID { get; set; }
        public void Dispose()
        {

        }
    }
}
