using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DTO;
using DAL.Conexiones;

namespace DAL
{
    public class dalOpcion : dalMsSql<parOpcion>,IDisposable
    {
        private string strConn { get; set; }
        public dalOpcion(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoOpcion> ejecutarProc(string _strProc)
        {
            try
            {
                return this.proc(this.strConn, _strProc).AsEnumerable().Select((dr => new dtoOpcion()
                {
                    intOpcion = dr.Field<int>("op_id_opcion"),
                    strNombre = dr.Field<string>("op_nombre").ToString(),
                    strPlantilla = dr.Field<string>("op_plantilla").ToString(),
                    arbySentencia = dr.Field<byte[]>("op_sentencia"),
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ejecutarProc(string _strProc, parOpcion _par)
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

        public override Dictionary<string, object> parametro(parOpcion _par)
        {
            Dictionary<string, object> par = new Dictionary<string, object>();
            try
            {
                if (_par.intOpcion != null)
                    par.Add("@i_op_id_opcion", _par.intOpcion);
                if(!string.IsNullOrEmpty(_par.strNombre))
                    par.Add("@i_op_nombre", _par.strNombre);
                if (!string.IsNullOrEmpty(_par.strPlantilla))
                    par.Add("@i_op_plantilla", _par.strPlantilla);
                if (_par.arbySentencia != null)
                    par.Add("@i_op_sentencia", _par.arbySentencia);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return par;
        }
    }
}
