using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    [Serializable]
    public class dtoParametro : IDisposable
    {
        public bool booEnviaAlerta { get; set; }
        public void Dispose()
        {
            
        }
    }
}
