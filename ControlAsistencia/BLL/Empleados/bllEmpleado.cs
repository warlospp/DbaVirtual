using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Empleados;
using DTO.Empleados;  

namespace BLL.Empleados
{
    public class bllEmpleado : IDisposable
    {
        private string strConn { get; set; }

        public bllEmpleado(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoEmpleado> execQuery(string[] _strIdDepartamento, string _strEmpleado)
        {
            try
            {
                if (_strEmpleado == string.Empty)
                    using (dalEmpleado dal = new dalEmpleado(this.strConn))
                        return (from e in dal.execQuery("SELECT * from USERINFO ORDER BY USERID;")
                               where _strIdDepartamento.Contains(e.DEFAULTDEPTID.ToString())
                               select e).ToList();
                else
                    using (dalEmpleado dal = new dalEmpleado(this.strConn))
                        return (from e in dal.execQuery("SELECT * from USERINFO ORDER BY USERID;")
                                where _strIdDepartamento.Contains(e.DEFAULTDEPTID.ToString())
                                && e.Name.ToUpper().StartsWith(_strEmpleado.ToUpper())
                                select e).ToList();
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
