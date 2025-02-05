using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppFundacion.Models;

public partial class Donante : ObservableObject
{
    [Key]
    public int Id { get; set; }

    [Column("DNI")]
    public string? Dni { get; set; }

    [Required]
    [Column("NombreApellido")]
    public string NombreApellido { get; set; } = null!;

    [Column("Ciudad")]
    public string? Ciudad { get; set; }

    [Column("Provincia")]
    public string? Provincia { get; set; }

    [Column("Pais")]
    public string? Pais { get; set; }

    [Column("FechaIngreso")]
    public DateTime? FechaIngreso { get; set; }

    [Required]
    [Column("Monto")]
    public int Monto { get; set; }

    [Column("Domicilio")]
    public string? Domicilio { get; set; }

    [Required]
    [ForeignKey("IdCobrador")]
    public int IdCobrador { get; set; }


    public virtual Cobrador? IdCobradorNavigation { get; set; }
}
