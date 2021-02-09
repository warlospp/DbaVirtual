namespace CMN
{
    public static class cmnConfiguraciones
    {
        public static string MongoDb
        {
            get
            {
                return parametros.Default.MongoDb;
            }
        }

        public static string Redis
        {
            get
            {
                return parametros.Default.Redis;
            }
        }

        public static string ConectorSql
        {
            get
            {
                return parametros.Default.ConectorSql;
            }
        }

        public static string Sql
        {
            get
            {
                return parametros.Default.Sql;
            }
        }

        public static string NumeroDecimales
        {
            get
            {
                return parametros.Default.NumeroDecimales;
            }
        }
    }
}