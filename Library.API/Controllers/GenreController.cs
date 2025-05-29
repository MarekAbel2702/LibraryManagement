using AutoMapper;
using Library.Application.DTOs;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenreController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public GenreController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var genres = await _context.Genres
                .Include(g => g.Books)
                .ToListAsync();

            return Ok(_mapper.Map<List<GenreDto>>(genres));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) 
        {
            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre == null) 
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<GenreDto>(genre));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]  GenreDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var genre = _mapper.Map<Genre>(dto);
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = genre.Id }, genre);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] GenreDto dto)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null )
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(dto, genre);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null ) 
            {
                return NotFound();
            }
            else
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
