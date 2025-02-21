using AppFundacion.ViewModels;
using UraniumUI.Pages;

namespace AppFundacion.Views;

public partial class ZonaAgregarView : UraniumContentPage
{
    private ZonaAgregarViewModel? ViewModel => BindingContext as ZonaAgregarViewModel;
    public ZonaAgregarView()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel != null)
        {
            BindingContext = new ZonaAgregarViewModel();
            await ViewModel.CargarListasAsync();
        }
    }

}