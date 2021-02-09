using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DTO.Empleados;
using DTO.Marcaciones;
using DTO.Turnos;
using BLL.Empleados;
using BLL.Marcaciones;
using BLL.Turnos;

namespace SVC
{
    [ServiceContract]
    public interface IControlAsistencia
    {
        [OperationContract]
        List<dtoDepartamento> getDepartamento(string _str);

        [OperationContract]
        void procesarTurno(string[] _strIdDepartamento, int[] _intIdEmpleado, string _strEmpleado, DateTime _dtFechaInicio, DateTime _dtFechaFin);


    }
}
