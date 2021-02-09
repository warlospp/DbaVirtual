using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Turnos;
using DTO.Turnos;

namespace BLL.Turnos
{
    public class bllRegla : IDisposable
    {
        private string strConn { get; set; }

        public bllRegla(string _strConn)
        {
            this.strConn = _strConn;
        }

        private List<dtoReglaPivot> execQuery()
        {
            List<dtoReglaPivot> dtos = new List<dtoReglaPivot>();
            try
            {
                string[] str = new string[5]
                {
                  "EatTime",
                  "MINSNOIN",
                  "NoInAbsent",
                  "MINSNOLEAVE",
                  "NoOutAbsent"
                };
                using (dalRegla dal = new dalRegla(this.strConn))
                {
                    var tmp_unpivot = dal.execQuery("select * from ATTPARAM ORDER BY PARANAME;")
                        .Where(r => str.Contains(r.PARANAME));
                    foreach (var data in    from tu in tmp_unpivot
                                            where tu.PARANAME == "EatTime"
                                            select new
                                            {
                                                EatTime = int.Parse(tu.PARAVALUE),
                                                MINSNOIN =  (from tt in tmp_unpivot
                                                            where tt.PARANAME == "MINSNOIN"
                                                            select int.Parse(tt.PARAVALUE)).Sum(),
                                                NoInAbsent = (from tt in tmp_unpivot
                                                             where tt.PARANAME == "NoInAbsent"
                                                             select int.Parse(tt.PARAVALUE)).Sum(),
                                                MINSNOLEAVE = (from tt in tmp_unpivot
                                                              where tt.PARANAME == "MINSNOLEAVE"
                                                              select int.Parse(tt.PARAVALUE)).Sum(),
                                                NoOutAbsent = (from tt in tmp_unpivot
                                                              where tt.PARANAME == "NoOutAbsent"
                                                              select int.Parse(tt.PARAVALUE)).Sum()
                                            })
                    {
                        dtos.Add(new dtoReglaPivot()
                        {
                            MINSNOIN = data.MINSNOIN,
                            NoInAbsent = data.NoInAbsent,
                            MINSNOLEAVE = data.MINSNOLEAVE * -1,
                            NoOutAbsent = data.NoOutAbsent
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dtos;
        }

        public List<dtoRegla> execQuery(DateTime _dtFechaInicio, DateTime _dtFechaFin)
        {
            List<dtoRegla> dtos = new List<dtoRegla>();
            try
            {
                foreach (var data in from r in this.execQuery()
                                     select new dtoRegla()
                                     {
                                         REGLA_ENTRADA = r.NoInAbsent != 0,
                                         FALTA_ENTRADA = r.NoInAbsent == 2,
                                         ENTRADA_TARDE = r.NoInAbsent == 1,
                                         MINS_ENTRADA = r.MINSNOIN,
                                         REGLA_SALIDA = r.NoOutAbsent != 0,
                                         FALTA_SALIDA = r.NoOutAbsent == 2,
                                         SALIDA_TEMPRANO = r.NoOutAbsent == 1,
                                         MINS_SALIDA = r.MINSNOLEAVE
                                     })
                {
                    dtos.Add(new dtoRegla()
                    {
                        REGLA_ENTRADA = data.REGLA_ENTRADA,
                        FALTA_ENTRADA = data.FALTA_ENTRADA,
                        ENTRADA_TARDE = data.ENTRADA_TARDE,
                        MINS_ENTRADA = data.MINS_ENTRADA,
                        REGLA_SALIDA = data.REGLA_SALIDA,
                        FALTA_SALIDA = data.FALTA_SALIDA,
                        SALIDA_TEMPRANO = data.SALIDA_TEMPRANO,
                        MINS_SALIDA = data.MINS_SALIDA
                    });
                    if (dtos.Count == 0)
                    {
                        dtos.Add(new dtoRegla()
                        {
                            REGLA_ENTRADA = false,
                            FALTA_ENTRADA = false,
                            ENTRADA_TARDE = false,
                            MINS_ENTRADA = 0,
                            REGLA_SALIDA = false,
                            FALTA_SALIDA = false,
                            SALIDA_TEMPRANO = false,
                            MINS_SALIDA = 0
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dtos;
        }

        public void Dispose()
        {
        }
    }
}
