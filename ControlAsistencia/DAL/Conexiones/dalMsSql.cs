using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Conexiones
{
    public abstract class dalMsSql : IDisposable
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }

        protected DataTable exec(string _strConn, string _strProc)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(_strProc, this.abrir(_strConn)))
                {
                    //sqlCommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                    {
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        sqlDataAdapter.Fill(dataSet);
                        sqlDataAdapter.Dispose();
                    }
                    sqlCommand.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.cerrar();
            }
            return dataSet.Tables[0];
        }

        protected DataTable exec(string _strConn, string _strQuery, Dictionary<string, string> _obj)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SqlCommand oleDbCommand = new SqlCommand(_strQuery, this.abrir(_strConn)))
                {
                    foreach (KeyValuePair<string, string> keyValuePair in _obj)
                        oleDbCommand.Parameters.AddWithValue(keyValuePair.Key, (object)keyValuePair.Value);
                    using (SqlDataAdapter oleDbDataAdapter = new SqlDataAdapter())
                    {
                        oleDbDataAdapter.SelectCommand = oleDbCommand;
                        oleDbDataAdapter.Fill(dataSet);
                        oleDbDataAdapter.Dispose();
                    }
                    oleDbCommand.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.cerrar();
            }
            return dataSet.Tables[0];
        }

        //protected bool exec(string _strConn)
        //{
        //    bool flag = false;
        //    try
        //    {
        //        this.abrir(_strConn);
        //        flag = true;
        //    }
        //    catch (Exception)
        //    {
        //        flag = false;
        //        throw;
        //    }
        //    finally
        //    {
        //        this.cerrar();
        //    }
        //    return flag;
        //}
        public void Dispose()
        {
    
        }
    }
}
