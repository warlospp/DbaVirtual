namespace CMN
{
    public static class cmnConectores
    {
        public static string MsSql
        {
            get
            {
                return parametros.Default.MsSql;
            }
        }

        public static string MySql
        {
            get
            {
                return parametros.Default.MySql;
            }
        }

        public static string PostgreSql
        {
            get
            {
                return parametros.Default.PostgreSql;
            }
        }
    }
}
