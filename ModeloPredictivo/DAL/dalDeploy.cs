using DTO;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class dalDeploy :  IDisposable
    {
        private string strConn { get; set; }
        public dalDeploy(string _strConn)
        {
            this.strConn = _strConn;
        }
        public IDataView ejecutarProc(string _strProc)
        {
            IDataView data = null;
            try
            {
                MLContext mlContext = new MLContext();
                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<dtoDeploy>();

                DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, this.strConn, _strProc);
                data = loader.Load(dbSource);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
