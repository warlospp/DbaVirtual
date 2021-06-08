using CMN;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Configuration;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace SRV
{
    public partial class srvTelemetria : ServiceBase
    {
        private System.Timers.Timer tmr;

        private string strIntervalo
        {
            get
            {
                return ConfigurationManager.AppSettings["intervalo"];
            }
        }

        private string strRapidApi
        {
            get
            {
                return ConfigurationManager.AppSettings["RapidApi"];
            }
        }

        private string strRapidKey
        {
            get
            {
                return ConfigurationManager.AppSettings["RapidKey"];
            }
        }

        private string strRapidHost
        {
            get
            {
                return ConfigurationManager.AppSettings["RapidHost"];
            }
        }

        public srvTelemetria()
        {
            
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                new RestClient(Program.strUrl + "eliminarParametros").ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
                Program.logger.Warn("Eliminación de Parametrizaciones");
                this.tmr = new System.Timers.Timer();
                string[] strArray = this.strIntervalo.Split('.');
                this.tmr.Enabled = true;
                this.tmr.Interval = strArray[1] == cmnIntervalos.Minuto ? (double)(int.Parse(strArray[0]) * 60000) : (strArray[1] == cmnIntervalos.Segundo ? (double)(int.Parse(strArray[0]) * 1000) : (strArray[1] == cmnIntervalos.Milisegundo ? (double)int.Parse(strArray[0]) : 0.0));
                this.tmr.Elapsed += new ElapsedEventHandler(this.OnEnviarAlerta);
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

        private async void OnEnviarAlerta(object sender, ElapsedEventArgs e)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(this.strRapidApi),
                    Headers =
                    {
                        { "x-rapidapi-key", this.strRapidKey },
                        { "x-rapidapi-host", this.strRapidHost },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    body = body.Replace("telemetria(", "").Replace(")", "");
                    JObject obj = JObject.Parse(body);
                    JObject subObjs = (JObject)obj["main"];
                    foreach (JProperty parsedProperty in subObjs.Properties())
                    {
                        if (parsedProperty.Name == "feels_like")
                        {
                            string strValor = (string)parsedProperty.Value;
                            await new RestClient(Program.strUrl + "insertarAlerta/3/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss.fff") + "/" + strValor).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
                        }
                        else if (parsedProperty.Name == "humidity")
                        {
                            string strValor = (string)parsedProperty.Value;
                            await new RestClient(Program.strUrl + "insertarAlerta/4/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss.fff") + "/" + strValor).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
                        }
                        else if (parsedProperty.Name == "humidity")
                        {
                            string strValor = (string)parsedProperty.Value;
                            await new RestClient(Program.strUrl + "insertarAlerta/4/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss.fff") + "/" + strValor).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
                        }
                        else if (parsedProperty.Name == "pressure")
                        {
                            string strValor = (string)parsedProperty.Value;
                            await new RestClient(Program.strUrl + "insertarAlerta/5/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss.fff") + "/" + strValor).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
                        }
                    }                   
                }
                Program.logger.Info("Inserción de Alerta");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnStop()
        {
            try
            {
                this.tmr.Enabled = false;
                this.tmr.Dispose();
            }
            catch (Exception ex)
            {
                Program.logger.Error<Exception>(ex);
            }
        }

    }
}