using CMN;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace DAL.Conexiones
{
    public class dalRedis : IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();
        private ConnectionMultiplexer conexion;

        public dalRedis(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        private ConnectionMultiplexer abrir()
        {
            try
            {
                this.conexion = ConnectionMultiplexer.Connect(this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.Redis)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>(), (TextWriter)null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.conexion;
        }

        private void cerrar()
        {
            try
            {
                this.conexion.Close(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void insertar(Dictionary<int, string> _dic, string _str)
        {
            try
            {
                HashEntry[] array = _dic.Select<KeyValuePair<int, string>, HashEntry>((Func<KeyValuePair<int, string>, HashEntry>)(pair => new HashEntry((RedisValue)pair.Key, (RedisValue)pair.Value))).ToArray<HashEntry>();
                this.abrir().GetDatabase(-1, (object)null).HashSetAsync((RedisKey)_str, array, CommandFlags.None);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.cerrar();
            }
        }

        public List<HashEntry> consultar(string _str)
        {
            HashEntry[] all;
            try
            {
                all = this.abrir().GetDatabase(-1, (object)null).HashGetAll((RedisKey)_str, CommandFlags.None);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.cerrar();
            }
            return ((IEnumerable<HashEntry>)all).ToList<HashEntry>();
        }

        public void eliminar(string _str)
        {
            try
            {
                foreach (EndPoint endPoint in this.abrir().GetEndPoints(true))
                    this.conexion.GetDatabase(-1, (object)null).KeyDeleteAsync(this.conexion.GetServer(endPoint, (object)null).Keys(0, (RedisValue)_str, 250, 0L, 0, CommandFlags.None).ToArray<RedisKey>(), CommandFlags.None);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.cerrar();
            }
        }

        public void Dispose()
        {
        }
    }
}
