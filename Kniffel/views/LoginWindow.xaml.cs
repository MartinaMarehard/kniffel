using System;
using System.Windows;
using Kniffel.models;
using Kniffel.services;

namespace Kniffel.views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var service = new UserService();
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Benutzername und Passwort erforderlich.");
                return;
            }

            if (!service.UserExists(username))
            {
                MessageBox.Show("Benutzer nicht gefunden.");
                return;
            }

            if (!service.VerifyPassword(username, password))
            {
                MessageBox.Show("Falsches Passwort.");
                return;
            }

            // Login erfolgreich
            DialogResult = true;
            Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            register.Owner = this;
            register.ShowDialog();
        }
    }
}