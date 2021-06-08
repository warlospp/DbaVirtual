using BLL;
using DTO;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF
{
    public class wcfChatBot : IwcfChatBot, IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private string strConnLocal
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;
            }
        }

        private string strConnRemoto
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["dbRemoto"].ConnectionString;
            }
        }

        private string strDirectorio = ConfigurationManager.AppSettings["directorio"].ToString();

        public List<dtoOpcion> opciones(parOpcion _par)
        {
            List<dtoOpcion> dtos = new List<dtoOpcion>();
            try
            {
                using (bllOpcion bll = new bllOpcion(this.strConnLocal))
                {
                    dtos = bll.ejecutar(_par);
                };
                if (!string.IsNullOrEmpty(_par.strUsuario))
                {
                    using (bllAuditoria bll = new bllAuditoria(this.strConnLocal))
                    {
                        bll.ejecutar(new parAuditoria()
                        {
                            strInstancia = _par.intCrud == 3 ? _par.intOpcion.ToString() : _par.strNombre,
                            strOpcion = _par.intCrud.ToString(),
                            strUsuario = _par.strUsuario
                        });
                    };
                }
                logger.Info("Procesar Opciones");
            }
            catch (Exception ex)
            {
                logger.Error<Exception>(ex);
                throw ex;
            }
            return dtos;
        }

        public List<dtoInstancia> cargarInstancias()
        {
            List<dtoInstancia> dtos = new List<dtoInstancia>();
            try
            {
                using (bllInstancia bll = new bllInstancia(this.strConnLocal))
                {
                    dtos = bll.ejecutar();
                };
                logger.Info("Instancias Cargadas...");
            }
            catch (Exception ex)
            {
                logger.Error<Exception>(ex);
                throw ex;
            }
            return dtos;
        }

        public string cargarMensaje(dtoOpcion _dtoOpcion, dtoInstancia _dtoInstancia, string _strUsuario)
        {
            string str = string.Empty;
            try
            {
                List<dtoMensaje> dtosMensaje = new List<dtoMensaje>();
                using (bllMensaje bll = new bllMensaje())
                {
                    string strArchivo = string.Empty;
                    strArchivo = (this.strDirectorio.EndsWith(@"\")
                                        ? this.strDirectorio
                                        : this.strDirectorio + @"\") + _dtoOpcion.strPlantilla;
                    dtosMensaje = bll.ejecutar(strArchivo);
                };
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(dtosMensaje.Where(x => x.strKey == "#Mensaje").Select(x => x.strValue).FirstOrDefault().Replace("@instancia", _dtoInstancia.strAlias));
                sb.AppendLine();
                sb.AppendLine();
                string strConexion = this.strConnRemoto.Replace("@instancia", _dtoInstancia.strInstancia);
                using (bllMensaje bll = new bllMensaje(strConexion))
                {
                    string strQuery = Encoding.UTF8.GetString(_dtoOpcion.arbySentencia);
                    List<dtoMensaje> tmpMensaje = bll.ejecutar(dtosMensaje, strQuery);
                    int intMax = tmpMensaje.Select(x => x.intId).Max();
                    for (int i = 0; i < intMax; i++)
                    {
                        List<dtoMensaje> tmpKeyValue = tmpMensaje.Where(x => x.intId == i).ToList();
                        foreach (dtoMensaje item in tmpKeyValue)
                        {
                            int intLongitud = item.strValue.Length;
                            sb.AppendFormat("{0}\t{1}", item.strKey, (intLongitud < 175 ? item.strValue : item.strValue.Substring(0, 175)));
                            sb.AppendLine();
                        }
                        sb.AppendLine();
                    }
                };
                sb.AppendFormat("Te puedo ayudar con algo más❓");
                sb.AppendLine();
                using (bllAuditoria bll = new bllAuditoria(this.strConnLocal))
                {
                    bll.ejecutar(new parAuditoria()
                    {
                        strInstancia = _dtoInstancia.strAlias,
                        strOpcion = _dtoOpcion.strNombre,
                        strUsuario = _strUsuario
                    });
                };
                str = sb.ToString();
                logger.Info("Mensaje Cargado...");
            }
            catch (Exception ex)
            {
                logger.Error<Exception>(ex);
                throw ex;
            }
            return str;
        }
        public void Dispose()
        {

        }
    }
}
