using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Reportes
{
    public class dtoPermiso : IDisposable
    {
        public string EMPLEADO { get; set; }

        public DateTime FECHA_INICO { get; set; }

        public DateTime FECHA_FIN { get; set; }

        public string PERMISO { get; set; }

        public string MOTIVO { get; set; }
        public void Dispose()
        {
  
        }
    }
}
