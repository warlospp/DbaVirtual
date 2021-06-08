using DAL;
using DTO;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class bllDeploy : IDisposable
    {
        private string strConn { get; set; }
        private static MLContext mlContext = new MLContext(/*seed: 1*/);
        public bllDeploy(string _strConn)
        {
            this.strConn = _strConn;
        }

        public void ejecutar()
        {
            IDataView trainingDataView = null;
            try
            {
                DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<dtoDeploy>();

                DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, this.strConn, procesos.Default.ConsultarEntrenamiento);
                
                IDataView data = loader.Load(dbSource);

                if (data.GetRowCount() >0)
                {

                }
                else
                {

                }
                // Load Data
                //using (dalDeploy dal = new dalDeploy(this.strConn))
                //    trainingDataView = dal.ejecutarProc(procesos.Default.ConsultarEntrenamiento);
                // Build training pipeline
                //IEstimator<ITransformer> trainingPipeline = this.BuildTrainingPipeline(mlContext);

                // Evaluate quality of Model
                //Evaluate(mlContext, trainingDataView, trainingPipeline);

                // Train Model
                //ITransformer mlModel = TrainModel(mlContext, trainingDataView, trainingPipeline);

                // Save model
                //SaveModel(mlContext, mlModel, savingPath, trainingDataView.Schema);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //private  IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        //{
            // Data process configuration with pipeline data transformations 
          //  var dataProcessPipeline = mlContext.Transforms.Concatenate("Features", new[] {"TipoArchivo","TipoArchivo","BajarSistema","Sistema","Distribucion","Extension","DiaSemana","TipoDia","Solicitante","TipoDeploy","Responsable" });

            // Set the training algorithm 
            //var trainer = mlContext.BinaryClassification.Trainers. LightGbm(labelColumnName: "Class", featureColumnName: "Features");
            //var trainingPipeline = dataProcessPipeline.Append(trainer);

            //return trainingPipeline;
        //}

        public void Dispose()
        {

        }
    }
}
