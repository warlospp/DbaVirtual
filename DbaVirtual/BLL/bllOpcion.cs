using System;
using DTO;
using DAL;
using System.Collections.Generic;

namespace BLL
{
    public class bllOpcion : IDisposable
    {
        private string strConn { get; set; }

        public bllOpcion(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoOpcion> ejecutar(parOpcion _par)
        {
            List<dtoOpcion> dtos = new List<dtoOpcion>();
            try
            {
                string strProc = string.Empty;
                int intCrud  = _par.intCrud;
                switch (intCrud)
                {
                    case 0:
                        strProc = procesos.Default.CargarOpciones;
                        break;
                    case 1:
                        strProc = procesos.Default.InsertarOpciones;
                        break;
                    case 2:
                        strProc = procesos.Default.ModificarOpciones;
                        break;
                    case 3:
                        strProc = procesos.Default.EliminarOpciones;
                        break;
                };
                if (intCrud == 0)
                {
                    using (dalOpcion dal = new dalOpcion(this.strConn))
                    {
                        dtos = dal.ejecutarProc(strProc);
                    };
                }
                else
                {
                    using (dalOpcion dal = new dalOpcion(this.strConn))
                    {
                        if (dal.ejecutarProc(strProc, _par))
                            dtos = dal.ejecutarProc(procesos.Default.CargarOpciones);
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtos;
        }

        public void Dispose()
        {
            
        }
    }
}
