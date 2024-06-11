using System;
using System.IO;
using BinaryFileHandler;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.App.UWP.Data;
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
using Windows.Foundation;
using Windows.Security.Cryptography.Core;
using Windows.UI.ViewManagement;
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
            var dataFilesFolderPath = Path.Combine(Path.GetTempPath(), "task-manager", "tm_data");

            var (secondaryTaskRepo, secondaryFolderRepo) = RepositoryFactory.BuildRepositories(dataFilesFolderPath);

            var controller = TaskManagerFactory.CreateDualDataSourceTaskManager(secondaryTaskRepo, secondaryFolderRepo);

            var provider = new ServiceCollection()
                .AddSingleton(controller)
                .AddSingleton<DataLoader>()
                .AddTransient<MainViewModel>()
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
            if (_serviceProvider == null)
            {
                _serviceProvider = ConfigureServices();
            }

            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(900, 600));
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
