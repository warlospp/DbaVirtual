using CMN;
using DAL.Conexiones;
using DTO;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DAL
{
    public class dalTelemetria : IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public dalTelemetria(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        public async void insertar(dtoTelemetria _obj)
        {
            try
            {
                string[] strArray = this.dic.Where((x => x.Key == cmnConfiguraciones.MongoDb)).Select((x => x.Value)).FirstOrDefault<string>().Split('|');
                using (dalMongoDb dalMongoDb = new dalMongoDb())
                {
                    dalMongoDb.abrir(strArray[0]);
                    await dalMongoDb.conexion.GetDatabase(strArray[1], null).GetCollection<BsonDocument>(cmnObjetos.ColeccionTelemetria, null).InsertOneAsync(new BsonDocument()
                    {
                        {
                        "id_sensor", _obj.intIdSensor
                        },
                        {
                        "fecha",  _obj.dtFecha
                        },
                        {
                        "metrica",  Math.Round(_obj.douMetrica, int.Parse(this.dic.Where( (x => x.Key == cmnConfiguraciones.NumeroDecimales)).Select( (x => x.Value)).FirstOrDefault<string>()))
                        },
                        {
                        "fecha_servidor",  _obj.dtFechaServidor
                        }
                    }, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<dtoTelemetria> consultar(string _strIntervalo)
        {
            List<dtoTelemetria> dtos = new List<dtoTelemetria>();
            try
            {
                string[] strArray1 = this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.MongoDb)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>().Split('|');
                string[] strArray2 = _strIntervalo.Split('.');
                DateTime dateTime1 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:00"));
                DateTime dateTime2 = strArray2[1] == cmnIntervalos.Minuto ? dateTime1.AddMinutes((double)(int.Parse(strArray2[0]) * -1)) : (strArray2[1] == cmnIntervalos.Segundo ? dateTime1.AddSeconds((double)(int.Parse(strArray2[0]) * -1)) : (strArray2[1] == cmnIntervalos.Milisegundo ? dateTime1.AddMilliseconds((double)(int.Parse(strArray2[0]) * -1)) : dateTime1));
                using (dalMongoDb item = new dalMongoDb())
                {
                    item.abrir(strArray1[0]);
                    IMongoCollection<BsonDocument> collection = item.conexion.GetDatabase(strArray1[1], (MongoDatabaseSettings)null).GetCollection<BsonDocument>(cmnObjetos.ColeccionTelemetria, (MongoCollectionSettings)null);
                    FilterDefinitionBuilder<BsonDocument> filter1 = Builders<BsonDocument>.Filter;
                    List<BsonDocument> bsonDocumentList = new List<BsonDocument>();
                    FilterDefinition<BsonDocument> filter2 = (FilterDefinition<BsonDocument>)(filter1.Lte<DateTime>((FieldDefinition<BsonDocument, DateTime>)"fecha", dateTime1) & filter1.Gt<DateTime>((FieldDefinition<BsonDocument, DateTime>)"fecha", dateTime2));
                    foreach (BsonDocument bsonDocument in collection.Find<BsonDocument>(filter2, (FindOptions)null).ToList<BsonDocument>(new CancellationToken()))
                        dtos.Add(new dtoTelemetria()
                        {
                            intIdSensor = int.Parse(bsonDocument["id_sensor"].ToString()),
                            dtFecha = DateTime.Parse(bsonDocument["fecha"].ToString()),
                            douMetrica = bsonDocument["metrica"].ToDouble()
                        });
                }
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