﻿using System;
using System.Collections.Generic;

namespace AppFundacion.Models;

public partial class Cobrador
{
    public int Id { get; set; }

    public int Codigo { get; set; } 

    public string Nombre { get; set; } = null!;

    public int IdZona { get; set; }

    public virtual ICollection<Donante> Donantes { get; set; } = new List<Donante>();

    public virtual Zona? IdZonaNavigation { get; set; }

    public string CodigoNombre => $"({Codigo}) {Nombre}";

    public override string ToString()
    {
        return CodigoNombre; // Asegura que el DropdownField muestre el nombre del cobrador
    }

}
