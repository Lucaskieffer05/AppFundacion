namespace AppFundacion.Views;

using AppFundacion.ViewModels;
using UraniumUI.Pages;
public partial class DonantesView : UraniumContentPage
{

    private DonantesViewModel? ViewModel => BindingContext as DonantesViewModel;
    private CancellationTokenSource _cts;
    public DonantesView()
	{
		InitializeComponent();
        _cts = new CancellationTokenSource();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel is not null)
            await ViewModel.CargarDonantesAsync();
    }
    private async void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is DonantesViewModel viewModel)
        {
            _cts.Cancel(); // Cancelamos el token anterior
            _cts = new CancellationTokenSource(); // Creamos un nuevo token

            try
            {
                await Task.Delay(500, _cts.Token); // Esperamos 1 segundo
                viewModel.FiltrarDonantes(); // Llamamos al m�todo FiltrarDonantes
            }
            catch (TaskCanceledException)
            {
                // Ignoramos la excepci�n si la tarea fue cancelada
            }
        }
    }


}