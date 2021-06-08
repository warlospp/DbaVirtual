using DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SRV
{
    public partial class srvChatBot : ServiceBase
    {
        private TelegramBotClient bot = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramToken"]);
        private string strTelegramChatId
        {
            get
            {
                return ConfigurationManager.AppSettings["TelegramChatId"];
            }
        }
        private string strLUISURL
        {
            get
            {
                return ConfigurationManager.AppSettings["LUISURL"];
            }
        }

        private float fltPrecision
        {
            get
            {
                return float.Parse(ConfigurationManager.AppSettings["Precision"]);
            }
        }       

        public string strUsuario { get; set; }

        private ReplyKeyboardMarkup rkmInteraccion = new ReplyKeyboardMarkup();

        private readonly ReplyKeyboardMarkup rkmOpciones = new ReplyKeyboardMarkup();
        private List<dtoOpcion> dtosOpcion = new List<dtoOpcion>();

        private readonly ReplyKeyboardMarkup rkmInstancias = new ReplyKeyboardMarkup();
        private List<dtoInstancia> dtosInstancia = new List<dtoInstancia>();
            
        private string strInstancia = string.Empty;
        private string strOpcion = string.Empty;
        public srvChatBot()
        {
            InitializeComponent();
            this.cargarMenus();
            Thread thread = new Thread(listenerMensajeria);
            thread.Start();
        }

        private void cargarMenus()
        {
            try
            {
                List<KeyboardButton[]> rows = new List<KeyboardButton[]>();
                List<KeyboardButton> cols = new List<KeyboardButton>();

                this.rkmOpciones.ResizeKeyboard = true;
                using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                {
                    this.dtosOpcion = proxy.opciones(new parOpcion() { intCrud = 0 }).ToList();
                    proxy.Close();
                };
                int intModulo = this.dtosOpcion.Count % 3;
                int intTotal = this.dtosOpcion.Count;
                int i = 0;
                foreach (var dto in this.dtosOpcion)
                {
                    i++;
                    cols.Add(new KeyboardButton(dto.strNombre));
                    if (i % 3 == 0)
                    {
                        rows.Add(cols.ToArray());
                        cols = new List<KeyboardButton>();
                    }
                    if (i >= (intTotal - intModulo) + 1 && i % intModulo == 0)
                    {
                        rows.Add(cols.ToArray());
                        cols = new List<KeyboardButton>();
                    }
                }
                this.rkmOpciones.Keyboard = rows.ToArray();

                rows = new List<KeyboardButton[]>();
                cols = new List<KeyboardButton>();

                this.rkmInstancias.ResizeKeyboard = true;
                using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                {
                    this.dtosInstancia = proxy.cargarInstancias().ToList();
                    proxy.Close();
                };
                foreach (var dto in this.dtosInstancia)
                {
                    cols.Add(new KeyboardButton(dto.strAlias));
                    rows.Add(cols.ToArray());
                    cols = new List<KeyboardButton>();
                }

                this.rkmInstancias.Keyboard = rows.ToArray();

                this.rkmInteraccion.OneTimeKeyboard = true;
                this.rkmInteraccion.ResizeKeyboard = true;
                this.rkmInteraccion.Keyboard =
                new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Opciones ⚙"),
                        new KeyboardButton("Reconfigurar 🛠")                        
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("Chao 👋")
                    }
                };
                Program.logger.Info("Menus Cargados...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }
        private void listenerMensajeria()
        {
            try
            {
                var me = bot.GetMeAsync().Result;
                this.strUsuario = me.Username;
                bot.OnMessage += onLeerMensajes;
                bot.OnReceiveError += onErrorRecibido;
                bot.StartReceiving(Array.Empty<UpdateType>());
                Program.logger.Info("Escuchando ChatBot " + this.strUsuario + " ...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        private async void onLeerMensajes(object sender, MessageEventArgs evt)
        {
            try
            {
                var mensaje = evt.Message;

                if (mensaje == null || mensaje.Type != MessageType.Text)
                    return;

                this.strOpcion = this.intencionLUIS(mensaje.Text.ToLower());
                
                //this.strOpcion = mensaje.Text.ToLower().Replace(".", string.Empty).Split(' ').First();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                string strUsuario = mensaje.From.FirstName + mensaje.From.LastName;
                switch (this.strOpcion)
                {
                    case "saludar":
                        this.saludar(strUsuario);
                        break;
                    case "opciones":
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("👇 Te puedo ayudar con una de estas opciones 😊");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, this.rkmOpciones, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)this.strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/8.webp", false, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, "Selecciona por favor 🙏", ParseMode.Html, false, false, mensaje.MessageId, this.rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (this.dtosOpcion.Where(x => x.strNombre.ToLower() == this.strOpcion.ToLower()).Count() >= 1 ? true : false):
                        this.strOpcion = this.dtosOpcion.Where(x => x.strNombre.ToLower() == this.strOpcion.ToLower()).Select(x => x.strNombre).FirstOrDefault();
                        this.strInstancia = this.strOpcion; 
                        sb.AppendLine();
                        sb.AppendFormat("👇 Selecciona la Instancia 👇");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmInstancias, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/10.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (this.dtosInstancia.Where(x => x.strAlias == this.strOpcion).Count() >= 1 ? true : false):
                        string strMensaje = string.Empty;
                        string str = this.strInstancia;
                        this.strInstancia = this.strOpcion;
                        this.strOpcion = str;
                        var tmpInstancia = this.dtosInstancia.Where(x => x.strAlias == this.strInstancia).FirstOrDefault();
                        var tmpOpcion = this.dtosOpcion.Where(x => x.strNombre == this.strOpcion).FirstOrDefault();
                        using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                        {
                            strMensaje = proxy.cargarMensaje(tmpOpcion, tmpInstancia, strUsuario);
                            proxy.Close();
                        };
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, strMensaje, ParseMode.Html, false, false, mensaje.MessageId, this.rkmInteraccion, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/11.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case "despedir":
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("😊 Gracias ‼, vuelve pronto @" + strUsuario);
                        sb.AppendLine();
                        sb.AppendFormat(this.strSaludo(true) + "👋");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync((ChatId)this.strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/2.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case "agradecer":
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("😊 Gracias a ti @" + strUsuario + "‼, es un placer ayudarte");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/11.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case "reconfigurar":
                        this.cargarMenus();
                        sb.AppendLine();
                        sb.AppendFormat("👌 Las configuraciones están actualizadas 👍");
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.AppendFormat("😊 En qué más te puedo ayudar❓");
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, this.rkmInteraccion, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/11.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    default:
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendFormat("😪 Con eso no te puedo ayudar 😵");
                        sb.AppendLine();
                        sb.AppendFormat("Intenta con algunas de estas opciones 🙏");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, this.rkmOpciones, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)this.strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/4.webp", false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                }
                Program.logger.Info("Mensaje leído: " + strUsuario + ", Instancia: " + this.strInstancia + ", Opcion: " + this.strOpcion);
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        private string intencionLUIS(string _strMensaje)
        {
            string strIntencion = string.Empty;
            List<dtoMensaje> dtos = new List<dtoMensaje>();
            try
            {
                if (this.dtosInstancia.Where(x => x.strAlias == _strMensaje).Count() >= 1)
                {
                    strIntencion = _strMensaje;
                    string strScore = "1";
                    Program.logger.Info("Mensaje: " + _strMensaje + ", Intencion: " + strIntencion + ", Score: " + strScore);
                }
                else
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.strLUISURL + _strMensaje);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var file = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        using (JsonTextReader reader = new JsonTextReader(file))
                        {
                            JObject obj = (JObject)JToken.ReadFrom(reader);
                            JObject subObjs = (JObject)obj["prediction"]["intents"];
                            foreach (JProperty parsedProperty in subObjs.Properties())
                            {
                                string strNombre = parsedProperty.Name;
                                JObject parsedValueObj = (JObject)parsedProperty.Value;
                                foreach (JProperty parsedValue in parsedValueObj.Properties())
                                {
                                    if (parsedValue.Name == "score")
                                    {
                                        string strValor = (string)parsedValue.Value;
                                        dtos.Add(new dtoMensaje()
                                        {
                                            intId = 0,
                                            strKey = strNombre,
                                            strValue = strValor
                                        });
                                    }
                                }
                            }
                        };
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tmp = dtos.Where(x => float.Parse(x.strValue) >= this.fltPrecision);
            if (tmp.Count() >= 1)
            {
                strIntencion = tmp.OrderByDescending(x => float.Parse(x.strValue)).Select(x => x.strKey).FirstOrDefault();
                string strScore = tmp.OrderByDescending(x => float.Parse(x.strValue)).Select(x => x.strValue).FirstOrDefault();
                Program.logger.Info("Mensaje: " + _strMensaje + ", Intencion: " + strIntencion + ", Score: " + strScore);
            }
            return strIntencion.ToLower();
        }

        private async void saludar(string _strUsuario)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                this.strInstancia = string.Empty;
                this.strOpcion = string.Empty;
                sb.AppendFormat("Hola @" + _strUsuario + " " + strSaludo(false) + " 😊 ‼ yo soy <b><a>@{0}</a></b>.", strUsuario);
                sb.AppendLine();
                sb.AppendFormat("En qué te puedo ayudar❓");
                sb.AppendLine();
                await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, 0, this.rkmInteraccion, new CancellationToken());
                await bot.SendStickerAsync((ChatId)this.strTelegramChatId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/1.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                Program.logger.Info("ChatBot Saludando...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        private string strSaludo(bool _booTipo)
        {
            int intHora = DateTime.Now.Hour;
            if (_booTipo)
            {
                return intHora >= 12 && intHora <= 18
                        ? "Buenas Tardes"
                        : intHora >= 19 && intHora <= 23
                            ? "Buenas Noches"
                            : "Buenos Días";
            }
            else
            {
                return intHora >= 12 && intHora <= 18
                        ? "buenas tardes"
                        : intHora >= 19 && intHora <= 23
                            ? "buenas noches"
                            : "buenos días";
            }
        }

        private void onErrorRecibido(object sender, ReceiveErrorEventArgs evt)
        {
            Program.logger.Error<Exception>(evt.ApiRequestException);
        }

        protected override void OnStart(string[] args)
        {
            this.saludar("Dba");
        }

        protected override void OnStop()
        {
            try
            {
                bot.StopReceiving();
                Program.logger.Info("ChatBot detenido...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }
    }
}

