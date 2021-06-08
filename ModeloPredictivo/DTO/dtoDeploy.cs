using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class dtoDeploy //: IDisposable
    {
        [ColumnName("TipoArchivo"), LoadColumn(0)]
        public string strTipoArchivo { get; set; }
        [ColumnName("BajarSistema"), LoadColumn(1)]
        public string strBajarSistema { get; set; }
        [ColumnName("Sistema"), LoadColumn(2)]
        public string strSistema { get; set; }
        [ColumnName("Distribucion"), LoadColumn(3)]
        public string strDistribucion { get; set; }
        [ColumnName("Extension"), LoadColumn(4)]
        public string strExtension { get; set; }
        [ColumnName("DiaSemana"), LoadColumn(5)]
        public string strDiaSemana { get; set; }
        [ColumnName("TipoDia"), LoadColumn(6)]
        public string strTipoDia { get; set; }
        [ColumnName("Solicitante"), LoadColumn(7)]
        public string strSolicitante { get; set; }
        [ColumnName("TipoDeploy"), LoadColumn(8)]
        public string strTipoDeploy { get; set; }
        [ColumnName("Responsable"), LoadColumn(9)]
        public string strResponsable { get; set; }
        [ColumnName("Class"), LoadColumn(10)]
        public string strResultado { get; set; }
        //public void Dispose()
        //{

        //}
    }
}
