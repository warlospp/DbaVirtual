using System;
using System.Collections.Generic;
using DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DAL
{
    public class dalMensaje : Conexiones.dalMsSql,IDisposable
    {
        private string strConn { get; set; }
        public dalMensaje(string _strConn)
        {
            this.strConn = _strConn;
        }
        public List<dtoMensaje> ejecutarQuery(string _strQuery)
        {
            List<dtoMensaje> dtos = new List<dtoMensaje>();
            try
            {
                string strJson = JsonConvert.SerializeObject(this.query(this.strConn, _strQuery));
                JArray jsonArray = JArray.Parse(strJson);           
                foreach (JObject parsedObject in jsonArray.Children<JObject>())
                {
                    string strId = parsedObject.Path.Replace("[", "").Replace("]", "");
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                       dtos.Add(new dtoMensaje()
                       { 
                           intId = int.Parse(strId), 
                           strKey = parsedProperty.Name ,
                           strValue = (string)parsedProperty.Value 
                       });                       
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtos;  
        }

    }
}
