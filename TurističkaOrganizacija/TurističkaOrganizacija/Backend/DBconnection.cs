using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurističkaOrganizacija.Backend
{
    public class DBconnection
    {
        private static DBconnection instance = null;
        private DbConnection _connection;

        private static readonly object _lock = new object();

        public static DbConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new DBconnection();
                        }
                    }
                }
                // Always return a fresh connection to avoid sharing issues
                return DbProviderFactory.CreateConnection();
            }
        }


        private DBconnection() { }
    }
}
