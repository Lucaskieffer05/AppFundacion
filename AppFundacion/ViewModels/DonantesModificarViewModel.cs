using AppFundacion.Controllers;
using CommunityToolkit.Mvvm.ComponentModel;
using AppFundacion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace AppFundacion.ViewModels
{
    public partial class DonantesModificarViewModel : ObservableObject
    {
        private Donante? donante { get; set; }
        public Donante? Donante
        {
            get => donante;
            set
            {
                if (donante != value)
                {
                    donante = value;
                    OnPropertyChanged(); // Notificar que la propiedad ha cambiado
                }
            }
        }
    }
}
