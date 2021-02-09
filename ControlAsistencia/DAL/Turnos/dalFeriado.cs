using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Turnos;

namespace DAL.Turnos
{
    public class dalFeriado : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalFeriado(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoFeriado> execQuery(string _strQuery,Dictionary<string, string> _param)
        {
            try
            {
                return this.exec(this.strConn, _strQuery, _param).AsEnumerable().Select<DataRow, dtoFeriado>((dr => new dtoFeriado()
                {
                    HOLIDAYID = dr.Field<int>("HOLIDAYID"),
                    HOLIDAYNAME = dr.Field<string>("HOLIDAYNAME"),
                    _STARTTIME = dr.Field<object>("STARTTIME"),
                    DURATION = dr.Field<short>("DURATION")
                })).ToList<dtoFeriado>();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<dtoFeriado> execQuery( string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoFeriado>((dr => new dtoFeriado()
                {
                    HOLIDAYID = dr.Field<int>("HOLIDAYID"),
                    HOLIDAYNAME = dr.Field<string>("HOLIDAYNAME"),
                    _STARTTIME = dr.Field<object>("STARTTIME"),
                    DURATION = dr.Field<short>("DURATION")
                })).ToList<dtoFeriado>();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void Dispose()
        {
        }
    }
}
