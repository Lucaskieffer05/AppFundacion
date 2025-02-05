using AppFundacion.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppFundacion.Controllers
{
    public class DonanteController
    {
        private readonly FundacionContext _context;

        public DonanteController(FundacionContext context)
        {
            _context = context;
        }

        // Obtiene todos los donantes
        public async Task<List<Donante>> GetAllDonantes()
        {
            try
            {
                return await _context.Donantes.Include(d => d.IdCobradorNavigation)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }
    }
}
