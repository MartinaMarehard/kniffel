using System.Windows;
using Kniffel.views;

namespace Kniffel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var login = new LoginWindow(); 
            bool? result = login.ShowDialog();
            Console.WriteLine($"[DEBUG] Login result: {result}");

            if (result == true)
            {
                var mainWindow = new MainWindow();
                MainWindow = mainWindow;
                mainWindow.ShowDialog();
                //mainWindow.Focus();
            }
            else
            {
                MessageBox.Show("Login wurde abgebrochen.");
                Shutdown(); // Benutzer hat Login abgebrochen → App schließen
            }
        }
    }
}
