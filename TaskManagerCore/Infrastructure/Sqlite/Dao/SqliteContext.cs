using Microsoft.EntityFrameworkCore;
using TaskManagerCore.Infrastructure.Sqlite.Entity;


namespace TaskManagerCore.Infrastructure.Sqlite.Dao;

internal class SqliteContext : DbContext
{
    public DbSet<TaskFolderEntityV2> Folders { get; set; }
    public DbSet<TaskDataEntityV2> Tasks { get; set; }

    public string DbPath { get; }

    public SqliteContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "task_manager.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
