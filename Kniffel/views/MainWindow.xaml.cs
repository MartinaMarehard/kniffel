using System.Text;
using System.Windows;
using System.Diagnostics;
using Kniffel.Session;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kniffel.models;

namespace Kniffel.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GameState gameState;
    public MainWindow()
    {
        InitializeComponent();

        gameState = new GameState
        {
            RollCount = 0,
            UsedCategory = new HashSet<string>()
        };

    }
    
    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Sie werden ausgeloggt!", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
        Application.Current.Shutdown();
    }

    private void OpenStatsView_Click(object sender, RoutedEventArgs e)
    {
        //MainContent.Content = new StatsView(); // Später
    }

    private void OpenDiceView_Click(object sender, RoutedEventArgs e)
    {
        var diceView = new DiceView();
        diceView.DataContext = gameState;

        MainContent.Content = diceView;
    }




}