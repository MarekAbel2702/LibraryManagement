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
    public class AuthorController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public AuthorController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetAllAuthors")]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();

            return Ok(_mapper.Map<List<AuthorDto>>(authors));
        }

        [HttpGet("{id}", Name ="GetAuthorById")]
        public async Task<IActionResult> Get(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null) 
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<AuthorDto>(author));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthorDto dto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            else
            {
                var author = _mapper.Map<Author>(dto);
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = author.Id }, author);
            }
        }

        [HttpPut("{id}")]
        public async Task <IActionResult> Update (int id, [FromBody] AuthorDto dto)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(dto, author);

                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            else
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
