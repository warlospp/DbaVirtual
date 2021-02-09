using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Marcaciones;

namespace DAL.Marcaciones
{
    public class dalMarcacion : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalMarcacion(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoMarcacion> execQuery(string _strQuery, Dictionary<string, string> _param)
        {
            try
            {
                return this.exec(this.strConn, _strQuery, _param).AsEnumerable().Select<DataRow, dtoMarcacion>((dr => new dtoMarcacion()
                {
                    USERID = dr.Field<int>("USERID"),
                    _CHECKTIME = dr.Field<object>("CHECKTIME"),
                    CHECKTYPE = dr.Field<string>("CHECKTYPE").ToString().ToUpper().Trim() == "O" ? "O" : "I"
                })).ToList<dtoMarcacion>();
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
