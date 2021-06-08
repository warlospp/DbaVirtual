using System;
using System.Runtime.Serialization;

namespace DTO
{
    [Serializable]
    [DataContract]
    public class dtoMensaje : IDisposable
    {
        [DataMember]
        public int intId { get; set; }
        [DataMember]
        public string strKey { get; set; }
        [DataMember]
        public string strValue { get; set; }
        public void Dispose()
        {
        }
    }

    [Serializable]
    [DataContract]
    public class parMensaje : IDisposable
    {
        public void Dispose()
        {
            
        }
    }
}
