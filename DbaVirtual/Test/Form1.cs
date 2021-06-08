using DTO;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
            string strPhyton = @"C:/ProgramData/Anaconda3/python.exe";
            string strScript = "C://Phyton//books_read.py";
            string strArgs = "abc";
            //this.PatchParameter(strScript, strArgs);
            //this.run_cmd(strPhyton, strScript , strArgs);
            this.intencionLUIS("ayuda");
        }

        private void intencionLUIS(string _strMensaje)
        {
            string strIntencion = string.Empty;
            List<dtoMensaje> dtos = new List<dtoMensaje>();
            try
            {

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://azpbo-bdd-lu.cognitiveservices.azure.com/luis/prediction/v3.0/apps/2a8192ca-ca21-4850-a491-3b04e21eed78/slots/production/predict?subscription-key=d787fc0e06fc4e92a13c958216fc4f81&verbose=true&show-all-intents=true&log=true&query=" + _strMensaje);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            var tmp = dtos.Where(x => float.Parse(x.strValue) >= 0);
            if (tmp.Count() >= 1)
            {
                strIntencion = tmp.OrderByDescending(x => float.Parse(x.strValue)).Select(x => x.strKey).FirstOrDefault();
                string strScore = tmp.OrderByDescending(x => float.Parse(x.strValue)).Select(x => x.strValue).FirstOrDefault();
            }
        }

        private void run_cmd(string strPhyton, string strScript, string strArgs)
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = strPhyton;//cmd is full path to python.exe
                start.Arguments = string.Format("\"{0}\" \"{1}\"", strScript, strArgs);
                start.UseShellExecute = false;
                start.CreateNoWindow = true; // We don't need new window
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                        string result = reader.ReadToEnd();
                        MessageBox.Show(stderr + " " + result);
                        Console.Write(stderr + " " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PatchParameter(string strScript, string strArgs)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine(); // Extract Python language engine from their grasp
                ScriptScope scope = engine.CreateScope(); // Introduce Python namespace (scope)
                                                  //var d = new Dictionary<string, object>
                                                  //{
                                                  //   { "var", strArgs}
                                                  //}; // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability
                var d = strArgs;
                scope.SetVariable("parametro_a", d); // This will be the name of the dictionary in python script, initialized with previously created .NET Dictionary
                var searchPaths = engine.GetSearchPaths();
                searchPaths.Add(@"C:/ProgramData/Anaconda3/Lib/site-packages/numpy/");
                engine.SetSearchPaths(searchPaths);
                ScriptSource source = engine.CreateScriptSourceFromFile(strScript); // Load the script
                object result = source.Execute(scope);
                string strArgs2 = scope.GetVariable<string>("parametro_a"); // To get the finally set variable 'parameter' from the python script
                var strArgs3 = scope.GetItems();



                MessageBox.Show(strArgs2);     
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);;
            }
  
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
