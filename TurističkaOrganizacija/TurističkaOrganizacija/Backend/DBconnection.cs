using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurističkaOrganizacija.Backend
{
    public class DBconnection
    {
        private static DBconnection instance = null;
        private SqlConnection _connection;

        private static readonly object _lock = new object();

        public static SqlConnection Instance
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
                return instance._connection;
            }
        }


        private DBconnection() 
        {
            string connection_string = @"Data Source=(LocalDB)\dizajniranje_softvera;Database=agencija;integrated Security = True";
            _connection = new SqlConnection(connection_string);
        }
    }
}
