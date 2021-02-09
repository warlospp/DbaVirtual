using System;
using DTO;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class bllOpcion : IDisposable
    {
        private string strConn { get; set; }

        public bllOpcion(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoOpcion> ejecutar()
        {
            try
            {
                using (dalOpcion dal = new dalOpcion(this.strConn))
                    return dal.ejecutarProc("sps_opciones");
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
