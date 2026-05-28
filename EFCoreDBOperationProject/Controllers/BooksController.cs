using EFCoreDBOperationProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDBOperationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController(AppDbContext appDbContext) : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> GetBooksAsync()
        {
            var books = await appDbContext.Books.Select(x=> new 
            {
                x.Id,
                MyBookTitle = x.Title,
                Author = x.Author !=null? x.Author.Name: "N/A"
            }).ToListAsync();
            return Ok(books);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddNewBook([FromBody] Books model)
        {
            appDbContext.Books.Add(model);
            await appDbContext.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> AddBooks([FromBody] List<Books> model)
        {
            appDbContext.Books.AddRange(model);
            await appDbContext.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdateBook([FromBody] Books model)
        {
            //var book = await appDbContext.Books.FirstOrDefaultAsync(x => x.Id == bookId);
            //if (book == null)
            //{
            //    return NotFound();
            //}
            //book.Title = model.Title;
            //book.Description = model.Description;
            //book.NoOfPages = model.NoOfPages;
            appDbContext.Books.Update(model);
            await appDbContext.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("bulk")]
        public async Task<IActionResult> UpdateBookInbulk()
        {
            await appDbContext.Books.ExecuteUpdateAsync(x => x
            .SetProperty(p => p.Title, "Book Title")
            .SetProperty(p=>p.Description,"Hello Description")
            );
             return Ok();
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBookByIdAsync([FromRoute] int bookId)
        {
            var book = await appDbContext.Books.FindAsync(bookId);
            if(book == null)
            {
                return NotFound();
            }
            appDbContext.Books.Remove(book);
            await appDbContext.SaveChangesAsync();

            return Ok();

        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteBooksInBulkAsync()
        {
            var books = await appDbContext.Books.Where(x => x.Id < 5).ToListAsync();
            appDbContext.Books.RemoveRange(books);
            await appDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("bulkWithTime")]
        public async Task<IActionResult> DeleteBooksInbulkWithTimeAsync()
        {
            var books = await appDbContext.Books.Where(x => x.Id < 5).ExecuteDeleteAsync();


            return Ok();
        }

        [HttpDelete("bulkDeleteFromUI")]
        public async Task<IActionResult> bulkDeleteFromUIAsync([FromBody] List<int> bookIds)
        {
            if (bookIds == null || !bookIds.Any())
            {
                return BadRequest("No book ids provided.");
            }

            var deletedCount = await appDbContext.Books
                .Where(x => bookIds.Contains(x.Id))
                .ExecuteDeleteAsync();

            return Ok(new
            {
                Message = $"{deletedCount} books deleted successfully."
            });
        }

    }
}
