using BLL;
using CMN;
using DTO;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web.DynamicData;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SVC
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Telemetria : ITelemetria, IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly TelegramBotClient bot = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramToken"], (HttpClient)null);
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        private string strTelegramId
        {
            get
            {
                return ConfigurationManager.AppSettings["TelegramId"];
            }
        }

        public Telemetria()
        {
            try
            {
                this.dic.Add(cmnConfiguraciones.MongoDb, ConfigurationManager.ConnectionStrings[cmnConfiguraciones.MongoDb].ConnectionString);
                this.dic.Add(cmnConfiguraciones.Redis, ConfigurationManager.ConnectionStrings[cmnConfiguraciones.Redis].ConnectionString);
                this.dic.Add(cmnConfiguraciones.ConectorSql, ConfigurationManager.ConnectionStrings[cmnConfiguraciones.Sql].ProviderName);
                this.dic.Add(cmnConfiguraciones.Sql, ConfigurationManager.ConnectionStrings[cmnConfiguraciones.Sql].ConnectionString);
                this.dic.Add(cmnConfiguraciones.NumeroDecimales, ConfigurationManager.AppSettings[cmnConfiguraciones.NumeroDecimales]);
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
        }

        public void insertarAlerta(string _intIdSensor, string _dtFecha, string _douMetrica)
        {
            try
            {
                dtoTelemetria dto = new dtoTelemetria()
                {
                    intIdSensor = int.Parse(_intIdSensor),
                    dtFecha = DateTime.Parse(_dtFecha.Replace('_', ':')),
                    douMetrica = double.Parse(_douMetrica)
                };
                using (bllAlerta bll = new bllAlerta(this.dic))
                {
                    bll.insertar(dto);
                };
                bool boo = false;
                using (bllParametro bll = new bllParametro(this.dic))
                    boo = bll.consultar().booEnviaAlerta;
                if (boo)
                    this.enviarAlerta(dto);
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
        }

        private async void enviarAlerta(dtoTelemetria _dto)
        {            
            try
            {
                List<dtoAlerta> dtos = new List<dtoAlerta>();
                using (bllAlerta bll = new bllAlerta(this.dic))
                    dtos = bll.consultarAlerta(_dto);
                foreach (dtoAlerta dto in dtos)
                {
                    bool boo = false;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine();
                    sb.AppendFormat("<b><a>@{0}</a></b>\t{1}", dto.strDispositivo, dto.strSensor);
                    sb.AppendLine();
                    sb.AppendFormat("[{0}]", dto.strIP);
                    if (dto.strTipo == cmnTipo.Temperatura)
                    {
                        sb.AppendLine();
                        if (dto.strUmbral == cmnUmbral.Bajo)
                        {
                            sb.AppendFormat("<u>{0}</u>\t{1}:\t<code>{2} </code>{3}", (object)dto.strTipo, (object)"❄", (object)dto.douMetrica, (object)dto.strUnidadMedida);
                            boo = true;
                        }
                        else if (dto.strUmbral == cmnUmbral.Sobre)
                        {
                            sb.AppendFormat("<u>{0}</u>\t{1}:\t<code>{2} </code>{3}", dto.strTipo, "\xD83D\xDD25", dto.douMetrica, dto.strUnidadMedida);
                            boo = true;
                        }
                    }
                    else if (dto.strTipo == cmnTipo.Humedad)
                    {
                        sb.AppendLine();
                        if (dto.strUmbral == cmnUmbral.Bajo)
                        {
                            sb.AppendFormat("<u>{0}</u>\t{1}:\t<code>{2}</code>{3}", dto.strTipo, "\xD83C\xDF1E", dto.douMetrica, dto.strUnidadMedida);
                            boo = true;
                        }
                        else if (dto.strUmbral == cmnUmbral.Sobre)
                        {
                            sb.AppendFormat("<u>{0}</u>\t{1}:\t<code>{2}</code>{3}", dto.strTipo, "\xD83D\xDCA6", dto.douMetrica, dto.strUnidadMedida);
                            boo = true;
                        }
                    }
                    if (boo)
                    {
                        sb.AppendLine();
                        sb.AppendFormat("<pre>[{0}]</pre>", dto.strFecha);
                        await Telemetria.bot.SendTextMessageAsync((ChatId)this.strTelegramId, sb.ToString(), ParseMode.Html, false, false, 0, (IReplyMarkup)null, new CancellationToken());
                        Telemetria.logger.Info("Envío de Alerta");
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
        }

        public void eliminarParametros()
        {
            try
            {
                using (bllParametro bll = new bllParametro(this.dic))
                    bll.eliminar();
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
        }

        public void modificarParametros(string _strEnviaAlerta)
        {
            try
            {
                using (bllParametro bll = new bllParametro(this.dic))
                    bll.modificar(new dtoParametro() { booEnviaAlerta = bool.Parse(_strEnviaAlerta) });
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
        }

        public List<dtoAlerta> consultarAlertaDetalle(string _strIntervalo)
        {
            List<dtoAlerta> dtos = new List<dtoAlerta>();
            try
            {
                using (bllAlerta bll = new bllAlerta(this.dic))
                    dtos = bll.consultarAlerta(_strIntervalo, "Detalle").OrderByDescending<dtoAlerta, DateTime>((Func<dtoAlerta, DateTime>)(x => DateTime.Parse(x.strFecha))).ToList<dtoAlerta>();
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
            return dtos;
        }

        public List<dtoAlerta> consultarAlertaxSensor(string _strIntervalo)
        {
            List<dtoAlerta> dtos = new List<dtoAlerta>();
            try
            {
                using (bllAlerta bll = new bllAlerta(this.dic))
                    dtos = bll.consultarAlerta(_strIntervalo, "AgrupadoxSensor").OrderByDescending<dtoAlerta, DateTime>((Func<dtoAlerta, DateTime>)(x => DateTime.Parse(x.strFecha))).ToList<dtoAlerta>();
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
            return dtos;
        }

        public List<dtoAlerta> consultarAlertaxTipo(string _strIntervalo)
        {
            List<dtoAlerta> dtos = new List<dtoAlerta>();
            try
            {
                using (bllAlerta bll = new bllAlerta(this.dic))
                    dtos = bll.consultarAlerta(_strIntervalo, "AgrupadoxTipo").OrderByDescending<dtoAlerta, DateTime>((Func<dtoAlerta, DateTime>)(x => DateTime.Parse(x.strFecha))).ToList<dtoAlerta>();
            }
            catch (Exception ex)
            {
                Telemetria.logger.Error<Exception>(ex);
            }
            return dtos;
        }

        public void Dispose()
        {
        }
    }
}