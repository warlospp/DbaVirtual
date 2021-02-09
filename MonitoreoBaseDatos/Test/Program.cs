using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BLL;
using DTO;

namespace Test
{
    class Program
    {
        private static TelegramBotClient bot = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramToken"]);
        private static string strTelegramId
        {
            get
            {
                return ConfigurationManager.AppSettings["TelegramId"];
            }
        }

        private static string strConnLocal
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["dbLocal"].ConnectionString;
            }
        }

        private static string strConnRemoto
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["dbRemoto"].ConnectionString;
            }
        }
        
        private static string strUsuario;

        private static ReplyKeyboardMarkup rkmOpciones = new ReplyKeyboardMarkup();
        private static List<dtoOpcion> dtosOpcion = new List<dtoOpcion>();

        private static ReplyKeyboardMarkup rkmInstancias = new ReplyKeyboardMarkup();
        private static List<dtoInstancia> dtosInstancia = new List<dtoInstancia>();

        private static ReplyKeyboardMarkup rkmConfirmacion = new ReplyKeyboardMarkup();

        private static List<string> strSaludar = ConfigurationManager.AppSettings["saludar"].Split(',').ToList();
        private static List<string> strAceptar = ConfigurationManager.AppSettings["aceptar"].Split(',').ToList();
        private static List<string> strDespedir = ConfigurationManager.AppSettings["despedir"].Split(',').ToList();
        private static string strDirectorio = ConfigurationManager.AppSettings["directorio"].ToString();

        private static List<dtoMensaje> dtosMensaje = new List<dtoMensaje>(); 
        private static string strInstancia = string.Empty;
        private static string strOpcion = string.Empty;
        private static string strMensaje = string.Empty;
        private static string strSentencia = string.Empty;
        private static string strPlantilla = string.Empty;

        static void Main(string[] args)
        {
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();

            rkmOpciones.ResizeKeyboard = true;
            using (bllOpcion bll = new bllOpcion(strConnLocal))
            {
                dtosOpcion = bll.ejecutar();
                for (var Index = 0; Index <= dtosOpcion.Count; Index++)
                {
                    cols.Add(new KeyboardButton(dtosOpcion.Where(x => x.intOpcion == Index).Select(x => x.strNombre).FirstOrDefault()));
                    //if (Index % 3 != 0) 
                    //    continue;
                    rows.Add(cols.ToArray());
                    cols = new List<KeyboardButton>();
                }
            }        
            rkmOpciones.Keyboard = rows.ToArray();
                        
            rows = new List<KeyboardButton[]>();
            cols = new List<KeyboardButton>();

            rkmInstancias.ResizeKeyboard = true;
            using (bllInstancia bll = new bllInstancia(strConnLocal))
            {
                dtosInstancia = bll.ejecutar();
                foreach (var dto in dtosInstancia)
                {
                    cols.Add(new KeyboardButton(dto.strAlias));
                    rows.Add(cols.ToArray());
                    cols = new List<KeyboardButton>();
                }
            }
            rkmInstancias.Keyboard = rows.ToArray();

            rkmConfirmacion.OneTimeKeyboard = true;
            rkmConfirmacion.ResizeKeyboard = true;
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


            //Thread thread = new Thread(mensajeria);
            //thread.Start();


            var me = bot.GetMeAsync().Result;
            strUsuario = me.Username;
            Console.Title = "Conectado a bot de Telegram " + strUsuario;

            //Asignamos los eventos de lectura de mensajes y captura de errores
            bot.OnMessage += onLeerMensajes;
            bot.OnReceiveError += onErrorRecibido;

            //Iniciamos la lectura de mensajes
            bot.StartReceiving(Array.Empty<UpdateType>());
            
            Console.WriteLine($"Escuchando mensajes de @{strUsuario}");

            //Si se pulsa INTRO en la consola se detiene la escucha de mensajes
            Console.ReadLine();
            bot.StopReceiving();
        }

        private static string strSaludo()
        {
            int intHora = DateTime.Now.Hour;
            string strSaludo = intHora >= 12 && intHora <= 18 ? "Buenas Tardes"
                                : intHora >= 19 && intHora <= 23 ? "Buenas Noches"
                                    : "Buenos Días";
            return strSaludo;
        }

        private static async void onLeerMensajes(object sender, MessageEventArgs evt)
        {
            try
            {
                var mensaje = evt.Message;

                if (mensaje == null || mensaje.Type != MessageType.Text)
                    return;

                strOpcion = mensaje.Text.ToLower().Replace(".", string.Empty).Split(' ').First();
 
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                switch (strOpcion)
                {
                    case var expression when (strSaludar.Contains(strOpcion)):
                        strInstancia = string.Empty;
                        strOpcion = string.Empty;
                        strMensaje = string.Empty;
                        strSentencia = string.Empty;
                        strPlantilla = string.Empty;
                        sb.AppendFormat("Hola " + strSaludo() + " 😊 ‼ yo soy <b><a>@{0}</a></b>.", strUsuario);
                        sb.AppendLine();
                        sb.AppendFormat("En qué te puedo ayudar❓");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)strTelegramId, sb.ToString(), ParseMode.Html, false, false, 0, rkmOpciones, new CancellationToken());
                        await bot.SendStickerAsync((ChatId) strTelegramId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/1.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strAceptar.Contains(strOpcion)):
                        strInstancia = string.Empty;
                        strOpcion = string.Empty;
                        strMensaje = string.Empty;
                        strSentencia = string.Empty;
                        strPlantilla = string.Empty;
                        await bot.SendTextMessageAsync((ChatId)strTelegramId, "😊 En qué más te puedo ayudar❓", ParseMode.Html, false, false, mensaje.MessageId, rkmOpciones, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/8.webp", false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (dtosOpcion.Where(x => x.strKeyword.Split(",").Contains(strOpcion.ToLower())).Count() >= 1 ? true : false):
                        var tmpOpcion = dtosOpcion.Where(x => x.strKeyword.Split(",").Contains(strOpcion.ToLower()));
                        strOpcion = tmpOpcion.Select(x => x.strNombre).FirstOrDefault();
                        strMensaje = tmpOpcion.Select(x => x.strMensaje).FirstOrDefault();
                        strSentencia = tmpOpcion.Select(x => x.strSentencia).FirstOrDefault(); 
                        strPlantilla = tmpOpcion.Select(x => x.strPlantilla).FirstOrDefault();
                        strInstancia = strOpcion;
                        sb.AppendLine();
                        sb.AppendFormat("👇 Selecciona la Instancia 👇");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)strTelegramId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmInstancias, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/10.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (dtosInstancia.Where(x => x.strAlias == strOpcion).Count() >= 1 ? true : false):
                        string str = strInstancia;
                        strInstancia = strOpcion;
                        strOpcion = str;
                        var tmpInstancia = dtosInstancia.Where(x => x.strAlias == strInstancia);
                        sb.AppendFormat("👇 " + strMensaje + ", de la Instancia @" + strInstancia + "  👇");
                        sb.AppendLine();
                        using (bllMensaje bll = new bllMensaje(strConnRemoto.Replace("@instancia", tmpInstancia.Select(x => x.strInstancia).FirstOrDefault())))
                        {
                            string strArchivo = (strDirectorio.EndsWith(@"\")
                                                    ? strDirectorio
                                                    : strDirectorio.Concat(@"\")).ToString()
                                                + strPlantilla;
                            dtosMensaje = bll.ejecutar(strSentencia, strArchivo);
                        };
                        int intMax = dtosMensaje.Select(x => x.intId).Max();
                        for (int i = 0; i < intMax; i++)
                        {
                            var tmpMensaje = dtosMensaje.Where(x => x.intId == i);
                            foreach (var item in tmpMensaje)
                            {
                                sb.AppendFormat(item.strKey + " " + item.strValue);
                                sb.AppendLine();
                            }
                            sb.AppendLine();
                        }                        
                        sb.AppendFormat("Te puedo ayudar con algo más❓");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)strTelegramId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmConfirmacion, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/11.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    case var expression when (strDespedir.Contains(strOpcion)):
                        strInstancia = string.Empty;
                        strOpcion = string.Empty;
                        sb.AppendLine();
                        sb.AppendFormat("😊 Gracias ‼, vuelve pronto");
                        sb.AppendLine();
                        sb.AppendFormat(strSaludo() + "👋");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)strTelegramId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, new ReplyKeyboardRemove(), new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/2.webp", true, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                    default:
                        strInstancia = string.Empty;
                        strOpcion = string.Empty;
                        sb.AppendFormat("😪 Con eso no te puedo ayudar.");
                        sb.AppendLine();
                        sb.AppendFormat("Intenta con algunas de estas opciones 🙏");
                        sb.AppendLine();
                        await bot.SendTextMessageAsync((ChatId)strTelegramId, sb.ToString(), ParseMode.Html, false, false, mensaje.MessageId, rkmOpciones, new CancellationToken());
                        await bot.SendStickerAsync((ChatId)strTelegramId, "https://tlgrm.es/_/stickers/eb5/41e/eb541eba-3be4-3bea-bd7f-5e487503be39/4.webp", false, 0, (IReplyMarkup)null, new CancellationToken());
                        break;
                }
                Console.WriteLine(strOpcion);
            }
            catch (Exception ex)
            {
// Program.logger.Error<Exception>(ex);
            }
        }



        private static void onErrorRecibido(object sender, ReceiveErrorEventArgs evt)
        {
            //Program.logger.Error<Exception>(evt.ApiRequestException);
        }
    }
}
