using CMN;
using DAL.Conexiones;
using DTO;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DAL
{
    public class dalSensor : IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public dalSensor(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        public List<dtoSensor> consultar()
        {
            List<dtoSensor> dtos = new List<dtoSensor>();
            try
            {
                using (dalRedis dalRedis = new dalRedis(this.dic))
                {
                    List<HashEntry> source = dalRedis.consultar(this.GetType().Name);
                    if (source.Count<HashEntry>() <= 0)
                    {
                        dtos = this.ejecutar();
                        Dictionary<int, string> _dic = new Dictionary<int, string>();
                        foreach (dtoSensor item in dtos)
                        {
                            string str = JsonConvert.SerializeObject((object)item);
                            _dic.Add(item.intIdSensor, str);
                        }
                        dalRedis.insertar(_dic, this.GetType().Name);
                    }
                    else
                    {
                        foreach (HashEntry hashEntry in source)
                        {
                            dtoSensor item = JsonConvert.DeserializeObject<dtoSensor>((string)hashEntry.Value);
                            dtos.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtos;
        }

        private List<dtoSensor> ejecutar()
        {
            try
            {
                DataTable source = new DataTable();
                string str = this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.ConectorSql)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>();
                string _strConn = this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.Sql)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>();
                if (str == cmnConectores.MySql)
                {
                    using (dalMySql dalMySql = new dalMySql())
                        source = dalMySql.ejecutar(_strConn, cmnObjetos.ConsultarSensor);
                }
                else if (str == cmnConectores.PostgreSql)
                {
                    using (dalPostgreSql dalPostgreSql = new dalPostgreSql())
                        source = dalPostgreSql.ejecutar(_strConn, cmnObjetos.ConsultarSensor);
                }
                else if (str == cmnConectores.MsSql)
                {
                    using (dalMsSql dalMsSql = new dalMsSql())
                        source = dalMsSql.ejecutar(_strConn, cmnObjetos.ConsultarSensor);
                }
                return source.AsEnumerable().Select<DataRow, dtoSensor>((Func<DataRow, dtoSensor>)(dr => new dtoSensor()
                {
                    intIdSensorTipo = dr.Field<int>("ID_SENSOR_TIPO"),
                    strDispositivo = dr.Field<string>("DISPOSITIVO"),
                    strIP = dr.Field<string>("IP"),
                    strDescripcion = dr.Field<string>("DESCRIPCION"),
                    strDireccion = dr.Field<string>("DIRECCION"),
                    intIdSensor = dr.Field<int>("ID_SENSOR"),
                    strSensor = dr.Field<string>("SENSOR"),
                    strTipo = dr.Field<string>("TIPO"),
                    strUnidadMedida = dr.Field<string>("UNIDAD_MEDIDA"),
                    douLatitud = dr.Field<double>("LATITUD"),
                    douLongitud = dr.Field<double>("LONGITUD"),
                    strColor = dr.Field<string>("COLOR")
                })).ToList<dtoSensor>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void eliminar()
        {
            try
            {
                using (dalRedis dalRedis = new dalRedis(this.dic))
                    dalRedis.eliminar(this.GetType().Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
        }
    }
}