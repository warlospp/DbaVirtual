using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Conexiones
{
    public abstract class dalMsSql<T>: IDisposable
    {
        private SqlConnection conexion { get; set; }
        public abstract Dictionary<string, object> parametro(T dto);
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

        public DataTable proc(string _strConn, string _strProc)
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
                    };
                    sqlCommand.Dispose();
                };
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

        public bool proc(string _strConn, string _strProc, T _parametro)
        {
            bool boo = false;
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(_strProc, this.abrir(_strConn)))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    foreach (var item in this.parametro(_parametro))
                    {
                        sqlCommand.Parameters.AddWithValue(item.Key, item.Value);
                    };
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    boo = true;
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.cerrar();
            }
            return boo;
        }

        public DataTable query(string _strConn, string _strQuery)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(_strQuery, this.abrir(_strConn)))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        sqlDataAdapter.Fill(dataSet);
                        sqlDataAdapter.Dispose();
                    };
                    sqlCommand.Dispose();
                };
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
