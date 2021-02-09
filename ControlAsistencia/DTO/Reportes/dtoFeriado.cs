using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Reportes
{
    public class dtoFeriado : IDisposable
    {
        public string FERIADO { get; set; }

        public DateTime FECHA_INICIO { get; set; }

        public int DURACION { get; set; }
        public void Dispose()
        {

        }
    }
}
