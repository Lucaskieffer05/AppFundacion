using AppFundacion.ViewModels;
using UraniumUI.Pages;

namespace AppFundacion.Views;

public partial class CobradorModificarView : UraniumContentPage
{

    private CobradorModificarViewModel? ViewModel => BindingContext as CobradorModificarViewModel;
    public CobradorModificarView()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel is not null)
            await ViewModel.CargarListasAsync();
    }

}