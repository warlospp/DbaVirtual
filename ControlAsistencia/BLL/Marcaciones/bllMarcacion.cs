using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Marcaciones;
using DAL.Marcaciones;

namespace BLL.Marcaciones
{
    public class bllMarcacion : IDisposable
    {
        private string strConn { get; set; }

        public bllMarcacion(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoMarcacion> execQuery(DateTime _dtFechaInicio, DateTime _dtFechaFin, int[] _intIdEmpleado)
        {
            try
            {
                TimeSpan timeSpan = TimeSpan.Parse("23:59:59");
                DateTime dtFechaInicio = _dtFechaInicio;
                DateTime dateTime = _dtFechaFin.AddDays(1.0).Add(timeSpan);
                using (dalMarcacion dal = new dalMarcacion(this.strConn))
                    return dal.execQuery("Select * from CHECKINOUT WHERE CHECKTIME BETWEEN @dtIni AND @dtFin;", new Dictionary<string, string>()
                    {
                        {"@dtIni", dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss")},
                        {"@dtFin", dateTime.ToString("yyyy-MM-dd HH:mm:ss")}
                    })
                    .Where(m => _intIdEmpleado.Contains(m.USERID))
                    .OrderBy(m => m.USERID)
                    .ToList<dtoMarcacion>();
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public void Dispose()
        {
        }
    }
}
