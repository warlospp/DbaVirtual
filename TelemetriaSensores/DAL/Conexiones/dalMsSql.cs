using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Conexiones
{
    public class dalMsSql : IDisposable
    {
        private SqlConnection conexion { get; set; }

        private SqlConnection abrir(string _str)
        {
            this.conexion = new SqlConnection(_str);
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
                using (SqlCommand sqlCommand = new SqlCommand(_strProc, this.abrir(_strConn)))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        sqlDataAdapter.Fill(dataSet);
                        sqlDataAdapter.Dispose();
                    }
                    sqlCommand.Dispose();
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