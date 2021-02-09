using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configuraciones
{
    public class dalConfiguracion : Conexiones.dalMsAccess, IDisposable
    {
        //public bool execQuery(string _strConn)
        //{
        //    return this.exec(_strConn);
        //}

        public void Dispose()
        {
        }
    }
}
