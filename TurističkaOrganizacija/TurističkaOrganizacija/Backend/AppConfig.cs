using System;
using System.IO;

namespace TurističkaOrganizacija.Backend
{
    /// <summary>
    /// Singleton for reading application configuration from config.txt.
    /// Line 1: Agency name
    /// Line 2: Database connection string
    /// </summary>
    public sealed class AppConfig
    {
        private static readonly object _lock = new object();
        private static AppConfig _instance;

        public string AgencyName { get; private set; }
        public string ConnectionString { get; private set; }

        private AppConfig()
        {
            LoadFromConfigFile();
        }

        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AppConfig();
                        }
                    }
                }
                return _instance;
            }
        }

        private void LoadFromConfigFile()
        {
            
            string projectRoot = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            string configPath = Path.Combine(projectRoot, "config.txt");

            if (!File.Exists(configPath))
            {
                File.WriteAllLines(configPath, new[]
                {
                    "Turistička Agencija",
                    ""
                });
            }

            string[] lines = File.ReadAllLines(configPath);
            AgencyName = lines.Length > 0 ? lines[0].Trim() : "Turistička Agencija";
            ConnectionString = lines.Length > 1 ? lines[1].Trim() : string.Empty;
        }
    }
}


