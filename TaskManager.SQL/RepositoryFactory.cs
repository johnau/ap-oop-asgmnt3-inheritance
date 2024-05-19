using System;
using System.Collections.Generic;
using System.Text;
using TaskManagerCore.SQL.Sqlite.Dao;
using TaskManagerCore.SQL.Sqlite;
using TaskManagerCore.Model.Repository;

namespace TaskManager.SQL
{
    /// <summary>
    /// 
    /// </summary>
    public class RepositoryFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFolderPath"></param>
        /// <returns></returns>
        public static (ITaskDataRepository taskRepoSql, ITaskFolderRepository folderRepoSql) BuildRepositories(string dataFolderPath)
        {
            var dbContext = new SqliteContext(dataFolderPath);

            var taskDataDaoSql = new TaskDataSqlDao(dbContext);
            var taskRepoSql = new TaskDataSqlRepository(taskDataDaoSql);

            var taskFolderDaoSql = new TaskFolderSqlDao(dbContext);
            var folderRepoSql = new TaskFolderSqlRepository(taskFolderDaoSql);

            return (taskRepoSql, folderRepoSql);
        }
    }
}
