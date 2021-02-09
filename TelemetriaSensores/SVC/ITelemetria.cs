using DTO;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SVC
{
    [ServiceContract]
    public interface ITelemetria
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "insertarAlerta/{_intIdSensor}/{_dtFecha}/{_douMetrica}")]
        void insertarAlerta(string _intIdSensor, string _dtFecha, string _douMetrica);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "eliminarParametros")]
        void eliminarParametros();

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "modificarParametros/{_strEnviaAlerta}")]
        void modificarParametros(string _strEnviaAlerta);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "consultarAlertaDetalle/{_strIntervalo}")]
        List<dtoAlerta> consultarAlertaDetalle(string _strIntervalo);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "consultarAlertaxSensor/{_strIntervalo}")]
        List<dtoAlerta> consultarAlertaxSensor(string _strIntervalo);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "consultarAlertaxTipo/{_strIntervalo}")]
        List<dtoAlerta> consultarAlertaxTipo(string _strIntervalo);

    }
}
