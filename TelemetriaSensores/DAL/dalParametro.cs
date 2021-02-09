using CMN;
using DAL.Conexiones;
using DTO;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class dalParametro : IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public dalParametro(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        public dtoParametro consultar()
        {
            dtoParametro dto = new dtoParametro();
            try
            {
                using (dalRedis dalRedis = new dalRedis(this.dic))
                {
                    List<HashEntry> source = dalRedis.consultar(this.GetType().Name);
                    if (source.Count<HashEntry>() <= 0)
                    {
                        Dictionary<int, string> _dic = new Dictionary<int, string>();
                        dto = new dtoParametro()
                        {
                            booEnviaAlerta = true
                        };
                        string str = JsonConvert.SerializeObject(dto);
                        _dic.Add(1, str);
                        dalRedis.insertar(_dic, this.GetType().Name);
                    }
                    else
                    {
                        foreach (HashEntry hashEntry in source)
                        {
                            dto = JsonConvert.DeserializeObject<dtoParametro>((string)hashEntry.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dto;
        }

        public void modificar(dtoParametro _dto)
        {
            try
            {
                using (dalRedis dalRedis = new dalRedis(this.dic))
                {
                    List<HashEntry> source = dalRedis.consultar(this.GetType().Name);
                    if (source.Count<HashEntry>() > 0)
                        this.eliminar();
                    Dictionary<int, string> _dic = new Dictionary<int, string>();
                    string str = JsonConvert.SerializeObject(_dto);
                    _dic.Add(1, str);
                    dalRedis.insertar(_dic, this.GetType().Name);
                }
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
