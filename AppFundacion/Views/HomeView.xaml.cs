namespace AppFundacion.Views
{
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private async void OnDonantesButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("DonantesView");
        }
    }
}