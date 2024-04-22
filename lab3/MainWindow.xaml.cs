using lab3.utils;
using lab3.viewmodel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            bool showStartupMessage = SettingsManager.LoadShowStartupMessageSetting();

            if (showStartupMessage)
            {
                Loaded += MainWindow_Loaded;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Показываем всплывающее окно
            MainViewModel.ShowStartupInfo();
        }

    }
}