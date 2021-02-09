using System;
using System.Runtime.Serialization;

namespace DTO
{
    [DataContract]
    [Serializable]
    public class dtoAlerta : IDisposable
    {
        [DataMember]
        public string strDispositivo { get; set; }

        [DataMember]
        public string strDescripcion { get; set; }

        [DataMember]
        public string strDireccion { get; set; }

        [DataMember]
        public string strIP { get; set; }

        [DataMember]
        public string strSensor { get; set; }

        [DataMember]
        public string strTipo { get; set; }

        [DataMember]
        public string strUnidadMedida { get; set; }

        [DataMember]
        public double douLatitud { get; set; }

        [DataMember]
        public double douLongitud { get; set; }

        [DataMember]
        public string strColor { get; set; }

        [DataMember]
        public string strUmbral { get; set; }

        [DataMember]
        public double douMetrica { get; set; }

        [DataMember]
        public string strFecha { get; set; }

        public void Dispose()
        {
        }
    }
}
