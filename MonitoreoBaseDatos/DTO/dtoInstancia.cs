using System;

namespace DTO
{
    public class dtoInstancia : IDisposable
    {
        public int intServidor { get; set; }
        public string strInstancia { get; set; }
        public string strAlias { get; set; }
        public void Dispose()
        {
        }
    }
}
