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

            Console.WriteLine($"[DEBUG] Username: {username}");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("[DEBUG] Benutzername oder Passwort leer");
                MessageBox.Show("Benutzername und Passwort erforderlich.");
                return;
            }

            if (!service.UserExists(username))
            {
                Console.WriteLine("[DEBUG] Benutzer nicht gefunden");
                MessageBox.Show("Benutzer nicht gefunden.");
                return;
            }

            if (!service.VerifyPassword(username, password))
            {
                Console.WriteLine("[DEBUG] Passwort falsch");
                MessageBox.Show("Falsches Passwort.");
                return;
            }

            var user = service.GetUserByUsername(username);
            Session.SessionManager.Login(user);
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