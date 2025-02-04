namespace AppFundacion.Views;

using AppFundacion.ViewModels;
using UraniumUI.Pages;
public partial class DonantesView : UraniumContentPage
{

    private DonantesViewModel? ViewModel => BindingContext as DonantesViewModel;
    public DonantesView()
	{
		InitializeComponent();

	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel is not null)
            await ViewModel.CargarDonantesAsync();
    }
}