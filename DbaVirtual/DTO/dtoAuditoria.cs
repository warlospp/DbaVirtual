using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class parAuditoria : IDisposable
    {
        public string strInstancia { get; set; }
        public string strOpcion { get; set; }
        public string strUsuario { get; set; }
        public void Dispose()
        {

        }
    }
}
