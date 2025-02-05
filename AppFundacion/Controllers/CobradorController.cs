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


    }
}
