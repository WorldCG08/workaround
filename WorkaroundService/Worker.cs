using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WorkaroundService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string sourceDir = @"D:\wd\learning\C#\Workaround\Workaround\bin\Release\netcoreapp3.1";
        private readonly string backupDir = @"D:\BACKUPS";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string[] dbs = Directory.GetFiles(sourceDir, "*.db");

                foreach (var db in dbs)
                {
                    // Remove path from the file name.
                    string fName = db.Substring(sourceDir.Length + 1);
                    var time24 = DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss");
                    
                    // Use the Path.Combine method to safely append the file name to the path.
                    // Will overwrite if the destination file already exists.
                    File.Copy(Path.Combine(sourceDir, fName), Path.Combine(backupDir, time24 + "_" + fName), true);
                }
                
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                // save every hour
                // await Task.Delay(3600000, stoppingToken);
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
