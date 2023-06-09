﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChurrasTrinca.Data;
using ChurrasTrinca.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChurrasTrinca.Pages.Churrascos
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ChurrasTrinca.Data.AppDbContext _context;

        public EditModel(ChurrasTrinca.Data.AppDbContext context)
        {
            _context = context;
        }

        public Participante? Participante { get; set; }

        [BindProperty]
        public Churrasco Churrasco { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (id == null || _context.Churrascos == null)
            {
                return NotFound();
            }

            var churrasco = await _context.Churrascos.Where(c => c.Id == id).Include(x => x.Participantes).FirstOrDefaultAsync();

            if (churrasco == null)
            {
                return NotFound();
            }

            Churrasco = churrasco;

            if (churrasco.Participantes != null)
            {
                foreach (var participante in churrasco.Participantes.Where(p => p.ChurrascoId == id))
                    Churrasco.SetContribuicaoTotal(participante);
            }

            Churrasco.Participantes = await _context.Participantes.Where(p => p.ChurrascoId == id).Include(x => x.Churrasco).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Entry(Churrasco).State = EntityState.Modified;

            var churrasId = Churrasco.Id;

            var participantes = await _context.Participantes.Where(c => c.ChurrascoId == churrasId).Include(x => x.Churrasco).ToListAsync();

            decimal valorTotal = 0;

            if (participantes != null)
            {
                //INCREMENTA O VALOR DE CONTRIB. DE CHURRAS E BEBIDAS DE ACORDO COM A QUANT. DE PARTICIPANTES DO CHURRASCO
                foreach (var participante in participantes)
                    valorTotal = valorTotal += participante.ValorContribuicaoChurras + participante.ValorContribuicaoBebidas;
            }

            Churrasco.ValorTotalArrecadado = valorTotal;

            try
            {
                await _context.SaveChangesAsync();

                TempData["Msg"] = "O churras foi editado com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChurrascoExists(Churrasco.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Redirect($"Edit?id={Churrasco.Id}");
        }

        private bool ChurrascoExists(int id)
        {
            return (_context.Churrascos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
