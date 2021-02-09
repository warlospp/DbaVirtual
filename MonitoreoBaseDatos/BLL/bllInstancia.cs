using System;
using DTO;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class bllInstancia : IDisposable
    {
        private string strConn { get; set; }

        public bllInstancia(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoInstancia> ejecutar()
        {
            try
            {
                using (dalInstancia dal = new dalInstancia(this.strConn))
                    return dal.ejecutarProc("sps_instancias");
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
