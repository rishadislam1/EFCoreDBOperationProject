using EFCoreDBOperationProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        [HttpGet("eagerLoading")]
        public async Task<IActionResult> GetBooksEagerLoadingAsync()
        {
            var books = await appDbContext.Books.Include(x => x.Author).ToListAsync();
            return Ok(books);
        }
        [HttpGet("explicitLoading")]
        public async Task<IActionResult> GetBooksExplicitLoadingAsync()
        {
            var book = await appDbContext.Books.FirstAsync();
            await appDbContext.Entry(book).Reference(x => x.Author).LoadAsync();
           
            return Ok(book);
        }

        [HttpGet("lazyLoading")]
        public async Task<IActionResult> GetBooksLazyLoadingAsync()
        {
// for adding lazyloading first install microsoft.entityframeworkcore.proxies. then add in program.cs with options. then add virtual with all class connected with foreign keys.

            var book = await appDbContext.Books.FirstAsync();

            var author = book.Author;

            return Ok(book);
        }


        [HttpGet("language")]
        public async Task<IActionResult> GetBooksLanguageAsync()
        {
            var languages = await appDbContext.Languages.ToListAsync();
            foreach(var language in languages)
            {
                await appDbContext.Entry(language).Collection(x => x.Books).LoadAsync();
            }

            return Ok(languages);
        }

        // using sql queries
        [HttpGet("usingSqlQueries")]
        public async Task<IActionResult> GetBooksUsingSqlQueriesAsync()
        {
            
            var books = await appDbContext.Books.FromSql($"select * from Books").ToListAsync();

            return Ok(books);
        }

        // using store procedure
        [HttpGet("usingsp")]
        public async Task<IActionResult> GetBooksUsingspAsync()
        {
            var parameter = new SqlParameter("@BookId", 1);
            var books = await appDbContext.Books
                .FromSql($"EXEC SP_GetAllBooks {parameter}").ToListAsync();

            return Ok(books);
        }

        // using withoutEFCoreClass
        [HttpGet("withoutEFCoreClass")]
        public async Task<IActionResult> GetBooksWithoutEFCoreClassAsync()
        {
            var books = await appDbContext.Database
                .SqlQuery<Books>($"select * from Books").ToListAsync();
            return Ok(books);
        }

        // for update query in sql
        [HttpGet("updateQuery")]
        public async Task<IActionResult> UpdateQueryAsync()
        {
            var books = await appDbContext.Database
                .ExecuteSqlAsync($"update Books set NoOfPages=1000 where Id=1");
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
