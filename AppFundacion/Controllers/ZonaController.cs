using AppFundacion.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFundacion.Controllers
{
    class ZonaController
    {
        private readonly FundacionContext _context;

        public ZonaController(FundacionContext context)
        {
            _context = context;
        }

        //Obtener todas las zonas
        public async Task<List<Zona>> GetAllZonas() 
        {
            try
            {
                return await _context.Zonas.ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Método para obtener la cantidad de donantes por zona
        public async Task<Dictionary<string, int>> GetDonantesPorZona()
        {
            try
            {
                return await _context.Zonas
               .Include(z => z.Cobradores)
               .ThenInclude(c => c.Donantes)
               .Select(z => new
               {
                   Zona = z.Nombre ?? "N/A",
                   CantidadDonantes = z.Cobradores.SelectMany(c => c.Donantes).Count()
               })
               .ToDictionaryAsync(z => z.Zona, z => z.CantidadDonantes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<string, int>();
            }
           
        }

    }
}
