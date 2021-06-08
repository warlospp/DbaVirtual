using DAL.Conexiones;
using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class dalAuditoria : dalMsSql<parAuditoria>, IDisposable
    {
        private string strConn { get; set; }
        public dalAuditoria(string _strConn)
        {
            this.strConn = _strConn;
        }
        public bool ejecutarProc(string _strProc, parAuditoria _par)
        {
            try
            {
                return this.proc(this.strConn, _strProc, _par);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override Dictionary<string, object> parametro(parAuditoria _par)
        {
            Dictionary<string, object> par = new Dictionary<string, object>();
            try
            {
                par.Add("@i_au_instancia", _par.strInstancia);
                par.Add("@i_au_opcion", _par.strOpcion);
                par.Add("@i_au_usuario", _par.strUsuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return par;
        }
    }
}
