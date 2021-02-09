using System;

namespace DTO
{
    public class dtoMensaje : IDisposable
    {
        public int intId { get; set; }
        public string strKey { get; set; }
        public string strValue { get; set; }
        public void Dispose()
        {
        }
    }
}
