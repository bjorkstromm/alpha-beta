using System.Windows;
using alpha_beta.ViewModel;

namespace alpha_beta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel) DataContext;

            await vm.NextWord();
        }
    }
}
