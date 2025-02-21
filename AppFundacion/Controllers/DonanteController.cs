using AppFundacion.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine(ex.Message);
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
                Debug.WriteLine(ex.Message);
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
                Debug.WriteLine(ex.Message);
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
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // Asignar nuevo cobrador al donante
        public async Task<bool> TransferirDonantes(List<Donante> listaDonantes, Cobrador cobradorDestino)
        {
            try
            {
                foreach (var donante in listaDonantes)
                {
                    donante.IdCobrador = cobradorDestino.Id;
                    _context.Entry(donante).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }


        // Método para obtener cobradores sin donantes asignados
        public async Task<List<Cobrador>> GetCobradoresSinDonantes()
        {
            try
            {
                return await _context.Cobradores
                .Include(c => c.Donantes)
                .Where(c => !c.Donantes.Any())
                .ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return [];
            }
            
        }

        // Método para obtener la cantidad total de donantes
        public async Task<int> GetTotalDonantes()
        {
            try 
            {
                return await _context.Donantes.CountAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
            
        }

        // Actualizar montos
        public async Task<(bool, int)> ActualizarMontos(List<Donante> listaDonantes, int? viejoMonto, int? nuevoMonto, bool menorIgual)
        {
            try
            {
                int count = 0;
                foreach (var donante in listaDonantes)
                {
                    if (menorIgual)
                    {
                        if (donante.Monto <= viejoMonto && nuevoMonto != null)
                        {
                            donante.Monto = (int)nuevoMonto;
                            _context.Entry(donante).State = EntityState.Modified;
                            count++;
                        }
                    }
                    else
                    {
                        if (donante.Monto == viejoMonto && nuevoMonto != null)
                        {
                            donante.Monto = (int)nuevoMonto;
                            _context.Entry(donante).State = EntityState.Modified;
                            count++;
                        }
                    }

                }
                await _context.SaveChangesAsync();
                return (true, count);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return (false, 0);
            }
        }
    }
}
