using System;

namespace DTO
{
    [Serializable]
    public class dtoSensor : IDisposable
    {
        public int intIdSensorTipo { get; set; }

        public string strDispositivo { get; set; }

        public string strIP { get; set; }

        public string strDescripcion { get; set; }

        public string strDireccion { get; set; }

        public int intIdSensor { get; set; }

        public string strSensor { get; set; }

        public string strTipo { get; set; }

        public string strUnidadMedida { get; set; }

        public double douLatitud { get; set; }

        public double douLongitud { get; set; }

        public string strColor { get; set; }

        public DateTime dtFecha
        {
            get
            {
                return DateTime.Now;
            }
        }

        public void Dispose()
        {
        }
    }
}
