using AppFundacion.Models;
using Microsoft.EntityFrameworkCore;
using UraniumUI.Pages;

namespace AppFundacion
{
    public partial class MainPage : UraniumContentPage
    {
        int count = 0;
        private readonly AppDbContext db = new AppDbContext();
        public MainPage()
        {
            InitializeComponent();
            
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;
            ProbarConexion();
            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void ProbarConexion()
        {
            try
            {
                db.Database.OpenConnection();
                db.Database.CloseConnection();
                DisplayAlert("Conexión Exitosa", "La conexión con SQL Server fue exitosa", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"No se pudo conectar a la base de datos: {ex.Message}", "OK");
            }
        }

        private void OnDarkModeButtonClicked(object sender, EventArgs e)
        {
            // Cambiar al tema oscuro
            ((App)Application.Current).SetTheme(AppTheme.Dark);
        }

        private void OnLightModeButtonClicked(object sender, EventArgs e)
        {
            // Cambiar al tema oscuro
            ((App)Application.Current).SetTheme(AppTheme.Light);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}
