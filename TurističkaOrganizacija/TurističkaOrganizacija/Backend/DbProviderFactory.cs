using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace TurističkaOrganizacija.Backend
{
    /// <summary>
    /// Abstracts creation of database connections based on connection string.
    /// For now supports SQL Server via SqlConnection.
    /// Later can be extended to SQLite/MySQL and adapted via Adapter/Abstract Factory.
    /// </summary>
    public static class DbProviderFactory
    {
        public static DbConnection CreateConnection()
        {
            string connectionString = AppConfig.Instance.ConnectionString;

            // Na osnovu connection stringa odrediti provajdera.
            // SQLite: ako konekcioni string liči na Data Source=*.db ili sadrži ".sqlite"/".db"
            if (!string.IsNullOrWhiteSpace(connectionString) &&
                (connectionString.IndexOf(".db", StringComparison.OrdinalIgnoreCase) >= 0 ||
                 connectionString.IndexOf(".sqlite", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                try
                {
                    var factory = System.Data.Common.DbProviderFactories.GetFactory("System.Data.SQLite");
                    var conn = factory.CreateConnection();
                    conn.ConnectionString = connectionString;
                    return conn;
                }
                catch
                {
                    throw new NotSupportedException("SQLite provider nije instaliran. Instaliraj NuGet paket 'System.Data.SQLite.Core' i registruj provajdera.");
                }
            }

            // SQL Server podrazumevano
            if (!string.IsNullOrWhiteSpace(connectionString)) return new SqlConnection(connectionString);

            // Fallback na postojeći LocalDB ukoliko config nije postavljen.
            return new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Database=agencija;Integrated Security=True");
        }
    }
}


