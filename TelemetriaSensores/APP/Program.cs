using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace APP
{
    class Program
    {
        //Conectamos con el bot de Telegram usando el token recibido al crearlo
        private static readonly TelegramBotClient bot = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramToken"]);
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static string strUrl { get { return ConfigurationManager.AppSettings["URL"]; } }
        private static string strUsuario;

        private static ReplyKeyboardMarkup rkmOpciones = new ReplyKeyboardMarkup();
        private static ReplyKeyboardMarkup rkmConfirmacion = new ReplyKeyboardMarkup();

        private static List<string> strSaludar = ConfigurationManager.AppSettings["saludar"].Split(',').ToList();
        private static List<string> strAceptar = ConfigurationManager.AppSettings["aceptar"].Split(',').ToList();
        private static List<string> strDespedir = ConfigurationManager.AppSettings["despedir"].Split(',').ToList();
        private static List<string> strDetener = ConfigurationManager.AppSettings["detener"].Split(',').ToList();
        private static List<string> strIniciar = ConfigurationManager.AppSettings["iniciar"].Split(',').ToList();
        private static List<string> strActualizar = ConfigurationManager.AppSettings["actualizar"].Split(',').ToList();
        private static List<string> strReservado = ConfigurationManager.AppSettings["reservado"].Split(',').ToList();

        static void Main(string[] args)
        {
            try
            {
                //Escuchamos los mensajes enviados en los grupos donde esté el bot
                var me = bot.GetMeAsync().Result;
                strUsuario = me.Username;
                Console.Title = $"Conectado a bot de Telegram @{strUsuario}...";
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
                //Asignamos los eventos de lectura de mensajes y captura de errores
                bot.OnMessage += onLeerMensajes;
                bot.OnReceiveError += onErrorRecibido;

                //Iniciamos la lectura de mensajes
                bot.StartReceiving(Array.Empty<UpdateType>());
                Console.WriteLine($"Escuchando mensajes de @{strUsuario}...");

                //Si se pulsa INTRO en la consola se detiene la escucha de mensajes
                Console.ReadLine();
                bot.StopReceiving();
            }
            catch (Exception ex)
            {
                logger.Error<Exception>(ex);
            }
        }

        //Evento que lee los mensajes de los grupos donde esté el bot de Telegram        
        private static async void onLeerMensajes(object sender, MessageEventArgs evt)
        {
            try
            {
                var mensaje = evt.Message;

                if (mensaje == null || mensaje.Type != MessageType.Text)
                    return;

                Console.WriteLine($"Mensaje de @{mensaje.Chat.Username}:" + mensaje.Text);

                string strOp = mensaje.Text.ToLower().Split(' ').First();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                switch (strOp)
                {
                    //Según el mensaje leído podremos hacer cualquier tarea
                    case var expression when (strSaludar.Contains(strOp)):
                        sb.AppendFormat("Hola 😊 ‼ yo soy <b><a>@{0}</a></b>.", strUsuario);
                        sb.AppendLine();
                        sb.AppendFormat("En qué te puedo ayudar❓");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, 0, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/5dd/b4d/5ddb4dfa-b3eb-4bfe-8884-9af555e16c6f/192/1.png", true, 0, rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (strAceptar.Contains(strOp)):
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, "😊 En qué más te puedo ayudar❓", ParseMode.Html, false, false, mensaje.MessageId, rkmOpciones, new CancellationToken());
                        break;
                    case var expression when (strDetener.Contains(strOp)):
                        sb.AppendFormat("👌 Las Alertas se encuentran Detenidas 😊.");
                        sb.AppendLine();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        modificarParametro(false);
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/8b8/ab8/8b8ab835-6e72-4fee-8340-73dba6d204d8/192/9.png", false, 0, rkmConfirmacion, new CancellationToken());
                        break;
                    case var expression when (strIniciar.Contains(strOp)):
                        sb.AppendFormat("👌 Las Alertas se encuentran Iniciadas 😊.");
                        sb.AppendLine();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        modificarParametro(true);
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/8b8/ab8/8b8ab835-6e72-4fee-8340-73dba6d204d8/192/9.png", false, 0, rkmConfirmacion, new CancellationToken());
                        break;
                    case var expression when (strActualizar.Contains(strOp)):
                        sb.AppendFormat("👌 Los Parámetros se encuentran Actualizados 😊.");
                        sb.AppendLine();
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        actualizarParametro();
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/8b8/ab8/8b8ab835-6e72-4fee-8340-73dba6d204d8/192/9.png", true, 0, rkmConfirmacion, new CancellationToken());
                        break;
                    case var expression when (strDespedir.Contains(strOp)):
                        await bot.SendTextMessageAsync(mensaje.Chat.Id, "😊 Chao ‼, hasta luego 👋", ParseMode.Html, false, false, mensaje.MessageId, (IReplyMarkup)null, new CancellationToken());
                        await bot.SendStickerAsync(mensaje.Chat.Id, "https://s.tcdn.co/5dd/b4d/5ddb4dfa-b3eb-4bfe-8884-9af555e16c6f/192/1.png", false, 0, new ReplyKeyboardRemove(), new CancellationToken());
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
                logger.Error<Exception>(ex);
            }
        }

        private static async void modificarParametro(bool _boo)
        {
            await new RestClient(strUrl + "modificarParametros/" + _boo.ToString()).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
        }

        private static async void actualizarParametro()
        {
            await new RestClient(strUrl + "eliminarParametros").ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
        }

        private static void onErrorRecibido(object sender, ReceiveErrorEventArgs evt)
        {
            Console.WriteLine("Error recibido: {0} — {1}", evt.ApiRequestException.ErrorCode, evt.ApiRequestException.Message);
            logger.Error<Exception>(evt.ApiRequestException);
        }
    }
}

