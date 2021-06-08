using System;
using System.Runtime.Serialization;

namespace DTO
{
    [Serializable]
    [DataContract]
    public class dtoInstancia : IDisposable
    {
        [DataMember]
        public int intServidor { get; set; }
        [DataMember]
        public string strInstancia { get; set; }
        [DataMember]
        public string strAlias { get; set; }
        public void Dispose()
        {
        }
    }

    [Serializable]
    [DataContract]
    public class parInstancia : IDisposable
    {
        public void Dispose()
        {
            
        }
    }
}
