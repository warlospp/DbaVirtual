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
    public class dtoRegla : IDisposable
    {
        [DataMember]
        public bool REGLA_ENTRADA { get; set; }
        [DataMember]
        public bool FALTA_ENTRADA { get; set; }
        [DataMember]
        public bool ENTRADA_TARDE { get; set; }
        [DataMember]
        public int MINS_ENTRADA { get; set; }
        [DataMember]
        public bool REGLA_SALIDA { get; set; }
        [DataMember]
        public bool FALTA_SALIDA { get; set; }
        [DataMember]
        public bool SALIDA_TEMPRANO { get; set; }
        [DataMember]
        public int MINS_SALIDA { get; set; }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoReglaPivot : IDisposable
    {
        [DataMember]
        public int NoInAbsent { get; set; }
        [DataMember]
        public int MINSNOIN { get; set; }
        [DataMember]
        public int MINSNOLEAVE { get; set; }
        [DataMember]
        public int NoOutAbsent { get; set; }
        public void Dispose()
        {

        }
    }

    [DataContract]
    [Serializable]
    public class dtoReglaUnpivot : IDisposable
    {
        [DataMember]
        public string PARANAME { get; set; }
        [DataMember]
        public string PARAVALUE { get; set; }
        public void Dispose()
        {

        }
    }
}
