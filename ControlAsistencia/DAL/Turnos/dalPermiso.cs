using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Turnos;

namespace DAL.Turnos
{
    public class dalPermiso : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalPermiso(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoPermiso> execQueryPermiso( string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoPermiso>((dr => new dtoPermiso()
                {
                    USERID = dr.Field<int>("USERID"),
                    _STARTSPECDAY = dr.Field<object>("STARTSPECDAY"),
                    _ENDSPECDAY = dr.Field<object>("ENDSPECDAY"),
                    DATEID = dr.Field<short>("DATEID"),
                    YUANYING = dr.Field<string>("YUANYING")
                })).ToList<dtoPermiso>();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<dtoTipoPermiso> execQueryTipoPermiso(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoTipoPermiso>((dr => new dtoTipoPermiso()
                {
                    LEAVEID = dr.Field<int>("LEAVEID"),
                    LEAVENAME = dr.Field<string>("LEAVENAME"),
                    REPORTSYMBOL = dr.Field<string>("REPORTSYMBOL")
                })).ToList<dtoTipoPermiso>();
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
