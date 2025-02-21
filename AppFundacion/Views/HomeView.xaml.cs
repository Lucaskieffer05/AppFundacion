using AppFundacion.ViewModels;
using Microsoft.Maui.Controls;

namespace AppFundacion.Views
{
    public partial class HomeView : ContentPage
    {
        private HomeViewModel? ViewModel => BindingContext as HomeViewModel;
        public HomeView()
        {
            InitializeComponent();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel != null)
            {
                await ViewModel.CargarEstadisticas();
            }
        }

    }
}