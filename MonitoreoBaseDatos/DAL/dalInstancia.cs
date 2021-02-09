using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DTO;

namespace DAL
{
    public class dalInstancia : Conexiones.dalMsSql,IDisposable
    {
        private string strConn { get; set; }
        public dalInstancia(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoInstancia> ejecutarProc(string _strProc)
        {
            try
            {
                return this.proc(this.strConn, _strProc).AsEnumerable().Select((dr => new dtoInstancia()
                {
                    intServidor = dr.Field<int>("se_id_server"),
                    strInstancia = dr.Field<string>("se_instancia").ToString(),
                    strAlias = dr.Field<string>("se_alias").ToString()
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
