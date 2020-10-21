using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DatabaseMigrationToolASPNETCore
{
    public class Program
    {
        private static IConfigurationRoot _config;

        public static void Main(string[] args)
        {
            _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            MigrateDatabase();

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }

        private static void MigrateDatabase()
        {
            var cnstr = _config.GetConnectionString("myDb1");

            try
            {
                var cnx = new SqlConnection(cnstr);
                var evolve = new Evolve.Evolve(cnx)
                {
                    Locations = new[] { "db/migrations", "db/datasets" },
                    IsEraseDisabled = true,

                };

                evolve.Migrate();
            }
            catch (Exception ex)
            {

            }
        }

        private static void MigrateDatabaseTargetVersion()
        {
            var cnstr = _config.GetConnectionString("myDb1");
            string targetMigrationVersion = _config.GetSection("MySettings").GetSection("TargetMigrationVersion").Value;

            try
            {
                var cnx = new SqlConnection(cnstr);
                var evolve = new Evolve.Evolve(cnx)
                {
                    Locations = new[] { "db/migrations", "db/datasets" },
                    IsEraseDisabled = true,

                };

                Evolve.Migration.MigrationVersion version = new Evolve.Migration.MigrationVersion(targetMigrationVersion);

                evolve.TargetVersion = version;

                evolve.Migrate();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
