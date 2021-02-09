using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace DAL.Conexiones
{
    public class dalMySql : IDisposable
    {
        private MySqlConnection conexion { get; set; }

        private MySqlConnection abrir(string _str)
        {
            this.conexion = new MySqlConnection(_str);
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
                using (MySqlCommand mySqlCommand = new MySqlCommand(_strProc, this.abrir(_strConn)))
                {
                    mySqlCommand.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter())
                    {
                        mySqlDataAdapter.SelectCommand = mySqlCommand;
                        mySqlDataAdapter.Fill(dataSet);
                        mySqlDataAdapter.Dispose();
                    }
                    mySqlCommand.Dispose();
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
