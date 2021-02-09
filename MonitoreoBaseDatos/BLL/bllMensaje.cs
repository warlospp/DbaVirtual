using System;
using DTO;
using DAL;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace BLL
{
    public class bllMensaje : IDisposable
    {
        private string strConn { get; set; }

        public bllMensaje(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoMensaje> ejecutar(string _strQuery, string _strPlatilla)
        {
            List<dtoMensaje> dtos = new List<dtoMensaje>();
            try
            {
                using (dalMensaje dal = new dalMensaje(this.strConn))
                {
                    var tmp = dal.ejecutarQuery(_strQuery);
                    using (StreamReader file = File.OpenText(_strPlatilla))
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject obj = (JObject)JToken.ReadFrom(reader);
                        foreach (var item in tmp)
                        {
                            foreach (JProperty parsedProperty in obj.Properties())
                            {
                                string strValor = (string)parsedProperty.Value;
                                if ("@" + item.strKey == strValor)
                                {
                                    dtos.Add(new dtoMensaje()
                                    {
                                        intId = item.intId,
                                        strKey = parsedProperty.Name,
                                        strValue = item.strValue
                                    });
                                }
                            }
                        }
                    };
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtos;
        }
        public void Dispose()
        {
            
        }
    }
}
