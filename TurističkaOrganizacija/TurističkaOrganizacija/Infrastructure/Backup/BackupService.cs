using System;
using System.Data.SqlClient;
using System.IO;
using TurističkaOrganizacija.Application;
using TurističkaOrganizacija.Backend;
using System.Data.Common;

namespace TurističkaOrganizacija.Infrastructure.Backup
{
    /// <summary>
    /// Simple backup facade. For SQL Server demo, it just writes a timestamped marker file.
    /// Later can branch to SQLite copy or MySQL dump based on connection string/provider.
    /// </summary>
    public class BackupService : IBackupService
    {
        public void RunBackup()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backups");
            Directory.CreateDirectory(folder);

            // For SQL Server, execute BACKUP DATABASE to a .bak file
            var sqlConn = TurističkaOrganizacija.Backend.DbProviderFactory.CreateConnection() as SqlConnection;
            if (sqlConn != null)
            {
                var csb = new SqlConnectionStringBuilder(sqlConn.ConnectionString);
                string dbName = csb.InitialCatalog;
                if (string.IsNullOrWhiteSpace(dbName)) dbName = "agencija";

                string bakPath = Path.Combine(folder, $"{dbName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak");
                string sql = $"BACKUP DATABASE [{dbName}] TO DISK = '{bakPath.Replace("'","''")}' WITH INIT";

                using (var conn = new SqlConnection(sqlConn.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                }
                return;
            }

            var sqliteConn = TurističkaOrganizacija.Backend.DbProviderFactory.CreateConnection();
            if (sqliteConn != null && sqliteConn.GetType().Name.IndexOf("SQLite", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Za SQLite: kopija fajla iz connection string-a (Data Source=...)
                string source = ExtractSqliteFilePath(sqliteConn.ConnectionString);
                if (File.Exists(source))
                {
                    string dest = Path.Combine(folder, $"sqlite_{DateTime.UtcNow:yyyyMMdd_HHmmss}.db");
                    File.Copy(source, dest, overwrite: true);
                }
                return;
            }

            // Fallback marker file
            string filename = $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt";
            File.WriteAllText(Path.Combine(folder, filename), "Backup placeholder");
        }

        private static string ExtractSqliteFilePath(string connStr)
        {
            const string key = "Data Source=";
            int i = connStr.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (i >= 0)
            {
                string rest = connStr.Substring(i + key.Length);
                int sep = rest.IndexOf(';');
                return sep >= 0 ? rest.Substring(0, sep) : rest;
            }
            return connStr;
        }
    }
}