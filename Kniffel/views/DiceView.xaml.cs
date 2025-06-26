using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Kniffel.models;

namespace Kniffel.views
{
    public partial class DiceView : UserControl
    {
        private readonly Random _random = new();
        private int rollCount = 0;
        private const int maxRolls = 3;
        
        private Random rng = new Random();
        private List<Die> Dice = new();

        public DiceView()
        {
            InitializeComponent();
            for (int i = 0; i < 5; i++)
            {
                Dice.Add(new Die { Value = 1, IsHeld = false });
            }
            UpdateDiceDisplay();
        }
        
        private bool hasRolledOnce = false;

        private void RollDice(object sender, RoutedEventArgs e)
        {
            if (rollCount >= maxRolls)
            {
                return;
            }
            hasRolledOnce = true;
            rollCount++;
            foreach (var die in Dice)
            {
                if (!die.IsHeld)
                    die.Value = rng.Next(1, 7);
            }
            UpdateDiceDisplay();
            UpdateRollCount();
        }

        private void UpdateDiceDisplay()
        {
            DicePanel.Children.Clear();
            foreach (var die in Dice)
            {
                var btn = new Button
                {
                    Content = die.Value.ToString(),
                    Background = die.IsHeld ? Brushes.LightGreen : Brushes.White,
                    Margin = new Thickness(5),
                    Width = 50,
                    Height = 50
                };

                if (hasRolledOnce)
                {
                    btn.Click += (s, e) =>
                    {
                        die.IsHeld = !die.IsHeld;
                        UpdateDiceDisplay();
                    };
                }

                DicePanel.Children.Add(btn);
            }
        }

        private void UpdateRollCount()
        {
            RollCounterText.Text = $"Würfe: {rollCount} / {maxRolls}";
        }
    }
}