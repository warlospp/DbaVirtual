using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Turnos;
using DAL.Turnos;

namespace BLL.Turnos
{
    public class bllFeriado : IDisposable
    {
        private string strConn { get; set; }

        public bllFeriado(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoFeriado> execQuery(DateTime _dtFechaInicio, DateTime _dtFechaFin)
        {
            try
            {
                using (dalFeriado dal = new dalFeriado(this.strConn))
                    return dal.execQuery("Select * from HOLIDAYS;")
                        .Where(f =>
                        {
                            if (f.DURATION <= 0)
                                return false;
                            if (f.STARTTIME >= _dtFechaInicio && f.STARTTIME <= _dtFechaFin)
                                return true;
                            return f.ENDTIME >= _dtFechaInicio && f.ENDTIME <= _dtFechaFin;
                        })
                    .OrderBy(f => f.STARTTIME)
                    .ToList<dtoFeriado>();
            }
            catch (Exception)
            {
                throw ;
            }
        }

        //public List<dtoFeriado> execQueryFeriado(string _strQuery)
        //{
        //    try
        //    {
        //        using (dalFeriado dal = new dalFeriado(this.strConn))
        //            return dal.execQuery( _strQuery);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void Dispose()
        {
        }
    }
}
