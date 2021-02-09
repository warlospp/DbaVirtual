using CMN;
using DAL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class bllParametro:IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public bllParametro(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        public dtoParametro consultar()
        {
            dtoParametro dto = new dtoParametro();
            try
            {
                using (dalParametro dal = new dalParametro(this.dic))
                    dto = dal.consultar();              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dto;
        }

        public void eliminar()
        {
            try
            {
                using (dalUmbral dal = new dalUmbral(this.dic))
                    dal.eliminar();
                using (dalSensor dal = new dalSensor(this.dic))
                    dal.eliminar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void modificar(dtoParametro _dto)
        {
            try
            {
                using (dalParametro dal = new dalParametro(this.dic))
                    dal.modificar(_dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {

        }
    }
}
