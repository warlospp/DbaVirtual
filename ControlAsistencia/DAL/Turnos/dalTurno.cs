using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Turnos;

namespace DAL.Turnos
{
    public class dalTurno : Conexiones.dalMsSql, IDisposable
    {
        private string strConn { get; set; }
        public dalTurno(string _strConn)
        {
            this.strConn = _strConn;
        }

        public List<dtoHorario> execQueryHorario(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoHorario>((dr => new dtoHorario()
                {
                    SCHCLASSID = dr.Field<int>("SCHCLASSID"),
                    SCHNAME = dr.Field<string>("SCHNAME"),
                    _STARTTIME = dr.Field<object>("STARTTIME"),
                    _ENDTIME = dr.Field<object>("ENDTIME"),
                    LATEMINUTES = dr.Field<int>("LATEMINUTES"),
                    EARLYMINUTES = dr.Field<int>("EARLYMINUTES"),
                    _CHECKIN = dr.Field<int>("CHECKIN"),
                    _CHECKOUT = dr.Field<int>("CHECKOUT"),
                    _CHECKINTIME1 = dr.Field<object>("CHECKINTIME1"),
                    _CHECKINTIME2 = dr.Field<object>("CHECKINTIME2"),
                    _CHECKOUTTIME1 = dr.Field<object>("CHECKOUTTIME1"),
                    _CHECKOUTTIME2 = dr.Field<object>("CHECKOUTTIME2"),
                    WorkDay = dr.Field<double>("WorkDay"),
                    _WorkMins = dr.Field<double>("WorkMins")
                })).ToList<dtoHorario>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<dtoTurnoCabecera> execQueryTurnoCabecera(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoTurnoCabecera>((dr => new dtoTurnoCabecera()
                {
                    NUM_RUNID = dr.Field<int>("NUM_RUNID"),
                    NAME = dr.Field<string>("NAME"),
                    _STARTDATE = dr.Field<object>("STARTDATE"),
                    _ENDDATE = dr.Field<object>("ENDDATE"),
                    CYLE = dr.Field<short>("CYLE"),
                    _UNITS = dr.Field<short>("UNITS")
                })).ToList<dtoTurnoCabecera>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<dtoTurnoDetalle> execQueryTurnoDetalle(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoTurnoDetalle>((dr => new dtoTurnoDetalle()
                {
                    NUM_RUNID = dr.Field<short>("NUM_RUNID"),
                    SDAYS = dr.Field<short>("SDAYS"),
                    EDAYS = dr.Field<short>("EDAYS"),
                    SCHCLASSID = dr.Field<int>("SCHCLASSID"),
                    _Overtime = dr.Field<object>("Overtime")
                })).ToList<dtoTurnoDetalle>();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public List<dtoTurnoFijo> execQueryTurnoFijo(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoTurnoFijo>((dr => new dtoTurnoFijo()
                {
                    USERID = dr.Field<int>("USERID"),
                    NUM_OF_RUN_ID = dr.Field<int>("NUM_OF_RUN_ID"),
                    _STARTDATE = dr.Field<object>("STARTDATE"),
                    _ENDDATE = dr.Field<object>("ENDDATE")
                })).ToList<dtoTurnoFijo>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<dtoTurnoRotativo> execQueryTurnoRotativo( string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoTurnoRotativo>((dr => new dtoTurnoRotativo()
                {
                    UserId = dr.Field<int>("UserId"),
                    SchId = dr.Field<int>("SchId")
                })).ToList<dtoTurnoRotativo>();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public List<dtoTurnoTemporal> execQueryTurnoTemporal(string _strQuery)
        {
            try
            {
                return this.exec(this.strConn, _strQuery).AsEnumerable().Select<DataRow, dtoTurnoTemporal>((dr => new dtoTurnoTemporal()
                {
                    USERID = dr.Field<int>("USERID"),
                    _COMETIME = dr.Field<object>("COMETIME"),
                    _LEAVETIME = dr.Field<object>("LEAVETIME"),
                    _OVERTIME = dr.Field<int>("OVERTIME"),
                    SCHCLASSID = dr.Field<int>("SCHCLASSID")
                })).ToList<dtoTurnoTemporal>();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void Dispose()
        {
        }
    }
}
