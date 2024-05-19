using System;
using System.IO;
using BinaryFileHandler;
using Microsoft.Extensions.DependencyInjection;

using TaskManager.App.UWP.Services;
using TaskManager.App.UWP.ViewModels;
using TaskManager.SQL;
using TaskManagerCore;
using TaskManagerCore.Configuration;
using TaskManagerCore.Controller;
using TaskManagerCore.Infrastructure;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.SQL.Sqlite;
using TaskManagerCore.SQL.Sqlite.Dao;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace TaskManager.App.UWP
{
    public sealed partial class App : Application
    {
        #region DependencyInjection
        private IServiceProvider _serviceProvider;

        public static IServiceProvider Services
        {
            get
            {
                IServiceProvider serviceProvider = ((App)Current)._serviceProvider
                    ?? throw new InvalidOperationException("The service provider is not initialized");
                return serviceProvider;
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            ////var dataFilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tm_data");
            var dataFilesFolderPath = Path.Combine(Path.GetTempPath(), "task-manager", "tm_data");
            //Directory.CreateDirectory(dataFilesFolderPath);
            //var tasksFileConf = new BinaryFileConfig("taskmanager-task-data", dataFilesFolderPath);
            //var folderFileConf = new BinaryFileConfig("taskmanager-folder-data", dataFilesFolderPath);

            //var taskWriter = new TaskDataFileWriter(tasksFileConf);
            //var taskReader = new TaskDataFileReader(tasksFileConf);
            //var folderWriter = new TaskFolderFileWriter(folderFileConf);
            //var folderReader = new TaskFolderFileReader(folderFileConf);

            //var taskDao = new TaskDataDao(taskReader, taskWriter);              // Use this for the BinaryFile namespace class (part2 of task)
            //var taskRepo = new TaskDataRepository(taskDao);

            //var folderDao = new TaskFolderDao(folderReader, folderWriter);      // Use this for the BinaryFile namespace class (part2 of task)
            //var fodlerRepo = new TaskFolderRepository(folderDao);

            //var dbContext = new SqliteContext(dataFilesFolderPath);

            //var taskDataDaoSql = new TaskDataSqlDao(dbContext);
            //var taskRepoSql = new TaskDataSqlRepository(taskDataDaoSql);

            //var taskFolderDaoSql = new TaskFolderSqlDao(dbContext);
            //var folderRepoSql = new TaskFolderSqlRepository(taskFolderDaoSql);

            //var dualTaskRepo = new TaskDataDualRepositoryRunner(taskRepo, taskRepoSql);
            //var dualFolderRepo = new TaskFolderDualRepositoryRunner(fodlerRepo, folderRepoSql);

            //var controller = new TaskController(dualTaskRepo, dualFolderRepo);

            var (secondaryTaskRepo, secondaryFolderRepo) = RepositoryFactory.BuildRepositories(dataFilesFolderPath);

            var controller = TaskManagerFactory.CreateDualDataSourceTaskManager(secondaryTaskRepo, secondaryFolderRepo);

            var provider = new ServiceCollection()
                .AddSingleton(controller)
                .AddTransient<ListDetailsViewModel>()
                .AddTransient<TreeViewViewModel>()
                .BuildServiceProvider(true);

            return provider;
        }
        #endregion

        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            if (_serviceProvider == null)
            {
                _serviceProvider = ConfigureServices();
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }
    }
}
