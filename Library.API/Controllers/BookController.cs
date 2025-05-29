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
    public class BookController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public BookController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToListAsync();

            return Ok(_mapper.Map<List<BookDto>>(books));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) 
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<BookDto>(book));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] BookDto dto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);  
            }

            var book = _mapper.Map<Book>(dto);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) 
            {
                return NotFound();
            }
            else
            {
                _mapper.Map(dto, book);

                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            else
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
