namespace AppFundacion.Views;
using AppFundacion.Models;
using AppFundacion.ViewModels;
using System.IO;
using Microsoft.Maui.Storage;

using System.Text;
using UraniumUI.Pages;

public partial class ReportesView : UraniumContentPage
{

    private ReportesViewModel? ViewModel => BindingContext as ReportesViewModel;
    public int opcionSeleccionada = 0;
    public ReportesView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel is not null)
            await ViewModel.CargarListaDeComponentes();
    }

    public ContentPage GenerarToolbar(WebView webView) 
    {
        var volverButton = new ToolbarItem
        {
            Text = "Volver",
            Command = new Command(() => {
                Navigation.PopAsync();
            }),
            Priority = 0
        };

        var imprimirButton = new ToolbarItem
        {
            Text = "Imprimir",
            Command = new Command(async () =>
            {
                await webView.EvaluateJavaScriptAsync("window.print();");
            }),
            Priority = 1
        };
   
        var page = new ContentPage
        {
            Content = webView,
            ToolbarItems = { volverButton, imprimirButton }
        };

        return page;
    }

    public async void VerReporte(object sender, EventArgs e)
    {
        if (ViewModel is not null) { 

            var htmlContent = ViewModel.SeleccionReporteHtml();
            if(htmlContent == "")
            {
                await Shell.Current.DisplayAlert("Error", "No seleccionó una zona u opción", "OK");
                return;
            }
            // Guardar el HTML en un archivo temporal, para poder mostrarlo en un WebView. Esto ayuda cuando htmlContent es muy largo.
            string filePath = Path.Combine(FileSystem.CacheDirectory, "tarjetas.html");
            File.WriteAllText(filePath, htmlContent);

            var webView = new WebView { Source = new UrlWebViewSource { Url = filePath } };

            ContentPage page = GenerarToolbar(webView);
            await Navigation.PushAsync(page);
        }
    }

    public async void VerTarjetas(object sender, EventArgs e)
    {
        if (ViewModel is not null)
        {
            var htmlContent = ViewModel.SeleccionTarjetasHtml();
            if (string.IsNullOrEmpty(htmlContent))
            {
                await Shell.Current.DisplayAlert("Error", "No seleccionó una zona u opción", "OK");
                return;
            }

            // Guardar el HTML en un archivo temporal, para poder mostrarlo en un WebView. Esto ayuda cuando htmlContent es muy largo.
            string filePath = Path.Combine(FileSystem.CacheDirectory, "tarjetas.html");
            File.WriteAllText(filePath, htmlContent);

            var webView = new WebView { Source = new UrlWebViewSource { Url = filePath } };

            ContentPage page = GenerarToolbar(webView);
            await Navigation.PushAsync(page);
        }
    }


}