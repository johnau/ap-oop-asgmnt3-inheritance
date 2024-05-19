using TaskManager.App.UWP.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TaskManager.App.UWP.Views
{
    public sealed partial class TwoPaneViewPage : Page
    {
        public TwoPaneViewViewModel ViewModel { get; } = new TwoPaneViewViewModel();

        public TwoPaneViewPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(twoPaneView);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
