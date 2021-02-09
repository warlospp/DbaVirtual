using System;

namespace DTO
{
    [Serializable]
    public class dtoTelemetria : IDisposable
    {
        public int intIdSensor { get; set; }

        public DateTime dtFecha { get; set; }

        public double douMetrica { get; set; }

        public DateTime dtFechaServidor
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
