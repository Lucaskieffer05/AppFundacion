using System;
using System.Collections.Generic;

namespace AppFundacion.Models;

public partial class Donante
{
    public int Id { get; set; }

    public string? Dni { get; set; }

    public string NombreApellido { get; set; } = null!;

    public string? Ciudad { get; set; }

    public string? Provincia { get; set; }

    public string? Pais { get; set; }

    public DateTime? FechaIngreso { get; set; }

    public int Monto { get; set; }

    public int IdCobrador { get; set; }

    public string? Domicilio { get; set; }

    public virtual Cobrador IdCobradorNavigation { get; set; } = null!;
}
