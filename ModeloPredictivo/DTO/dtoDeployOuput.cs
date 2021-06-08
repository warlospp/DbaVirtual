using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class dtoDeployOuput
    {
        [ColumnName("PredictedLabel")]
        public string strPrediccion { get; set; }

        public float floScore { get; set; }
    }
}
