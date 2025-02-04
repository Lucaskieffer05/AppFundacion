using System;
using System.Collections.Generic;

namespace AppFundacion.Models;

public partial class Zona
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Cobrador> Cobradores { get; set; } = new List<Cobrador>();
}
