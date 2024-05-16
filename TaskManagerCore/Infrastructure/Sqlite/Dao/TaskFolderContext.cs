using Microsoft.EntityFrameworkCore;
using TaskManagerCore.Infrastructure.Sqlite.Entity;


namespace TaskManagerCore.Infrastructure.Sqlite.Dao;

internal class TaskFolderContext : DbContext
{
    public DbSet<TaskFolderEntity> Folders { get; set; }
    public DbSet<TaskDataEntity> Tasks { get; set; }

    public string DbPath { get; }

    public TaskFolderContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "taskmanager.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

}
