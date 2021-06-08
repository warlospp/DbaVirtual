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

        public bllMensaje()
        {
           
        }

        public bllMensaje(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoMensaje> ejecutar(string _strPlatilla)
        {
            List<dtoMensaje> dtos = new List<dtoMensaje>();
            try
            {
                using (StreamReader file = File.OpenText(_strPlatilla))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        JObject obj = (JObject)JToken.ReadFrom(reader);
                        foreach (JProperty parsedProperty in obj.Properties())
                        {
                            dtos.Add(new dtoMensaje()
                            {
                                intId = 0,
                                strKey = parsedProperty.Name,
                                strValue = (string)parsedProperty.Value
                            });
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

        public List<dtoMensaje> ejecutar(List<dtoMensaje> _dtos, string _strQuery)
        {
            List<dtoMensaje> dtos = new List<dtoMensaje>();
            try
            {
                using (dalMensaje dal = new dalMensaje(this.strConn))
                {
                    List<dtoMensaje> tmp = dal.ejecutarQuery(_strQuery);
                    foreach (var item in tmp)
                    {
                        foreach (var obj in _dtos)
                        {
                            string strItem = "@" + item.strKey;
                            if (obj.strValue.Contains(strItem))
                            {
                                dtos.Add(new dtoMensaje()
                                {
                                    intId = item.intId,
                                    strKey = obj.strKey,
                                    strValue = obj.strValue.Replace(strItem,item.strValue)
                                });
                            }
                        }
                    }                    
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
