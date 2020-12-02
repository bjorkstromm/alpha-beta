using System.Windows;
using System.Windows.Input;
using AlphaBeta.ViewModels;

namespace AlphaBeta
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            await vm.NextWord();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) && Keyboard.IsKeyDown(Key.LeftAlt))
            { 
                if(WindowState == WindowState.Maximized)
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Normal;
                }
                else
                {
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                }

                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }
    }
}
