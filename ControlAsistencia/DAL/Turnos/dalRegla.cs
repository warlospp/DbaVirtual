using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Turnos;

namespace DAL.Turnos
{
    public class dalRegla : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalRegla(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoReglaUnpivot> execQuery(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoReglaUnpivot>((dr => new dtoReglaUnpivot()
                {
                    PARANAME = dr.Field<string>("PARANAME"),
                    PARAVALUE = dr.Field<string>("PARAVALUE")
                })).ToList<dtoReglaUnpivot>();
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
