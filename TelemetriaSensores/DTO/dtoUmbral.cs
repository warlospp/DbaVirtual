using System;

namespace DTO
{
    [Serializable]
    public class dtoUmbral : IDisposable
    {
        public int intIdUmbral { get; set; }

        public string strTipo { get; set; }

        public string strUmbral { get; set; }

        public string strDescripcion { get; set; }

        public double douMinimo { get; set; }

        public double douMaximo { get; set; }

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
