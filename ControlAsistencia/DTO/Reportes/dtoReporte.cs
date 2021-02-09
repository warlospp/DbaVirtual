using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Reportes
{
    public class dtoReporte : IDisposable
    {
        public string EMPLEADO { get; set; }

        public string DEPARTAMENTO { get; set; }

        public string TURNO { get; set; }

        public string HORARIO { get; set; }

        public string DIA { get; set; }

        public DateTime MARCACION_ENTRADA { get; set; }

        public DateTime FECHA_HORA_INICIO_ENTRADA { get; set; }

        public DateTime FECHA_HORA_ENTRADA { get; set; }

        public DateTime FECHA_HORA_FIN_ENTRADA { get; set; }

        public TimeSpan ENTRADA_TARDE { get; set; }

        public TimeSpan ENTRADA_TEMPRANO { get; set; }

        public DateTime MARCACION_SALIDA { get; set; }

        public DateTime FECHA_HORA_INICIO_SALIDA { get; set; }

        public DateTime FECHA_HORA_SALIDA { get; set; }

        public DateTime FECHA_HORA_FIN_SALIDA { get; set; }

        public TimeSpan SALIDA_TEMPRANO { get; set; }

        public TimeSpan SALIDA_TARDE { get; set; }

        public TimeSpan LUNCH { get; set; }

        public DateTime MARCACION_ENTRADA_LUNCH { get; set; }

        public DateTime MARCACION_SALIDA_LUNCH { get; set; }

        public bool SOBRETIEMPO_LUNCH { get; set; }

        public TimeSpan TOTAL_LUNCH { get; set; }

        public TimeSpan JORNADA_NORMAL { get; set; }

        public TimeSpan JORNADA_NOCTURNA { get; set; }

        public bool FERIADO { get; set; }

        public bool FALTA { get; set; }

        public bool PERMISO_VACACION { get; set; }

        public TimeSpan HORA_EXTRA_50 { get; set; }

        public TimeSpan HORA_EXTRA_100 { get; set; }

        public TimeSpan TOTAL_JORNADA { get; set; }

        public TimeSpan TOTAL_HORA_EXTRA { get; set; }

        public TimeSpan TOTAL_ASISTIDO { get; set; }
        public void Dispose()
        {

        }
    }
}
