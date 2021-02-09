using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Empleados;
using DTO.Empleados;    

namespace BLL.Empleados
{
    public class bllDepartamento : IDisposable
    {
        private string strConn { get; set; }

        public bllDepartamento(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoDepartamento> execQuery(string[] _strIdDepartamento)
        {
            try
            {
                using (dalDepartamento dal = new dalDepartamento(this.strConn))
                {
                    if (_strIdDepartamento.Contains(string.Empty) && _strIdDepartamento.Count() == 1)
                        return dal.execQuery("Select * from DEPARTMENTS ORDER BY DEPTID;");
                    else
                        return dal.execQuery("Select * from DEPARTMENTS ORDER BY DEPTID;")
                                .Where(d => _strIdDepartamento.Contains(d.DEPTID.ToString()))
                                .ToList();
                };
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
