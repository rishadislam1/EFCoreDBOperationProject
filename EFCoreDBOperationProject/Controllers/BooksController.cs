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
        public async Task<IActionResult> UpdateBookInbulk([FromBody] Books model)
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

    }
}
