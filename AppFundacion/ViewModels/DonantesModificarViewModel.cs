﻿using AppFundacion.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using AppFundacion.Views;
using AppFundacion.Mensajes;
using CommunityToolkit.Mvvm.Messaging;

namespace AppFundacion.ViewModels
{
    [QueryProperty(nameof(DonanteModificar), "donanteModificar")]
    public partial class DonantesModificarViewModel : ObservableObject
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        private readonly CobradorController _cobradorController;
        private readonly DonanteController _donanteController;

        [ObservableProperty]
        private Donante? donanteModificar;

        [ObservableProperty]
        private ObservableCollection<Cobrador> listaCobradores = [];


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public DonantesModificarViewModel()
        {
            _donanteController = new DonanteController(new FundacionContext());
            _cobradorController = new CobradorController(new FundacionContext());
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------


        [RelayCommand]
        async Task VolverAtras()
        {
            try
            {
                await Shell.Current.Navigation.PopAsync();
                //await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                // Registra el error en un archivo de log o muestra un mensaje de error
                // Aquí se muestra un ejemplo de cómo registrar el error en un archivo de log
                string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "error_log.txt");
                logFilePath = "C:\\prueba\\error_log.txt";
                await File.AppendAllTextAsync(logFilePath, $"{DateTime.Now}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");

                // También puedes mostrar un mensaje de error al usuario
                await Shell.Current.DisplayAlert("Error", $"{ex}", "OK");
            }
        }

        [RelayCommand]
        async Task ModficiarDonador()
        {
            if (DonanteModificar== null || DonanteModificar.NombreApellido == null || DonanteModificar.NombreApellido == "")
            {
                await Shell.Current.DisplayAlert("Error", "Debes seleccionar un nombre adecuado", "OK");
                return;
            }

            if (DonanteModificar.Monto <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Debes ingresar un monto adecuado", "OK");
                return;
            }


            if (DonanteModificar == null || DonanteModificar.IdCobradorNavigation == null)
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el donante", "OK");
                return;
            }
            if (DonanteModificar.IdCobrador != DonanteModificar.IdCobradorNavigation.Id)
            {
                DonanteModificar.IdCobrador = DonanteModificar.IdCobradorNavigation.Id;
            }
            var resultado = await _donanteController.UpdateDonante(DonanteModificar);
            if (resultado)
            {
                //await Shell.Current.DisplayAlert("Exito", "Donante modificado con exito", "OK");
                await Shell.Current.GoToAsync("..");
                WeakReferenceMessenger.Default.Send(new DonanteModificadoMessage(true));
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Ocurrió un error al modificar el donante", "OK");
            }
        }


        public async Task CargarCobradoresAsync()
        {
            var cobradoresList = await _cobradorController.GetAllCobradores();
            ListaCobradores = new ObservableCollection<Cobrador>(cobradoresList);
        }
    }
}
