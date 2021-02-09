using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Reportes
{
    public class dtoMarcacionManual : IDisposable
    {
        public string EMPLEADO { get; set; }
        public DateTime FECHA_HORA { get; set; }
        public string TIPO { get; set; }
        public void Dispose()
        {

        }
    }
}
