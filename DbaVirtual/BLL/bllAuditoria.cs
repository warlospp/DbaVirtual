using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class bllAuditoria : IDisposable
    {
        private string strConn { get; set; }

        public bllAuditoria(string _strConn)
        {
            this.strConn = _strConn;
        }

        public bool ejecutar(parAuditoria _par)
        {
            try
            {
                using (dalAuditoria dal = new dalAuditoria(this.strConn))
                {
                    return dal.ejecutarProc(procesos.Default.InsertarAuditoria, _par);
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Dispose()
        {

        }
    }
}
