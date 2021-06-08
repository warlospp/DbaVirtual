using System;
using System.Runtime.Serialization;

namespace DTO
{
    [Serializable]
    [DataContract]
    public class dtoOpcion : IDisposable
    {
        [DataMember]
        public int intOpcion { get; set; }
        [DataMember]
        public string strNombre { get; set; }
        [DataMember]
        public string strPlantilla { get; set; }
        [DataMember]
        public byte[] arbySentencia { get; set; }
        public void Dispose()
        {
        }
    }

    [Serializable]
    [DataContract]
    public class parOpcion : IDisposable
    {
        [DataMember]
        public int intCrud { get; set; }
        [DataMember]
        public int? intOpcion { get; set; }
        [DataMember]
        public string strNombre { get; set; }
        [DataMember]
        public string strPlantilla { get; set; }
        [DataMember]
        public byte[] arbySentencia { get; set; }
        [DataMember]
        public string strUsuario { get; set; }
        public void Dispose()
        {
            
        }
    }
}
