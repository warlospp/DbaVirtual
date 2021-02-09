using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Reportes
{
    public class dtoTurno : IDisposable
    {
        public string EMPLEADO { get; set; }

        public string TURNO { get; set; }

        public string HORARIO { get; set; }

        public string DIA { get; set; }

        public DateTime FECHA { get; set; }

        public TimeSpan HORA_INICIO_ENTRADA { get; set; }

        public TimeSpan HORA_ENTRADA { get; set; }

        public TimeSpan HORA_FIN_ENTRADA { get; set; }

        public TimeSpan HORA_INICIO_SALIDA { get; set; }

        public TimeSpan HORA_SALIDA { get; set; }

        public TimeSpan HORA_FIN_SALIDA { get; set; }
        public void Dispose()
        {

        }
    }
}
