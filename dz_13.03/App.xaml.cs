using dz_13._03.Models;
using dz_13._03.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;

namespace dz_13._03
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                using (var db = new AuthorBookContext())
                {
                    var authors = from g in db.Authors
                                 select g;
                    var books = from st in db.Books
                                   select st;
                    MainWindow view = new MainWindow();
                    MainViewModel viewModel = new MainViewModel(authors, books);
                    view.DataContext = viewModel;
                    view.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}
