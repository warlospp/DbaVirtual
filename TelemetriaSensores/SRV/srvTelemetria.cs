using CMN;
using RestSharp;
using System;
using System.Configuration;
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
                await new RestClient(Program.strUrl + "insertarAlerta/3/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss.fff") + "/" + ((double)new Random().Next(13, 37) + 0.2).ToString()).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
                await new RestClient(Program.strUrl + "insertarAlerta/4/" + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss.fff") + "/" + ((double)new Random().Next(12, 38) + 0.1).ToString()).ExecuteAsync((IRestRequest)new RestRequest(), new CancellationToken());
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