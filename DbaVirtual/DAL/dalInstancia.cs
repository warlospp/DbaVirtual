using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DTO;
using DAL.Conexiones;

namespace DAL
{
    public class dalInstancia : dalMsSql<parInstancia>,IDisposable
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

        public override Dictionary<string, object> parametro(parInstancia _par)
        {
            Dictionary<string, object> par = new Dictionary<string, object>();
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return par;
        }
    }
}
