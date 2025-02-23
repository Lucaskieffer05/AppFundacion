using AppFundacion.ViewModels;
using UraniumUI.Pages;

namespace AppFundacion.Views;

public partial class ConfiguracionView : UraniumContentPage
{
    private ConfiguracionViewModel? ViewModel => BindingContext as ConfiguracionViewModel;
    public ConfiguracionView()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new ConfiguracionViewModel();
    }

}