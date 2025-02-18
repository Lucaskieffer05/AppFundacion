using AppFundacion.ViewModels;
using System.Diagnostics;
using UraniumUI.Pages;

namespace AppFundacion.Views;

public partial class CobradorAgregarView : UraniumContentPage
{
    private CobradorAgregarViewModel? ViewModel => BindingContext as CobradorAgregarViewModel;
    public CobradorAgregarView()
	{
		InitializeComponent();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new CobradorAgregarViewModel();
        CargarListasAlCrear();
    }

    private async void CargarListasAlCrear()
    {
        try
        {
            await Task.Run(async () =>
            {
                if (ViewModel is not null)
                    await ViewModel.CargarListasAsync();
            });
        }
        catch (Exception ex)
        {
            // Maneja la excepción (puedes mostrar un mensaje al usuario o loguear el error)
            Debug.WriteLine($"Error al cargar listas: {ex.Message}");
        }
    }

}