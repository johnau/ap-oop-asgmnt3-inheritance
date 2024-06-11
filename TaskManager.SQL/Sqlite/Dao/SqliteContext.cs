using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using TaskManagerCore.SQL.Sqlite.Entity;

namespace TaskManagerCore.SQL.Sqlite.Dao
{
    /// <summary>
    /// 
    /// </summary>
    public class SqliteContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public DbSet<TaskFolderEntityV2> Folders { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DbSet<TaskDataEntityV2> Tasks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DbPath { get; }

        /// <summary>
        /// 
        /// </summary>
        public SqliteContext()
            :this(null)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFolderPath"></param>
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

            Database.EnsureCreated();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
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