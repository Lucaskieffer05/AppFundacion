using AppFundacion.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFundacion.Controllers
{
    class CobradorController
    {
        private readonly FundacionContext _context;

        public CobradorController(FundacionContext context)
        {
            _context = context;
        }

        public async Task<List<Cobrador>> GetAllCobradores()
        {
            try
            {
                return await _context.Cobradores.Include(static c => c.IdZonaNavigation).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        public async Task<Dictionary<string, List<Donante>>> DonantesPorCobrador()
        {
            try
            {
                var donantes = await _context.Donantes
                    .Include(d => d.IdCobradorNavigation)
                    .ToListAsync();

                // Agrupar donantes por cobrador y ordenar por IdCobradorNavigation.codigo

                Dictionary<string, List<Donante>> DonantesPorCobrador = donantes
                    .Where(d => d.IdCobradorNavigation != null) // Asegurar que haya cobrador asignado
                    .GroupBy(d => d.IdCobradorNavigation!.CodigoNombre) // Agrupar por Nombre del cobrador
                    .OrderBy(group => group.First().IdCobradorNavigation!.Codigo) // Ordenar los grupos por Codigo del cobrador
                    .ToDictionary(
                        group => group.Key,
                        group => group.OrderBy(d => d.IdCobradorNavigation!.Codigo).ToList() // Ordenar los donantes dentro de cada grupo por Codigo del cobrador
                    );

                return DonantesPorCobrador;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return new Dictionary<string, List<Donante>>();
            }
        }

    }
}
