using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using TaskManagerCore.SQL.Sqlite.Entity;

namespace TaskManagerCore.SQL.Sqlite.Dao
{

    internal class SqliteContext : DbContext
    {
        public DbSet<TaskFolderEntityV2> Folders { get; set; }
        public DbSet<TaskDataEntityV2> Tasks { get; set; }

        public string DbPath { get; }

        public SqliteContext()
            :this(null)
        { }

        public SqliteContext(string dataFolderPath)
        {
            var fileName = "task_manager_sqlite.db";
            if (string.IsNullOrWhiteSpace(dataFolderPath))
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                DbPath = Path.Combine(path, fileName);
            } else
            {
                DbPath = Path.Combine(dataFolderPath, fileName);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskFolderEntityV2>(entity =>
            {
                //entity.HasKey(e => e.Id); // specified with annotation in EntityBaseV2
                entity.Ignore(e => e.TaskIds);
                entity.Property(e => e.TaskIdsString).HasColumnName("TaskIds");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}