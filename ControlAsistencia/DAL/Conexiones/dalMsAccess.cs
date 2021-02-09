using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Conexiones
{
    public abstract class dalMsAccess 
    {
        private OleDbConnection conexion { get; set; }

        private OleDbConnection abrir(string _str)
        {
            this.conexion = new OleDbConnection(_str);
            try
            {
                if (this.conexion.State != ConnectionState.Open)
                {
                    this.conexion.Open();
                }
            }
            catch (Exception) 
            {
                throw ;
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

        protected DataTable exec(string _strConn, string _strQuery)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (OleDbCommand oleDbCommand = new OleDbCommand(_strQuery, this.abrir(_strConn)))
                {
                    using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter())
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

        protected DataTable exec(string _strConn,string _strQuery,Dictionary<string, string> _obj)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (OleDbCommand oleDbCommand = new OleDbCommand(_strQuery, this.abrir(_strConn)))
                {
                    foreach (KeyValuePair<string, string> keyValuePair in _obj)
                        oleDbCommand.Parameters.AddWithValue(keyValuePair.Key, (object)keyValuePair.Value);
                    using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter())
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
    }
}
