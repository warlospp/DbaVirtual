using Npgsql;
using System;
using System.Data;

namespace DAL.Conexiones
{
    public class dalPostgreSql : IDisposable
    {
        private NpgsqlConnection conexion { get; set; }

        private NpgsqlConnection abrir(string _str)
        {
            this.conexion = new NpgsqlConnection(_str);
            try
            {
                if (this.conexion.State != ConnectionState.Open)
                    this.conexion.Open();
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
                if (this.conexion.State != ConnectionState.Open)
                    return;
                this.conexion.Close();
                this.conexion.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ejecutar(string _strConn, string _strProc)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (NpgsqlCommand npgsqlCommand = new NpgsqlCommand(_strProc, this.abrir(_strConn)))
                {
                    npgsqlCommand.CommandType = CommandType.StoredProcedure;
                    using (NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter())
                    {
                        npgsqlDataAdapter.SelectCommand = npgsqlCommand;
                        npgsqlDataAdapter.Fill(dataSet);
                        npgsqlDataAdapter.Dispose();
                    }
                    npgsqlCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.cerrar();
            }
            return dataSet.Tables[0];
        }

        public void Dispose()
        {
        }
    }
}