using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Reportes
{
    public class dtoEstadistica : IDisposable
    {
        public string EMPLEADO { get; set; }

        public string DEPARTAMENTO { get; set; }

        public TimeSpan ENTRADA_TARDE { get; set; }

        public TimeSpan SALIDA_TEMPRANO { get; set; }

        public TimeSpan JORNADA_NORMAL { get; set; }

        public TimeSpan JORNADA_NOCTURNA { get; set; }

        public bool FERIADO { get; set; }

        public bool FALTA { get; set; }

        public bool PERMISO_VACACION { get; set; }

        public TimeSpan HORA_EXTRA_50 { get; set; }

        public TimeSpan HORA_EXTRA_100 { get; set; }

        public TimeSpan TOTAL_LUNCH { get; set; }

        public TimeSpan TOTAL_JORNADA { get; set; }

        public TimeSpan TOTAL_HORA_EXTRA { get; set; }

        public TimeSpan TOTAL_ASISTIDO { get; set; }
        public void Dispose()
        {
        
        }
    }
}
