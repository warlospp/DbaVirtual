using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DTO;

namespace DAL
{
    public class dalOpcion : Conexiones.dalMsSql,IDisposable
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
                    strKeyword = dr.Field<string>("op_keyword").ToString(),
                    strMensaje = dr.Field<string>("op_mensaje").ToString(),
                    strSentencia = dr.Field<string>("op_sentencia").ToString(),
                    strPlantilla = dr.Field<string>("op_plantilla").ToString(),
                })).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
