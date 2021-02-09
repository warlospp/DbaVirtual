using CMN;
using DAL;
using DTO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class bllAlerta : IDisposable
    {
        private Dictionary<string, string> dic = new Dictionary<string, string>();

        public bllAlerta(Dictionary<string, string> _dic)
        {
            this.dic = _dic;
        }

        public void insertar(dtoTelemetria _dto)
        {
            try
            {
                using (dalTelemetria dalTelemetria = new dalTelemetria(this.dic))
                    dalTelemetria.insertar(_dto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<dtoAlerta> consultarAlerta(dtoTelemetria _dto)
        {
            List<dtoAlerta> dtos = new List<dtoAlerta>();
            try
            {
                List<dtoTelemetria> dtoT = new List<dtoTelemetria>();
                dtoT.Add(_dto);
                List<dtoUmbral> dtoU = new List<dtoUmbral>();
                using (dalUmbral dal = new dalUmbral(this.dic))
                    dtoU = dal.consultar();
                List<dtoSensor> dtoS = new List<dtoSensor>();
                using (dalSensor dal = new dalSensor(this.dic))
                    dtoS = dal.consultar();
                IEnumerable<dtoUmbral> tmpMin = dtoU.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => u.strUmbral == cmnUmbral.Minimo));
                IEnumerable<dtoUmbral> tmpOpt = dtoU.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => u.strUmbral == cmnUmbral.Optimo));
                IEnumerable<dtoUmbral> tmpMax = dtoU.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => u.strUmbral == cmnUmbral.Maximo));
                dtos = (from t in dtoT
                        join s in dtoS on t.intIdSensor equals s.intIdSensor
                        select new dtoAlerta()
                        {
                            strDispositivo = s.strDispositivo,
                            strIP = s.strIP,
                            strDescripcion = s.strDescripcion,
                            strDireccion = s.strDireccion,
                            strSensor = s.strSensor,
                            strTipo = s.strTipo,
                            strUnidadMedida = s.strUnidadMedida,
                            douLatitud = s.douLatitud,
                            douLongitud = s.douLongitud,
                            strColor = s.strColor,
                            strUmbral = tmpMin.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica < u.douMinimo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Bajo : (tmpMin.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMinimo && t.douMetrica < u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Minimo : (tmpOpt.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMinimo && t.douMetrica < u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Optimo : (tmpMax.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMinimo && t.douMetrica < u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Maximo : (tmpMax.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Sobre : string.Empty)))),
                            douMetrica = Math.Round(t.douMetrica, int.Parse(this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.NumeroDecimales)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>())),
                            strFecha = t.dtFecha.ToString("yyyy-MM-dd HH:mm:ss")
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtos;
        }

        public List<dtoAlerta> consultarAlerta(string _strIntervalo, string _strTipo)
        {
            List<dtoAlerta> dtos = new List<dtoAlerta>();
            try
            {
                List<dtoTelemetria> dtoT = new List<dtoTelemetria>();
                using (dalTelemetria dal = new dalTelemetria(this.dic))
                    dtoT = dal.consultar(_strIntervalo);
                List<dtoUmbral> dtoU = new List<dtoUmbral>();
                using (dalUmbral dal = new dalUmbral(this.dic))
                    dtoU = dal.consultar();
                List<dtoSensor> dtoS = new List<dtoSensor>();
                using (dalSensor dal = new dalSensor(this.dic))
                    dtoS = dal.consultar();
                IEnumerable<dtoUmbral> tmpMin = dtoU.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => u.strUmbral == cmnUmbral.Minimo));
                IEnumerable<dtoUmbral> tmpOpt = dtoU.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => u.strUmbral == cmnUmbral.Optimo));
                IEnumerable<dtoUmbral> tmpMax = dtoU.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => u.strUmbral == cmnUmbral.Maximo));
                dtos = (from t in dtoT
                        join s in dtoS on t.intIdSensor equals s.intIdSensor
                        select new dtoAlerta()
                        {
                            strDispositivo = s.strDispositivo,
                            strIP = s.strIP,
                            strDescripcion = s.strDescripcion,
                            strDireccion = s.strDireccion,
                            strSensor = s.strSensor,
                            strTipo = s.strTipo,
                            strUnidadMedida = s.strUnidadMedida,
                            douLatitud = s.douLatitud,
                            douLongitud = s.douLongitud,
                            strColor = s.strColor,
                            strUmbral = tmpMin.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica < u.douMinimo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Bajo : (tmpMin.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMinimo && t.douMetrica < u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Minimo : (tmpOpt.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMinimo && t.douMetrica < u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Optimo : (tmpMax.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMinimo && t.douMetrica < u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Maximo : (tmpMax.Where<dtoUmbral>((Func<dtoUmbral, bool>)(u => s.strTipo == u.strTipo && t.douMetrica >= u.douMaximo)).Count<dtoUmbral>() > 0 ? cmnUmbral.Sobre : string.Empty)))),
                            douMetrica = Math.Round(t.douMetrica, int.Parse(this.dic.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>)(x => x.Key == cmnConfiguraciones.NumeroDecimales)).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>)(x => x.Value)).FirstOrDefault<string>())),
                            strFecha = t.dtFecha.ToString("yyyy-MM-dd HH:mm:ss")
                        }).ToList();
                if (_strTipo == "AgrupadoxSensor")
                    dtos = (from t in dtos
                            group t by new dtoAlerta()
                            {
                                strDispositivo = t.strDispositivo,
                                strDescripcion = t.strDescripcion,
                                strDireccion = t.strDireccion,
                                strIP = t.strIP,
                                strSensor = t.strSensor,
                                strTipo = t.strTipo,
                                strUnidadMedida = t.strUnidadMedida,
                                douLatitud = t.douLatitud,
                                douLongitud = t.douLongitud,
                                strColor = t.strColor
                            } into g
                            select new dtoAlerta()
                            {
                                strDispositivo = g.Key.strDispositivo,
                                strDescripcion = g.Key.strDescripcion,
                                strDireccion = g.Key.strDireccion,
                                strIP = g.Key.strIP,
                                strSensor = g.Key.strSensor,
                                strTipo = g.Key.strTipo,
                                strUnidadMedida = g.Key.strUnidadMedida,
                                douLatitud = g.Key.douLatitud,
                                douLongitud = g.Key.douLongitud,
                                strColor = g.Key.strColor,
                                strUmbral = tmpMin.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) < u.douMinimo)).Count() > 0 ? cmnUmbral.Bajo : (tmpMin.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMinimo && g.Average((p => p.douMetrica)) < u.douMaximo)).Count() > 0 ? cmnUmbral.Minimo : (tmpOpt.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMinimo && g.Average((p => p.douMetrica)) < u.douMaximo)).Count() > 0 ? cmnUmbral.Optimo : (tmpMax.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMinimo && g.Average((p => p.douMetrica)) < u.douMaximo)).Count() > 0 ? cmnUmbral.Maximo : (tmpMax.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMaximo)).Count() > 0 ? cmnUmbral.Sobre : string.Empty)))),
                                douMetrica = Math.Round(g.Average((p => p.douMetrica)), int.Parse(this.dic.Where((x => x.Key == cmnConfiguraciones.NumeroDecimales)).Select((x => x.Value)).FirstOrDefault<string>())),
                                strFecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }).ToList<dtoAlerta>();
                else if (_strTipo == "AgrupadoxTipo")
                    dtos = (from t in dtos
                            group t by new dtoAlerta() { strTipo = t.strTipo } into g
                            select new dtoAlerta()
                            {
                                strTipo = g.Key.strTipo,
                                strUmbral = tmpMin.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) < u.douMinimo)).Count() > 0 ? cmnUmbral.Bajo : (tmpMin.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMinimo && g.Average((p => p.douMetrica)) < u.douMaximo)).Count() > 0 ? cmnUmbral.Minimo : (tmpOpt.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMinimo && g.Average((p => p.douMetrica)) < u.douMaximo)).Count() > 0 ? cmnUmbral.Optimo : (tmpMax.Where((u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMinimo && g.Average((p => p.douMetrica)) < u.douMaximo)).Count() > 0 ? cmnUmbral.Maximo : (tmpMax.Where(u => g.Key.strTipo == u.strTipo && g.Average((p => p.douMetrica)) >= u.douMaximo)).Count() > 0 ? cmnUmbral.Sobre : string.Empty))),
                                douMetrica = Math.Round(g.Average((p => p.douMetrica)), int.Parse(this.dic.Where((x => x.Key == cmnConfiguraciones.NumeroDecimales)).Select((x => x.Value)).FirstOrDefault<string>())),
                                strFecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }).ToList();
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
