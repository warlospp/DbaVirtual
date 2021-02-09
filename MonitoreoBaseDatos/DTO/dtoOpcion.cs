using System;

namespace DTO
{
    public class dtoOpcion : IDisposable
    {
        public int intOpcion { get; set; }
        public string strNombre { get; set; }
        public string strKeyword { get; set; }
        public string strMensaje { get; set; }
        public string strSentencia { get; set; }
        public string strPlantilla { get; set; }

        public void Dispose()
        {
        }
    }
}
