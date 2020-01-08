using FlyingLogbook.DataObjects;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyingLogbook.DataPersistence
{
    public class LogbookContext : DbContext
    {
        public LogbookContext() : base("SQLiteConnection") { }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Airfield> Airfields { get; set; }
        public DbSet<ProgramSetting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<LogbookContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        #region Helper Methods

        public ProgramSetting LoadOrAddSetting(string settingKey, int settingNumber = 0)
        {
            var setting = this.Settings.SingleOrDefault(s => s.SettingKey == settingKey && s.SettingNumber == settingNumber);

            if (setting == null)
            {
                setting = new ProgramSetting()
                {
                    SettingKey = settingKey,
                    SettingNumber = settingNumber
                };

                this.Settings.Add(setting);
            }

            return setting;
        }

        #endregion
    }
}
