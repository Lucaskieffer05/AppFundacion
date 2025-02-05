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

        // Actualizar donador
        public async Task<bool> UpdateDonante(Donante donante)
        {
            try
            {
                _context.Entry(donante).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // Obtiene todos los donantes
        public async Task<List<Donante>> GetAllDonantes()
        {
            try
            {
                return await _context.Donantes.Include(d => d.IdCobradorNavigation).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        // Agregar donante
        public async Task<bool> AddDonante(Donante donante)
        {
            try
            {
                _context.Donantes.Add(donante);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Eliminar un donante
        public async Task<bool> DeleteDonante(int id)
        {
            try
            {
                var donante = await _context.Donantes.FindAsync(id);
                if (donante == null)
                {
                    return false;
                }
                _context.Donantes.Remove(donante);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
