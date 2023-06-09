﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ChurrasTrinca.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurrasTrinca.Pages.Churrascos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly Data.AppDbContext _context;

        public IndexModel(Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Churrasco> Churrasco { get;set; } = default!;

        [BindProperty]
        public Participante Participante { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Churrascos != null)
            {
                Churrasco = await _context.Churrascos.Include(p => p.Participantes).ToListAsync();
            }
        }
    }
}
