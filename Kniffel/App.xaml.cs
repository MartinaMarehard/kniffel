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
            var result = login.ShowDialog();

            if (result == true)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                Shutdown(); // Benutzer hat Login abgebrochen → App schließen
            }
        }
    }
}
