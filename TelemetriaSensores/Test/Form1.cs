using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openWeather();
        }

        public async void openWeather()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://community-open-weather-map.p.rapidapi.com/weather?q=Quito%2Cec&lat=-0.0882507&lon=-78.4768975&callback=telemetria&id=2172797&lang=Spanish&units=metric&mode=xml"),
                    Headers =
                    {
                        { "x-rapidapi-key", "7ef44ff448msh0f797c63833dd5fp1543f6jsn724640bc3024" },
                        { "x-rapidapi-host", "community-open-weather-map.p.rapidapi.com" },
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
                            float fltValor = float.Parse((string)parsedProperty.Value);
                        }
                        else if (parsedProperty.Name == "humidity")
                        {
                            float fltValor = float.Parse((string)parsedProperty.Value);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
