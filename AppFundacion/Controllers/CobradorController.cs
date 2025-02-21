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
                return await _context.Cobradores.Include(static c => c.IdZonaNavigation).OrderBy(c => c.Codigo).ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
                Debug.WriteLine($"Error al obtener datos: {ex.Message}");
                return new Dictionary<string, List<Donante>>();
            }
        }

        // Agregar cobrador
        public async Task<bool> AddCobrador(Cobrador cobrador)
        {
            try
            {
                _context.Cobradores.Add(cobrador);
                await _context.SaveChangesAsync();

                // Guardar los cambios nuevamente para actualizar el campo Codigo
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // Eliminar cobrador
        public async Task<bool> DeleteCobrador(int id)
        {
            try
             {
                var cobrador = await _context.Cobradores.FindAsync(id);
                if (cobrador == null)
                {
                    return false;
                }

                _context.Cobradores.Remove(cobrador);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // Verificar si se puede eliminar un cobrador
        public bool VerificarEliminarCobrador(int id)
        {
            try
            {
                var donantes = _context.Donantes
                    .Where(c => c.IdCobrador == id)
                    .ToList();
                return donantes.Count == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // Verificar codigos duplicados
        public bool VerificarCodigoDuplicado(int codigo)
        {
            try
            {
                return _context.Cobradores.Any(c => c.Codigo == codigo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        // Actualizar cobrador
        public async Task<bool> UpdateCobrador(Cobrador cobrador)
        {
            try
            {
                _context.Entry(cobrador).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }


    }
}
