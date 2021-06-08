using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF
{
    [ServiceContract]
    public interface IwcfChatBot
    {
        [OperationContract]
        List<dtoOpcion> opciones(parOpcion _par);
        [OperationContract]
        List<dtoInstancia> cargarInstancias();
        [OperationContract]
        string cargarMensaje(dtoOpcion _dtoOpcion, dtoInstancia _dtoInstancia, string _strUsuario);

    }
}
