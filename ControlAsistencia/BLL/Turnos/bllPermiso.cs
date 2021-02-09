using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Turnos;
using DTO.Turnos;

namespace BLL.Turnos
{
    public class bllPermiso : IDisposable
    {
        private string strConn { get; set; }

        public bllPermiso(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoPermiso> execQueryPermiso(DateTime _dtFechaInicio, DateTime _dtFechaFin, int[] _intIdEmpleado)
        {
            try
            {
                using (dalPermiso dal = new dalPermiso(this.strConn))
                    return dal.execQueryPermiso("SELECT * FROM USER_SPEDAY;")
                        .Where(p => (
                                          (_dtFechaInicio >= p.STARTSPECDAY && _dtFechaInicio <= p.ENDSPECDAY)
                                          ||
                                          (_dtFechaFin >= p.STARTSPECDAY && _dtFechaFin <= p.ENDSPECDAY)
                                          ||
                                          (p.STARTSPECDAY >= _dtFechaInicio && p.ENDSPECDAY <= _dtFechaFin)
                                      )
                                      && _intIdEmpleado.Contains(p.USERID)

                                )
                        .OrderBy(p => (p.USERID, p.STARTSPECDAY))
                        .ToList<dtoPermiso>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<dtoTipoPermiso> execQueryTipoPermiso()
        {
            try
            {
                using (dalPermiso dal = new dalPermiso(this.strConn))
                    return dal.execQueryTipoPermiso("SELECT * FROM LEAVECLASS;")
                        .OrderBy(p => (p.LEAVEID, p.LEAVENAME, p.REPORTSYMBOL))
                        .ToList<dtoTipoPermiso>();
            }
            catch (Exception)
            {
                throw ;
            }
        }

        public void Dispose()
        {
        }
    }
}
