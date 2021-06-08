using DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
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

        public float fltPrecision { get; set; }

        private System.Timers.Timer tmr;
        private float fltTiempoInactividad { get; set; }

        private bool booSticker { get; set; }

        private string strStickerSaludar
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerSaludar"];
            }
        }

        private string strStickerDespedir
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerDespedir"];
            }
        }

        private string strStickerAgradecer
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerAgredecer"];
            }
        }       

        private string strStickerOpciones
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerOpciones"];
            }
        }
                
        private string strStickerInstancias
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerInstancias"];
            }
        }        

        private string strStickerProcesado
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerProcesado"];
            }
        }

        private string strStickerInactividad
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerInactividad"];
            }
        }
        
        private string strStickerNone
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerNone"];
            }
        }
        private string strStickerError
        {
            get
            {
                return ConfigurationManager.AppSettings["StickerError"];
            }
        }

        public string strBot { get; set; }

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
            this.cargarConfiguraciones();
            this.cargarMenus();
            if (this.strBot == string.Empty)
                this.strBot = "Db_produbot";              
            Thread thread = new Thread(listenerMensajeria);
            thread.Start();
        }

        private void cargarConfiguraciones()
        {
            try
            {  
                this.fltPrecision = float.Parse(ConfigurationManager.AppSettings["Precision"]);
                this.fltTiempoInactividad = float.Parse(ConfigurationManager.AppSettings["TiempoInactividad"]);
                this.booSticker = bool.Parse(ConfigurationManager.AppSettings["Sticker"]);      
                Program.logger.Info("Configuraciones Cargadas...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
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
                this.strBot = me.Username;     
                bot.OnMessage += onLeerMensajes;
                bot.OnReceiveError += onErrorRecibido;
                bot.StartReceiving(Array.Empty<UpdateType>());
                Program.logger.Info("Escuchando ChatBot " + this.strBot + " ...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        private async void onLeerMensajes(object sender, MessageEventArgs evt)
        {
            var mensaje = evt.Message;

            if (mensaje == null || mensaje.Type != MessageType.Text)
                return;

            this.tmr.Stop();
            this.tmr.Start();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            try
            {
                mensaje.Text = Regex.Replace(mensaje.Text, @"[^\u0000-\u007F]+", string.Empty).TrimEnd().TrimStart();
                string strIntencion =  this.intencionLUIS(mensaje.Text.ToLower());
                string strUsuario = mensaje.From.FirstName + mensaje.From.LastName;
                switch (strIntencion)
                {
                    case "saludar":
                        this.saludar(strUsuario);
                        break;
                    case "opciones":
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendLine();                        
                        sb.AppendFormat("👇 Te puedo ayudar con una de estas opciones 😊");
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, this.rkmOpciones, new CancellationToken());
                        if (this.booSticker)
                            await bot.SendStickerAsync((ChatId)this.strTelegramChatId, this.strStickerOpciones, false, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, "Selecciona por favor 🙏", ParseMode.Html, false, false, 0, this.rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (this.dtosOpcion.Where(x => x.strNombre.ToLower() == strIntencion).Count() >= 1 ? true : false):
                        this.strOpcion = strIntencion;
                        this.strInstancia = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("👇 Selecciona la Instancia 👇");
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmInstancias, new CancellationToken());
                        if(this.booSticker)
                            await bot.SendStickerAsync((ChatId)strTelegramChatId, this.strStickerInstancias, true, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, "👇👇👇👇👇👇👇👇👇👇👇👇👇👇", ParseMode.Html, false, false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (this.dtosInstancia.Where(x => x.strAlias == strIntencion).Count() >= 1 ? true : false):
                        string strMensaje = string.Empty;
                        this.strInstancia = strIntencion;       
                        dtoInstancia tmpInstancia = this.dtosInstancia.Where(x => x.strAlias.ToLower() == this.strInstancia).FirstOrDefault();
                        dtoOpcion tmpOpcion = this.dtosOpcion.Where(x => x.strNombre.ToLower() == this.strOpcion).FirstOrDefault();
                        using (srProxy.IwcfChatBotClient proxy = new srProxy.IwcfChatBotClient())
                        {            
                            strMensaje = proxy.cargarMensaje(tmpOpcion, tmpInstancia, strUsuario);
                            proxy.Close();
                        };
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, strMensaje, ParseMode.Html, false, false, mensaje.MessageId, this.rkmInteraccion, new CancellationToken());
                        if (this.booSticker)
                            await bot.SendStickerAsync((ChatId)strTelegramChatId, this.strStickerProcesado, true, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, "❓❓❓❓❓❓❓❓❓❓❓❓❓❓", ParseMode.Html, false, false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case "despedir":
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("😊 Gracias ‼, vuelve pronto @" + strUsuario);
                        sb.AppendLine();
                        sb.AppendLine();
                        sb.AppendFormat(this.strSaludo(true) + "👋");
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        if (this.booSticker)
                            await bot.SendStickerAsync((ChatId)this.strTelegramChatId, this.strStickerDespedir, true, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, "Cuidateeeee‼️‼️‼️‼️‼️‼️", ParseMode.Html, false, false, 0, (IReplyMarkup) null, new CancellationToken());
                        break;
                    case "agradecer":
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("😊 Gracias a ti @" + strUsuario + "‼, es un placer ayudarte");
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        if (this.booSticker)
                            await bot.SendStickerAsync((ChatId)strTelegramChatId, this.strStickerAgradecer, true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case "reconfigurar":
                        this.cargarConfiguraciones();
                        this.cargarMenus();
                        sb.AppendLine();
                        sb.AppendFormat("👌 Las configuraciones están actualizadas 👍");
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                        if (this.booSticker)
                            await bot.SendStickerAsync((ChatId)strTelegramChatId, this.strStickerProcesado, true, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)strTelegramChatId, "😊 En qué más te puedo ayudar❓", ParseMode.Html, false, false, 0, this.rkmInteraccion, new CancellationToken());
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        break;
                    default:
                        this.strInstancia = string.Empty;
                        this.strOpcion = string.Empty;
                        sb.AppendFormat("😪 Con eso no te puedo ayudar 😵");
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                        if (this.booSticker)
                            await bot.SendStickerAsync((ChatId)this.strTelegramChatId, this.strStickerNone, false, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, "Intenta con algunas de estas opciones 🙏", ParseMode.Html, false, false, 0, this.rkmOpciones, new CancellationToken());
                        break;
                }
                Program.logger.Info("Mensaje leído: Usuario[{0}] Intencion [{1}] Opcion [{2}] Instancia [{3}]", strUsuario, strIntencion, this.strOpcion,this.strInstancia);
            }
            catch (Exception ex)
            {
                this.strInstancia = string.Empty;
                this.strOpcion = string.Empty;
                sb.AppendFormat("😪 Se ha producido un error interno 😵");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("<code>" + ex.Message +"</code>");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("Por favor intenta más tarde 🙏");
                sb.AppendLine();
                await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                if (this.booSticker)
                    await bot.SendStickerAsync((ChatId)this.strTelegramChatId, this.strStickerError, false, 0, (IReplyMarkup)null, new CancellationToken());
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
                    Program.logger.Info("Mensaje[{0}] Intencion[{1}] Score[1]", _strMensaje, strIntencion);
                }
                else
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.strLUISURL + _strMensaje);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "GET";
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (StreamReader file = new StreamReader(httpResponse.GetResponseStream()))
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
                                        float fltValor = float.Parse((string)parsedValue.Value);
                                        if (fltValor >= Math.Round(this.fltPrecision / 10, 1))
                                        {
                                            dtos.Add(new dtoMensaje()
                                            {
                                                intId = 0,
                                                strKey = strNombre,
                                                strValue = fltValor.ToString()
                                            });
                                        }
                                    }
                                }
                            }
                        };
                    };
                    if (dtos.Count() >= 1)
                    {
                        strIntencion = dtos.OrderByDescending(x => float.Parse(x.strValue)).Select(x => x.strKey).FirstOrDefault();
                        string strScore = dtos.OrderByDescending(x => float.Parse(x.strValue)).Select(x => x.strValue).FirstOrDefault();
                        Program.logger.Info("Mensaje[{0}] Intencion[{1}] Score[{2}]", _strMensaje, strIntencion, strScore);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
            return strIntencion.ToLower();
        }

        private async void saludar(string _strUsuario)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                this.strInstancia = string.Empty;
                this.strOpcion = string.Empty;
                sb.AppendFormat("Hola @" + _strUsuario + " " + this.strSaludo(false) + " 😊 ‼ yo soy <b><a>@{0}</a></b>.", this.strBot );
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("En qué te puedo ayudar❓");
                sb.AppendLine();
                await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, 0, this.rkmInteraccion, new CancellationToken());
                if (this.booSticker)
                    await bot.SendStickerAsync((ChatId)this.strTelegramChatId, this.strStickerSaludar, true, 0, (IReplyMarkup)null, new CancellationToken());
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
            this.tmr = new System.Timers.Timer();
            this.tmr.Enabled = true;
            this.tmr.AutoReset = true;
            this.tmr.Interval = this.fltTiempoInactividad * 3600000;
            this.tmr.Elapsed += new ElapsedEventHandler(this.OnEnviarMensajeInactividadAsync);
            this.tmr.Start();
            this.saludar("Dba");            
        }

        private async void OnEnviarMensajeInactividadAsync(object sender, ElapsedEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                this.strInstancia = string.Empty;
                this.strOpcion = string.Empty;
                sb.AppendFormat("😴😴😴😴😴😴😴😴😴😴😴😴😴😴");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("🥱Estoy aburrido🥱");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("Te puedo ayudar en algo❓");
                sb.AppendLine();
                await bot.SendTextMessageAsync((ChatId)this.strTelegramChatId, sb.ToString(), ParseMode.Html, false, false, 0, this.rkmInteraccion, new CancellationToken());
                if (this.booSticker)
                    await bot.SendStickerAsync((ChatId)this.strTelegramChatId, this.strStickerInactividad, true, 0, (IReplyMarkup)null, new CancellationToken());
                Program.logger.Info("Enviando Mensaje Inactividad...");
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                this.tmr.Enabled = false;
                this.tmr.Dispose();
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
