
using MongoDB.Driver;
using System;

namespace DAL.Conexiones
{
    public class dalMongoDb : IDisposable
    {
        public MongoClient conexion { get; set; }

        public MongoClient abrir(string _strConexion)
        {
            try
            {
                this.conexion = new MongoClient(_strConexion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this.conexion;
        }

        public void Dispose()
        {
        }
    }
}
