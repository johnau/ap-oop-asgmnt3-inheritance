﻿using System;

using Microsoft.Toolkit.Uwp.UI.Animations;

using TaskManager.App.UWP.Services;
using TaskManager.App.UWP.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TaskManager.App.UWP.Views
{
    public sealed partial class ContentGridDetailPage : Page
    {
        public ContentGridDetailViewModel ViewModel { get; } = new ContentGridDetailViewModel();

        public ContentGridDetailPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
            if (e.Parameter is long orderID)
            {
                await ViewModel.InitializeAsync(orderID);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }
}