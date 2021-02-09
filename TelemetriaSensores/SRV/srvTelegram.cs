using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
        private readonly List<string> strAceptar = ConfigurationManager.AppSettings["aceptar"].Split(',').ToList();
        private readonly List<string> strDespedir = ConfigurationManager.AppSettings["despedir"].Split(',').ToList();
        private readonly List<string> strDetener = ConfigurationManager.AppSettings["detener"].Split(',').ToList();
        private readonly List<string> strIniciar = ConfigurationManager.AppSettings["iniciar"].Split(',').ToList();
        private readonly List<string> strActualizar = ConfigurationManager.AppSettings["actualizar"].Split(',').ToList();
        private readonly List<string> strReservado = ConfigurationManager.AppSettings["reservado"].Split(',').ToList();

        public srvTelegram()
        {
            InitializeComponent();
            rkmOpciones.Keyboard =
               new KeyboardButton[][]
               {
                new KeyboardButton[]
                {
                    new KeyboardButton("Detener ❌"),
                    new KeyboardButton("Iniciar ✅"),
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
                bot.OnMessage += onLeerMensajes;
                bot.OnReceiveError += onErrorRecibido;
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

                string strOp = mensaje.Text.ToLower().Split(' ').First();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                switch (strOp)
                {
                    case var expression when (strSaludar.Contains(strOp)):
                        sb.AppendFormat("Hola 😊 ‼ yo soy <b><a>@{0}</a></b>.", strUsuario);
                        sb.AppendLine();
                        sb.AppendFormat("En qué te puedo ayudar❓");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, 0, rkmOpciones, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/5dd/b4d/5ddb4dfa-b3eb-4bfe-8884-9af555e16c6f/192/1.png", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strAceptar.Contains(strOp)):
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, "😊 En qué más te puedo ayudar❓", ParseMode.Html, false, false, mensaje.MessageId, rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (strDetener.Contains(strOp)):
                        sb.AppendFormat("👌 Las Alertas se encuentran Detenidas 😊.");
                        sb.AppendLine();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        this.modificarParametro(false);
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmConfirmacion, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/8b8/ab8/8b8ab835-6e72-4fee-8340-73dba6d204d8/192/9.png", false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strIniciar.Contains(strOp)):
                        sb.AppendFormat("👌 Las Alertas se encuentran Iniciadas 😊.");
                        sb.AppendLine();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        this.modificarParametro(true);
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmConfirmacion, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/8b8/ab8/8b8ab835-6e72-4fee-8340-73dba6d204d8/192/9.png", false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strActualizar.Contains(strOp)):
                        sb.AppendFormat("👌 Los Parámetros se encuentran Actualizados 😊.");
                        sb.AppendLine();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        this.actualizarParametro();
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmConfirmacion, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/8b8/ab8/8b8ab835-6e72-4fee-8340-73dba6d204d8/192/9.png", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strDespedir.Contains(strOp)):
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, "😊 Chao ‼, hasta luego 👋", ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/5dd/b4d/5ddb4dfa-b3eb-4bfe-8884-9af555e16c6f/192/1.png", false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strReservado.Contains(strOp)):
                        break;
                    default:
                        sb.AppendFormat("😪 Con eso no te puedo ayudar.");
                        sb.AppendLine();
                        sb.AppendFormat("Intenta con algunas de estas opciones 🙏");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmOpciones, new CancellationToken());
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
