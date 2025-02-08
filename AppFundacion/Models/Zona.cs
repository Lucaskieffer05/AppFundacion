using System;
using System.Collections.Generic;

namespace AppFundacion.Models;

public partial class Zona
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Cobrador> Cobradores { get; set; } = new List<Cobrador>();

    public override string ToString()
    {
        return Nombre!; // Asegura que el DropdownField muestre el nombre del cobrador
    }
}
