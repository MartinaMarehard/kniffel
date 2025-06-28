using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Kniffel.models;

namespace Kniffel.views
{
    public partial class DiceView : UserControl
    {
        private readonly Random rng = new();
        private const int maxRolls = 3;
        private readonly List<Die> Dice = new();
        private GameState? gameState;

        public DiceView()
        {
            InitializeComponent();
            for (int i = 0; i < 5; i++)
            {
                Dice.Add(new Die { Value = 1, IsHeld = false });
            }
            UpdateDiceDisplay();
            this.DataContextChanged += DiceView_DataContextChanged;
        }

        private void DiceView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is GameState state)
            {
                gameState = state;
                gameState.OnResetRequested += ResetDiceView;
            }
        }

        private void RollDice(object sender, RoutedEventArgs e)
        {
            if (gameState == null) throw new InvalidOperationException("No game state in DiceView");

            gameState.RollCount++;

            if (gameState.RollCount >= maxRolls)
                reachedMaxRolls();

            foreach (var die in Dice)
            {
                if (!die.IsHeld)
                    die.Value = rng.Next(1, 7);
            }
            gameState.CurrentDice = Dice.Select(d => new Die { Value = d.Value, IsHeld = d.IsHeld }).ToList();
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
                if (gameState?.RollCount > 0)
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
            RollCounterText.Text = gameState != null
                ? $"Würfe: {gameState.RollCount} / {maxRolls}"
                : "Würfe: 0 / 3";
        }

        private void reachedMaxRolls()
        {
            RollButton.Visibility = Visibility.Collapsed;
        }
        
        private void ResetDiceView()
        {
            if (gameState == null) return;
            gameState.RollCount = 0;
            RollButton.Visibility = Visibility.Visible;

            foreach (var die in Dice)
            {
                die.IsHeld = false;
                die.Value = 1;
            }
            UpdateRollCount();
            UpdateDiceDisplay();
        }
    }
}
