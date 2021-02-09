using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Empleados;

namespace DAL.Empleados
{
    public class dalEmpleado : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalEmpleado(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoEmpleado> execQuery(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select((dr => new dtoEmpleado()
                {
                    USERID = dr.Field<int>("USERID"),
                    Name = dr.Field<string>("Name"),
                    _DEFAULTDEPTID = dr.Field<object>("DEFAULTDEPTID"),
                    _OVERTIME = dr.Field<short>("OVERTIME"),
                    _HOLIDAY = dr.Field<short>("HOLIDAY"),
                    INLATE = dr.Field<short>("INLATE"),
                    OUTEARLY = dr.Field<short>("OUTEARLY")
                })).ToList();
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
