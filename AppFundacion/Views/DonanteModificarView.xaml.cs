
namespace AppFundacion.Views;
using AppFundacion.ViewModels;
using UraniumUI.Pages;

public partial class DonanteModificarView : UraniumContentPage
{

    private DonantesModificarViewModel? ViewModel => BindingContext as DonantesModificarViewModel;
    public DonanteModificarView()
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