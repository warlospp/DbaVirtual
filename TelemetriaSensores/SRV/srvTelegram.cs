using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SRV
{
    partial class srvTelegram : ServiceBase
    {
        private readonly TelegramBotClient bot = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramToken"]);

        private string strUsuario;

        private ReplyKeyboardMarkup rkmOpciones = new ReplyKeyboardMarkup();
        private ReplyKeyboardMarkup rkmConfirmacion = new ReplyKeyboardMarkup();

        private readonly List<string> strSaludar = ConfigurationManager.AppSettings["saludar"].Split(',').ToList();
        private readonly string strStickerSaludar = ConfigurationManager.AppSettings["stickerSaludar"];

        private readonly List<string> strAceptar = ConfigurationManager.AppSettings["aceptar"].Split(',').ToList();
        private readonly string strStickerAceptar = ConfigurationManager.AppSettings["stickerAceptar"];

        private readonly List<string> strDetener = ConfigurationManager.AppSettings["detener"].Split(',').ToList();
        private readonly List<string> strIniciar = ConfigurationManager.AppSettings["iniciar"].Split(',').ToList();
        private readonly List<string> strActualizar = ConfigurationManager.AppSettings["actualizar"].Split(',').ToList();
        private readonly string strStickerOpciones = ConfigurationManager.AppSettings["stickerOpciones"];

        private readonly List<string> strDespedir = ConfigurationManager.AppSettings["despedir"].Split(',').ToList();
        private readonly string strStickerDespedir = ConfigurationManager.AppSettings["stickerDespedir"];
      
        private readonly List<string> strReservado = ConfigurationManager.AppSettings["reservado"].Split(',').ToList();
        private readonly string strStickerError = ConfigurationManager.AppSettings["stickerError"];
        public srvTelegram()
        {
            InitializeComponent();
            rkmOpciones.Keyboard =
               new KeyboardButton[][]
               {
                new KeyboardButton[]
                {
                    new KeyboardButton("Detener ❌"),
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Iniciar ✅")
                },
                new KeyboardButton[]
                {
                    new KeyboardButton("Actualizar ⚠")
                }
               };
            rkmConfirmacion.Keyboard =
            new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton("Si 👍"),
                    new KeyboardButton("No 👎"),
                    new KeyboardButton("Chao 👋")
                }
            };
            Thread thread = new Thread(mensajeria);
            thread.Start();
        }

        public void mensajeria()
        {
            try
            {
                var me = bot.GetMeAsync().Result;
                strUsuario = me.Username;               
                bot.OnMessage += this.onLeerMensajes;
                bot.OnReceiveError += this.onErrorRecibido;
                bot.StartReceiving(Array.Empty<UpdateType>());
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

                string strOp = Regex.Replace(mensaje.Text, @"[^\u0000-\u007F]+", string.Empty).TrimEnd().TrimStart().Split(' ').First().ToLower();
                StringBuilder sb = new StringBuilder();
                switch (strOp)
                {
                    case var expression when (strSaludar.Contains(strOp)):
                        sb.AppendFormat("Hola 😊 ‼ yo soy <b><a>@{0}</a></b>.", strUsuario);
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerSaludar, true, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        sb = new StringBuilder();
                        sb.AppendFormat("En qué te puedo ayudar❓");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, true, 0, rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (strAceptar.Contains(strOp)):
                        sb.AppendFormat("😊 En qué más te puedo ayudar❓");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, true, 0, rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (strDetener.Contains(strOp)):
                        this.modificarParametro(false);
                        sb.AppendFormat("👌 Las Alertas se encuentran Detenidas 😊.");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerAceptar, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        sb = new StringBuilder();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, false, 0, rkmConfirmacion, new CancellationToken());
                        break;
                    case var expression when (strIniciar.Contains(strOp)):
                        this.modificarParametro(true);
                        sb.AppendFormat("👌 Las Alertas se encuentran Iniciadas 😊.");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerAceptar, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        sb = new StringBuilder();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, false, 0, rkmConfirmacion, new CancellationToken());
                        break;
                    case var expression when (strActualizar.Contains(strOp)):
                        this.actualizarParametro();
                        sb.AppendFormat("👌 Los Parámetros se encuentran Actualizados 😊.");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerAceptar, true, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        sb = new StringBuilder();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, true, 0, rkmConfirmacion, new CancellationToken());
                        break;
                    case var expression when (strDespedir.Contains(strOp)):
                        sb.AppendFormat("😊 Chao ‼, hasta luego 👋");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerDespedir, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        break;
                    case var expression when (strReservado.Contains(strOp)):
                        sb.AppendFormat("😪 Con eso no te puedo ayudar.");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerError, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        sb = new StringBuilder();
                        sb.AppendFormat("Intenta con algunas de estas opciones 🙏");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, false, 0, rkmOpciones, new CancellationToken());
                        break;
                    default:
                        sb.AppendFormat("😪 Con eso no te puedo ayudar.");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerError, false, 0, new ReplyKeyboardRemove(), new CancellationToken());
                        sb = new StringBuilder();
                        sb.AppendFormat("Intenta con algunas de estas opciones 🙏");
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, this.strStickerOpciones, false, 0, rkmOpciones, new CancellationToken());
                        break;
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        private async void modificarParametro(bool _boo)
        {
            await new RestClient(Program.strUrl + "modificarParametros/" + _boo.ToString()).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
        }

        private async void actualizarParametro()
        {
            await new RestClient(Program.strUrl + "eliminarParametros").ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
        }

        private void onErrorRecibido(object sender, ReceiveErrorEventArgs evt)
        {
            Program.logger.Error<Exception>(evt.ApiRequestException);
        }

        protected override void OnStart(string[] args)
        {

        }

        protected override void OnStop()
        {
            try
            {
                bot.StopReceiving();
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }
    }   
}
