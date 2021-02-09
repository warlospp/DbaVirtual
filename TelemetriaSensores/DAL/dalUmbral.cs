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
    public class dalUmbral : IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public dalUmbral(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        public List<dtoUmbral> consultar()
        {
            List<dtoUmbral> dtos = new List<dtoUmbral>();
            try
            {
                using (dalRedis dalRedis = new dalRedis(this.dic))
                {
                    List<HashEntry> source = dalRedis.consultar(this.GetType().Name);
                    if (source.Count<HashEntry>() <= 0)
                    {
                        dtos = this.ejecutar();
                        Dictionary<int, string> _dic = new Dictionary<int, string>();
                        foreach (dtoUmbral item in dtos)
                        {
                            string str = JsonConvert.SerializeObject((object)item);
                            _dic.Add(item.intIdUmbral, str);
                        }
                        dalRedis.insertar(_dic, this.GetType().Name);
                    }
                    else
                    {
                        foreach (HashEntry hashEntry in source)
                        {
                            dtoUmbral item = JsonConvert.DeserializeObject<dtoUmbral>((string)hashEntry.Value);
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

        private List<dtoUmbral> ejecutar()
        {
            try
            {
                DataTable source = new DataTable();
                string str = this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.ConectorSql)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>();
                string _strConn = this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.Sql)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>();
                if (str == cmnConectores.MySql)
                {
                    using (dalMySql dalMySql = new dalMySql())
                        source = dalMySql.ejecutar(_strConn, cmnObjetos.ConsultarUmbral);
                }
                else if (str == cmnConectores.PostgreSql)
                {
                    using (dalPostgreSql dalPostgreSql = new dalPostgreSql())
                        source = dalPostgreSql.ejecutar(_strConn, cmnObjetos.ConsultarUmbral);
                }
                else if (str == cmnConectores.MsSql)
                {
                    using (dalMsSql dalMsSql = new dalMsSql())
                        source = dalMsSql.ejecutar(_strConn, cmnObjetos.ConsultarUmbral);
                }
                return source.AsEnumerable().Select<DataRow, dtoUmbral>((Func<DataRow, dtoUmbral>)(dr => new dtoUmbral()
                {
                    intIdUmbral = dr.Field<int>("ID_UMBRAL"),
                    strTipo = dr.Field<string>("TIPO"),
                    strUmbral = dr.Field<string>("UMBRAL"),
                    strDescripcion = dr.Field<string>("DESCRIPCION"),
                    douMinimo = dr.Field<double>("MINIMO"),
                    douMaximo = dr.Field<double>("MAXIMO")
                })).ToList<dtoUmbral>();
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
