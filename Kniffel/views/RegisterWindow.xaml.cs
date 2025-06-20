using System;
using System.Windows;
using Kniffel.models;
using Kniffel.Services;

namespace Kniffel.views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter username and password.");
                return;
            }

            var service = new UserService();

            if (service.UserExists(username))
            {
                MessageBox.Show("Username already exists. Please enter a different username.");
                return;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = service.HashPassword(password),
                RegisteredAt = DateTime.Now
            };

            service.AddUser(user);

            MessageBox.Show("Registration was successful.");
            DialogResult = true;
            Close();
        }
    }
}