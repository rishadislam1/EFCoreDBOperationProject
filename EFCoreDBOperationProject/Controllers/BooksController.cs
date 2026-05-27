using EFCoreDBOperationProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
