using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Empleados;

namespace DAL.Empleados
{
    public class dalDepartamento : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalDepartamento(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoDepartamento> execQuery(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select((dr => new dtoDepartamento()
                {
                    DEPTID = dr.Field<int>("DEPTID"),
                    DEPTNAME = dr.Field<string>("DEPTNAME").ToString(),
                    SUPDEPTID = dr.Field<int>("SUPDEPTID")
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
