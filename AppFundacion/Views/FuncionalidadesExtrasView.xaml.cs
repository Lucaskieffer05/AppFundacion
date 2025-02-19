using AppFundacion.Models;
using AppFundacion.ViewModels;
using System.Diagnostics;
using UraniumUI.Pages;

namespace AppFundacion.Views;

public partial class FuncionalidadesExtrasView : UraniumContentPage
{
    private FuncionalidadesExtrasViewModel? ViewModel => BindingContext as FuncionalidadesExtrasViewModel;
    private CancellationTokenSource _cts;
    public FuncionalidadesExtrasView()
	{
		InitializeComponent();
        _cts = new CancellationTokenSource();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = new FuncionalidadesExtrasViewModel();
        _cts = new CancellationTokenSource();
        CargarDonantesAlCrear();
    }

    private async void CargarDonantesAlCrear()
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
            Debug.WriteLine($"Error al cargar donantes: {ex.Message}");
        }
    }


    private async void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is FuncionalidadesExtrasViewModel viewModel)
        {
            _cts.Cancel(); // Cancelamos el token anterior
            _cts = new CancellationTokenSource(); // Creamos un nuevo token

            try
            {
                await Task.Delay(500, _cts.Token); // Esperamos 1 segundo
                if (string.IsNullOrEmpty(e.NewTextValue) || !e.NewTextValue.All(char.IsWhiteSpace))
                {
                    viewModel.FiltrarCobradorZona();
                    if (e.NewTextValue != "")
                    {
                        viewModel.FiltrarDonantes(new List<Donante>(viewModel.ListaDonantes));
                    }

                } // Llamamos al método FiltrarDonantes
            }
            catch (TaskCanceledException)
            {
                // Ignoramos la excepción si la tarea fue cancelada
            }
        }
    }

}