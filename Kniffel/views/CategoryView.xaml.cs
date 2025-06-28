using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Kniffel.models;
using Kniffel.Session;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Input;
using Kniffel.models;
using Kniffel.services;

namespace Kniffel.views
{
    public enum StreetSize
    {
        Small,
        Big
    }

    public partial class CategoryView : UserControl
    {
        private GameState? gameState;
        public CategoryView()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is GameState state)
                gameState = state;
        }
        
        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (gameState == null) return;
            if (sender is not Button btn || btn.Tag is not string category) return;

            if (gameState.UsedCategory.Contains(category)) return;
            if (gameState.RollCount == 0) return;

            int score = category switch
            {
                "Einsen" => CalcNumer(1),
                "Zweien" => CalcNumer(2),
                "Dreien" => CalcNumer(3),
                "Vieren" => CalcNumer(4),
                "Fünfen" => CalcNumer(5),
                "Sechsen" => CalcNumer(6),
                "Dreierpasch" => CalcPasch(3),
                "Viererpasch" => CalcPasch(4),
                "Full House" => CalcFullHouse(),
                "Kleine Straße" => CalcStrasse(StreetSize.Small),
                "Große Straße" => CalcStrasse(StreetSize.Big),
                "Chance" => CalcChance(),
                "Kniffel" => CalcKniffel(),
                _ => 0
            };

            btn.Content = score.ToString();
            btn.IsEnabled = false;
            gameState.UsedCategory.Add(category);

            UpdateZusatzfelder(); // ⬅️ Neu: Werte aktualisieren
            gameState.RequestReset();
            
            if (gameState.UsedCategory.Count == 13)
            {
                int finalScore = CalcGesamt() + CalcBonus();
    
                // Spiel speichern mit User aus Session
                if (Session.SessionManager.IsLoggedIn)
                {
                    GameService.SaveGameResult(Session.SessionManager.CurrentUser.Id, finalScore);
                    MessageBox.Show($"Spielstand gespeichert!\nPunkte: {finalScore}", "Fertig!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Spiel beendet, aber kein Benutzer eingeloggt – nichts gespeichert.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                ResetCategories();
            }


        }

        private void UpdateZusatzfelder()
        {
            int summe = CalcSumme();
            int bonus = CalcBonus();
            int gesamt = CalcGesamt() + bonus;

            Summe.Text = summe.ToString();
            Bonus.Text = bonus.ToString();
            Gesamt.Text = gesamt.ToString();
        }


        private int CalcNumer(int number)
        {
            if (gameState == null) return 0;
            return gameState.CurrentDice.Where(d => d.Value == number).Sum(d => d.Value);
        }

        private int CalcPasch(int PaschNumber)
        {
            if (gameState == null) return 0;
            var grouped = gameState.CurrentDice.GroupBy(d => d.Value);
            if (grouped.Any(g => g.Count() >= PaschNumber))
            {
                return gameState.CurrentDice.Sum(d => d.Value);
            }
            return 0;
        }

        private int CalcFullHouse()
        {
            if (gameState == null) return 0;
            var grouped = gameState.CurrentDice.GroupBy(d => d.Value);
            return grouped.Any(g => g.Count() == 2) && grouped.Any(g => g.Count() == 3) ? 25 : 0;
        }

        private int CalcStrasse(StreetSize size)
        {
            if (gameState == null) return 0;
            var values = gameState.CurrentDice.Select(d => d.Value).Distinct().OrderBy(v => v).ToList();
            List<List<int>> possibleStreets = new();
            if (size == StreetSize.Small)
            {
                possibleStreets = new List<List<int>>
                {
                    new List<int> {1,2,3,4},
                    new List<int> {2,3,4,5},
                    new List<int> {3,4,5,6}
                };
            }
            else if (size == StreetSize.Big)
            {
                possibleStreets = new List<List<int>>
                {
                    new List<int> {1,2,3,4,5},
                    new List<int> {2,3,4,5,6}
                };
            }
            foreach (var straße in possibleStreets)
            {
                if (straße.All(values.Contains))
                    return size == StreetSize.Small ? 30 : 40;
            }
            return 0;
        }


        private int CalcChance()
        {
            if (gameState == null) return 0;
            return gameState.CurrentDice.Sum(d => d.Value);
        }

        private int CalcKniffel()
        {
            if (gameState == null) return 0;
            return gameState.CurrentDice.GroupBy(d => d.Value).Any(g => g.Count() == 5) ? 50 : 0;
        }
        
        private int CalcBonus()
        {
            int summe = CalcSumme();
            return summe >= 63 ? 35 : 0;
        }
        
        private int CalcSumme()
        {
            if (gameState == null)
            {
                Console.WriteLine("GameState ist null");
                return 0;
            }

            var topCategories = new[] { "Einsen", "Zweien", "Dreien", "Vieren", "Fünfen", "Sechsen" };

            var buttons = FindVisualChildren<Button>(CategoryGrid)
                .Where(b =>
                    topCategories.Contains(b.Tag?.ToString()) &&
                    int.TryParse(b.Content?.ToString(), out _))
                .ToList();
            return buttons.Sum(b => int.Parse(b.Content?.ToString()!));
        }


        
        private int CalcGesamt()
        {
            if (gameState == null) return 0;
            return FindVisualChildren<Button>(CategoryGrid)
                .OfType<Button>()
                .Where(b => int.TryParse(b.Content?.ToString(), out _))
                .Sum(b => int.Parse(b.Content?.ToString()!));
        }


        private IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    yield return tChild;
                foreach (T descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }

        private void ResetCategories()
        {
            foreach (var btn in FindVisualChildren<Button>(CategoryGrid))
            {
                btn.Content = null;
                btn.IsEnabled = true;
            }

            Summe.Text = "";
            Bonus.Text = "";
            Gesamt.Text = "";
            
            gameState?.UsedCategory.Clear();
        }
        
    }
}

