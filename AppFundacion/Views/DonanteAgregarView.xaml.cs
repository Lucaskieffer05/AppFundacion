using AppFundacion.ViewModels;
using UraniumUI.Pages;

namespace AppFundacion.Views;

public partial class DonanteAgregarView : UraniumContentPage
{

    private DonanteAgregarViewModel? ViewModel => BindingContext as DonanteAgregarViewModel;
    public DonanteAgregarView()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel is not null)
            await ViewModel.CargarCobradoresAsync();
    }
}