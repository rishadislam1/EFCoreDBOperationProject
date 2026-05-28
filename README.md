# EF Core Database Operations Project

A comprehensive ASP.NET Core project demonstrating **Entity Framework Core** (EF Core) with SQL Server database operations. This project is designed as a learning resource for students to understand how EF Core works, database relationships, data loading strategies, and CRUD operations.

---

## Table of Contents

1. [What is Entity Framework Core?](#what-is-entity-framework-core)
2. [Why Do We Need EF Core?](#why-do-we-need-ef-core)
3. [Project Structure](#project-structure)
4. [Database Schema](#database-schema)
5. [Setup Instructions](#setup-instructions)
6. [Understanding Data Loading Concepts](#understanding-data-loading-concepts)
7. [CRUD Operations](#crud-operations)
8. [SQL Connection Setup](#sql-connection-setup)
9. [API Endpoints](#api-endpoints)
10. [Common Issues & Troubleshooting](#common-issues--troubleshooting)

---

## What is Entity Framework Core?

**Entity Framework Core (EF Core)** is a lightweight, open-source Object-Relational Mapping (ORM) framework for .NET. It allows you to:

- **Map C# classes to database tables** - Write C# code instead of SQL
- **Manage relationships** - Handle one-to-one, one-to-many, and many-to-many relationships easily
- **Query databases** - Use LINQ (Language Integrated Query) instead of SQL
- **Perform CRUD operations** - Create, Read, Update, Delete data with simple methods
- **Manage database schema** - Use Migrations to version control your database

### Simple Example:
```csharp
// Without EF Core (Traditional SQL)
var sql = "SELECT * FROM Books WHERE Id = @id";
var cmd = new SqlCommand(sql, connection);
cmd.Parameters.AddWithValue("@id", 1);

// With EF Core (Much simpler!)
var book = await dbContext.Books.FindAsync(1);
```

---

## Why Do We Need EF Core?

### Problems It Solves:

| Problem | Solution |
|---------|----------|
| **Writing complex SQL queries** | Use LINQ - write queries in C# |
| **Manual object-to-database mapping** | EF Core handles all mapping automatically |
| **Maintaining database schema changes** | Migrations track schema evolution |
| **SQL Injection vulnerabilities** | Parameterized queries by default |
| **Tight coupling between C# and SQL** | Database-agnostic (SQL Server, MySQL, PostgreSQL, etc.) |

### Key Benefits:

✅ **Productivity** - Write less code, accomplish more  
✅ **Type-Safety** - Get compile-time checking for queries  
✅ **Maintainability** - Easy to test and modify  
✅ **Flexibility** - Switch databases without rewriting code  
✅ **Relationship Management** - Navigate between related objects easily  

---

## Project Structure

```
EFCoreDBOperationProject/
├── Data/                          # Database Models & DbContext
│   ├── AppDbContext.cs           # Main database context
│   ├── Books.cs                  # Book entity
│   ├── Author.cs                 # Author entity
│   ├── Language.cs               # Language entity
│   ├── Currency.cs               # Currency entity
│   └── BookPrice.cs              # BookPrice entity (junction table)
├── Controllers/                   # API Endpoints
│   ├── BooksController.cs        # Book CRUD & loading operations
│   └── CurrencyController.cs     # Currency operations
├── Migrations/                    # Database schema versions
│   └── [Migration files]
├── appsettings.json              # Configuration
├── Program.cs                     # Application startup & EF Core setup
└── README.md                      # This file
```

---

## Database Schema

### Entity Relationships:

```
┌─────────────────────────────────────────────────────────┐
│ BOOKS (Main Table)                                      │
├─────────────────────────────────────────────────────────┤
│ • Id (PK)                                               │
│ • Title                                                 │
│ • Description                                           │
│ • NoOfPages                                             │
│ • IsActive                                              │
│ • CreatedOn                                             │
│ • LanguageId (FK) ──→ LANGUAGES                         │
│ • AuthorId (FK) ──→ AUTHORS                             │
└─────────────────────────────────────────────────────────┘
           │                              │
           │                              │
    One-to-Many              One-to-Many (Optional)
           │                              │
           ▼                              ▼
┌─────────────────┐            ┌──────────────────┐
│   LANGUAGES     │            │     AUTHORS      │
├─────────────────┤            ├──────────────────┤
│ • Id (PK)       │            │ • Id (PK)        │
│ • Title         │            │ • Name           │
│ • Description   │            │ • Email          │
└─────────────────┘            └──────────────────┘

        ┌─────────────────────────────────────────┐
        │ BOOKPRICES (Junction/Relationship)      │
        ├─────────────────────────────────────────┤
        │ • Id (PK)                               │
        │ • BooksId (FK) ──→ BOOKS                │
        │ • CurrencyId (FK) ──→ CURRENCIES        │
        │ • Amount                                │
        └─────────────────────────────────────────┘
              │                         │
              │                         │
        Many-to-One            Many-to-One
              │                         │
              ▼                         ▼
        (See BOOKS)        ┌────────────────────┐
                           │   CURRENCIES       │
                           ├────────────────────┤
                           │ • Id (PK)          │
                           │ • Title            │
                           │ • Description      │
                           └────────────────────┘
```

### Entity Models:

#### Books.cs
```csharp
public class Books
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int NoOfPages { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    
    // Foreign Key
    public int LanguageId { get; set; }
    public int? AuthorId { get; set; }  // Optional (nullable)
    
    // Navigation Properties (virtual for lazy loading)
    public virtual Language Language { get; set; }
    public virtual Author Author { get; set; }
}
```

#### Language.cs
```csharp
public class Language
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    // Collection navigation
    public virtual ICollection<Books> Books { get; set; }
}
```

#### Author.cs
```csharp
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

#### Currency.cs & BookPrice.cs
```csharp
// Many-to-Many relationship through BookPrice
public class Currency
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public virtual ICollection<BookPrice> BookPrices { get; set; }
}

public class BookPrice
{
    public int Id { get; set; }
    public int BooksId { get; set; }
    public int CurrencyId { get; set; }
    public int Amount { get; set; }
    
    public virtual Books Books { get; set; }
    public virtual Currency Currency { get; set; }
}
```

---

## Setup Instructions

### Prerequisites:
- **.NET 8 or higher** installed
- **SQL Server** or **SQL Server LocalDB**
- **Visual Studio 2022** or **Visual Studio Code**

### Step 1: Clone the Repository
```bash
git clone https://github.com/rishadislam1/EFCoreDBOperationProject.git
cd EFCoreDBOperationProject
```

### Step 2: Update Connection String

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "AppDb": "Server=YOUR_SERVER_NAME;Database=SampleEF;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Examples:**
- **LocalDB (Default):** `Server=(localdb)\\localdb;Database=SampleEF;Trusted_Connection=True;TrustServerCertificate=True;`
- **SQL Server Express:** `Server=.\\SQLEXPRESS;Database=SampleEF;Trusted_Connection=True;TrustServerCertificate=True;`
- **Remote Server:** `Server=192.168.1.100;Database=SampleEF;User Id=sa;Password=YourPassword;TrustServerCertificate=True;`

### Step 3: Install Required NuGet Packages

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Proxies  # For Lazy Loading
```

### Step 4: Apply Migrations (Create Database)

```bash
# Open Package Manager Console in Visual Studio
Update-Database

# OR using CLI
dotnet ef database update
```

This command:
- Creates the SQL Server database named "SampleEF"
- Creates all tables (Books, Languages, Authors, Currencies, BookPrices)
- Seeds initial data (sample currencies and languages)

### Step 5: Run the Application

```bash
dotnet run
```

The API will be available at: `https://localhost:5001`

---

## Understanding Data Loading Concepts

EF Core provides **three different strategies** to load related data. Choosing the right one is crucial for performance!

### 1. **Eager Loading** - Load Everything Upfront ⚡

**What it does:** Loads related entities along with the main entity in a **single query**.

**How to use:**
```csharp
// Load Books WITH their Authors in one query
var books = await appDbContext.Books
    .Include(x => x.Author)  // ← Eager Loading
    .ToListAsync();

// You can also include nested relationships
var books = await appDbContext.Books
    .Include(x => x.Author)
    .Include(x => x.Language)
    .ToListAsync();
```

**Generated SQL:**
```sql
SELECT b.*, a.* FROM Books b
LEFT JOIN Authors a ON b.AuthorId = a.Id
```

**When to use:**
✅ You know you'll need the related data  
✅ Working with small datasets  
✅ Performance is critical (single query)  

**Pros:**
- Single database query
- No lazy loading issues
- Predictable performance

**Cons:**
- Loading unnecessary data wastes memory
- Can create large result sets

**API Example:**
```csharp
[HttpGet("eagerLoading")]
public async Task<IActionResult> GetBooksEagerLoadingAsync()
{
    var books = await appDbContext.Books.Include(x => x.Author).ToListAsync();
    return Ok(books);
}
```

---

### 2. **Lazy Loading** - Load On-Demand 📌

**What it does:** Related entities are loaded **automatically when you access them**, using virtual properties and proxy objects.

**Setup Required:**

**Step 1:** Install the Proxies package
```bash
dotnet add package Microsoft.EntityFrameworkCore.Proxies
```

**Step 2:** Configure in `Program.cs`
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()  // ← Enable Lazy Loading
           .UseSqlServer(builder.Configuration.GetConnectionString("AppDb"))
);
```

**Step 3:** Mark navigation properties as `virtual`
```csharp
public class Books
{
    public int Id { get; set; }
    public string Title { get; set; }
    
    // Must be virtual for lazy loading!
    public virtual Author Author { get; set; }
    public virtual Language Language { get; set; }
}
```

**How to use:**
```csharp
var book = await appDbContext.Books.FirstAsync();

// Accessing Author triggers a SEPARATE query automatically
var author = book.Author;  // ← Another SQL query happens here!
```

**Generated SQL:**
```sql
-- Query 1: Get the book
SELECT * FROM Books WHERE Id = 1

-- Query 2: Automatically executed when accessing book.Author
SELECT * FROM Authors WHERE Id = @BookAuthorId
```

**When to use:**
✅ You might not need all related data  
✅ Working with large object graphs  
⚠️ Be careful about performance!  

**Pros:**
- Only loads what you need
- Clean code (no Include statements)
- Convenient for interactive apps

**Cons:**
- Multiple database queries (N+1 problem)
- Hard to track performance issues
- Not ideal for APIs with many concurrent requests

**The N+1 Problem:**
```csharp
var books = await appDbContext.Books.ToListAsync();  // 1 query

foreach(var book in books)
{
    var author = book.Author;  // ← This runs N times! (N+1 queries total!)
}
```

**API Example:**
```csharp
[HttpGet("lazyLoading")]
public async Task<IActionResult> GetBooksLazyLoadingAsync()
{
    var book = await appDbContext.Books.FirstAsync();
    var author = book.Author;  // ← Lazy loaded automatically
    return Ok(book);
}
```

---

### 3. **Explicit Loading** - Load When You Decide 🎯

**What it does:** You explicitly load related data **when needed** using `Entry()` and `Reference()` or `Collection()`.

**How to use:**
```csharp
var book = await appDbContext.Books.FirstAsync();

// Explicitly load the Author
await appDbContext.Entry(book)
    .Reference(x => x.Author)  // For single objects
    .LoadAsync();

var author = book.Author;  // Now it's available

// For collections:
var languages = await appDbContext.Languages.ToListAsync();
foreach(var language in languages)
{
    // Explicitly load Books for each language
    await appDbContext.Entry(language)
        .Collection(x => x.Books)  // For collections
        .LoadAsync();
}
```

**Generated SQL:**
```sql
-- Query 1: Get the book
SELECT * FROM Books WHERE Id = 1

-- Query 2: Explicitly loaded
SELECT * FROM Authors WHERE Id = @BookAuthorId
```

**When to use:**
✅ You have conditional logic  
✅ You want explicit control  
✅ Working with disconnected entities  
✅ Advanced scenarios with filtering  

**Pros:**
- Full control over what gets loaded
- Can filter loaded data
- Better than lazy loading for APIs

**Cons:**
- More verbose code
- Still multiple queries if not careful

**Advanced: Load with Filtering**
```csharp
var language = await appDbContext.Languages.FirstAsync();

// Load only active books for this language
await appDbContext.Entry(language)
    .Collection(x => x.Books)
    .Query()
    .Where(b => b.IsActive == true)
    .LoadAsync();
```

**API Example:**
```csharp
[HttpGet("explicitLoading")]
public async Task<IActionResult> GetBooksExplicitLoadingAsync()
{
    var book = await appDbContext.Books.FirstAsync();
    await appDbContext.Entry(book).Reference(x => x.Author).LoadAsync();
    return Ok(book);
}
```

---

### Comparison Table:

| Feature | Eager | Lazy | Explicit |
|---------|-------|------|----------|
| **Queries** | Single | Multiple (N+1 risk) | Single or Multiple |
| **Setup** | None | Requires Proxies | None |
| **Performance** | Best | Worst (N+1 problem) | Good (with control) |
| **Code Clarity** | Clear | Cleanest | Most Verbose |
| **For APIs** | ✅ Best | ❌ Worst | ✅ Good |
| **For Web Apps** | Good | ✅ Best | Good |
| **Virtual Properties** | Not needed | Required | Not needed |

---

## CRUD Operations

### CREATE (Add Data)

**Single Record:**
```csharp
[HttpPost("")]
public async Task<IActionResult> AddNewBook([FromBody] Books model)
{
    appDbContext.Books.Add(model);
    await appDbContext.SaveChangesAsync();
    return Ok(model);
}
```

**Bulk Insert:**
```csharp
[HttpPost("bulk")]
public async Task<IActionResult> AddBooks([FromBody] List<Books> model)
{
    appDbContext.Books.AddRange(model);
    await appDbContext.SaveChangesAsync();
    return Ok(model);
}
```

**Request Example:**
```json
{
  "title": "C# Programming",
  "description": "Learn C# from scratch",
  "noOfPages": 500,
  "isActive": true,
  "createdOn": "2026-05-28",
  "languageId": 1,
  "authorId": 1
}
```

---

### READ (Get Data)

**Get All:**
```csharp
[HttpGet("")]
public async Task<IActionResult> GetBooksAsync()
{
    var books = await appDbContext.Books
        .Select(x => new 
        {
            x.Id,
            x.Title,
            Author = x.Author != null ? x.Author.Name : "N/A"
        })
        .ToListAsync();
    return Ok(books);
}
```

**Get by ID:**
```csharp
var book = await appDbContext.Books.FindAsync(id);
```

**Get with Conditions:**
```csharp
var activeBooks = await appDbContext.Books
    .Where(x => x.IsActive == true)
    .ToListAsync();
```

---

### UPDATE (Modify Data)

**Single Record:**
```csharp
[HttpPut("")]
public async Task<IActionResult> UpdateBook([FromBody] Books model)
{
    appDbContext.Books.Update(model);
    await appDbContext.SaveChangesAsync();
    return Ok(model);
}
```

**Bulk Update:**
```csharp
[HttpPut("bulk")]
public async Task<IActionResult> UpdateBookInbulk()
{
    await appDbContext.Books.ExecuteUpdateAsync(x => x
        .SetProperty(p => p.Title, "New Book Title")
        .SetProperty(p => p.Description, "Updated Description")
    );
    return Ok();
}
```

---

### DELETE (Remove Data)

**Single Record:**
```csharp
[HttpDelete("{bookId}")]
public async Task<IActionResult> DeleteBookByIdAsync([FromRoute] int bookId)
{
    var book = await appDbContext.Books.FindAsync(bookId);
    if(book == null)
        return NotFound();
    
    appDbContext.Books.Remove(book);
    await appDbContext.SaveChangesAsync();
    return Ok();
}
```

**Bulk Delete:**
```csharp
[HttpDelete("bulk")]
public async Task<IActionResult> DeleteBooksInBulkAsync()
{
    var books = await appDbContext.Books.Where(x => x.Id < 5).ToListAsync();
    appDbContext.Books.RemoveRange(books);
    await appDbContext.SaveChangesAsync();
    return Ok();
}
```

---

## SQL Connection Setup

### Understanding the Connection String

```
Server=(localdb)\\localdb;Database=SampleEF;Trusted_Connection=True;TrustServerCertificate=True;
```

| Part | Meaning |
|------|---------|
| `Server` | Where SQL Server is running |
| `Database` | Which database to use |
| `Trusted_Connection=True` | Use Windows Authentication (no password needed) |
| `TrustServerCertificate=True` | Accept self-signed SSL certificates (dev only!) |

### Different Connection Scenarios:

**1. LocalDB (Built-in, Best for Learning):**
```
Server=(localdb)\\localdb;Database=SampleEF;Trusted_Connection=True;TrustServerCertificate=True;
```

**2. SQL Server Express (Free version):**
```
Server=.\\SQLEXPRESS;Database=SampleEF;Trusted_Connection=True;TrustServerCertificate=True;
```

**3. Remote SQL Server (Production):**
```
Server=192.168.1.100;Database=SampleEF;User Id=sa;Password=YourPassword123;Encrypt=true;TrustServerCertificate=false;
```

**4. Azure SQL Database:**
```
Server=tcp:servername.database.windows.net,1433;Initial Catalog=SampleEF;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### How EF Core Connects in Program.cs:

```csharp
// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies()
           .UseSqlServer(builder.Configuration.GetConnectionString("AppDb"))
);
```

**What happens:**
1. Reads connection string from `appsettings.json`
2. Creates DbContext with SQL Server provider
3. Enables lazy loading proxies
4. Registers DbContext in dependency injection

---

## API Endpoints

### Books Controller

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | Get all books with authors |
| GET | `/api/books/eagerLoading` | Eager loading example |
| GET | `/api/books/explicitLoading` | Explicit loading example |
| GET | `/api/books/lazyLoading` | Lazy loading example |
| GET | `/api/books/language` | Load books by language |
| GET | `/api/books/usingSqlQueries` | Execute raw SQL |
| GET | `/api/books/usingsp` | Execute stored procedure |
| POST | `/api/books` | Create single book |
| POST | `/api/books/bulk` | Create multiple books |
| PUT | `/api/books` | Update book |
| PUT | `/api/books/bulk` | Update multiple books |
| DELETE | `/api/books/{id}` | Delete book by ID |
| DELETE | `/api/books/bulk` | Delete multiple books |

### Currency Controller

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/currency` | Get all currencies |
| GET | `/api/currency/{id}` | Get currency by ID |
| GET | `/api/currency/{name}` | Get currency by name |

---

## Advanced Features

### Using Raw SQL Queries:

```csharp
[HttpGet("usingSqlQueries")]
public async Task<IActionResult> GetBooksUsingSqlQueriesAsync()
{
    var books = await appDbContext.Books
        .FromSql($"SELECT * FROM Books")
        .ToListAsync();
    return Ok(books);
}
```

### Using Stored Procedures:

```csharp
[HttpGet("usingsp")]
public async Task<IActionResult> GetBooksUsingspAsync()
{
    var parameter = new SqlParameter("@BookId", 1);
    var books = await appDbContext.Books
        .FromSql($"EXEC SP_GetAllBooks {parameter}")
        .ToListAsync();
    return Ok(books);
}
```

### Change Tracking:

EF Core tracks changes automatically:
```csharp
var book = appDbContext.Books.Find(1);
book.Title = "New Title";
await appDbContext.SaveChangesAsync();  // Automatically updates!
```

### Querying with LINQ:

```csharp
// Filter
var activeBooks = appDbContext.Books.Where(b => b.IsActive);

// Order
var ordered = appDbContext.Books.OrderBy(b => b.Title);

// Join
var booksWithAuthors = from book in appDbContext.Books
                       join author in appDbContext.Authors 
                       on book.AuthorId equals author.Id
                       select new { book.Title, author.Name };

// Group
var grouped = appDbContext.Books
    .GroupBy(b => b.LanguageId)
    .Select(g => new { LanguageId = g.Key, Count = g.Count() });
```

---

## Common Issues & Troubleshooting

### Issue 1: "No database provider specified"
**Error:** `InvalidOperationException: No database provider has been configured for this DbContext.`

**Solution:** Ensure `UseSqlServer()` is called in `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"))
);
```

---

### Issue 2: "Migration pending"
**Error:** `InvalidOperationException: The model backing the 'AppDbContext' context has changed since the database was last migrated.`

**Solution:** Create and apply a new migration:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

---

### Issue 3: "Lazy loading not working"
**Error:** Navigation properties are null even though they should have data.

**Solution:** 
1. Install Proxies package: `dotnet add package Microsoft.EntityFrameworkCore.Proxies`
2. Enable in Program.cs: `options.UseLazyLoadingProxies()`
3. Mark as virtual: `public virtual Author Author { get; set; }`

---

### Issue 4: "N+1 Query Problem"
**Symptom:** Lots of SQL queries being executed when using lazy loading.

**Solution:** Use eager loading instead:
```csharp
// Bad (N+1 queries)
var books = appDbContext.Books.ToList();
foreach(var b in books) { var author = b.Author; }

// Good (1 query)
var books = appDbContext.Books.Include(b => b.Author).ToList();
```

---

### Issue 5: "Connection string not found"
**Error:** `InvalidOperationException: No connection string named 'AppDb' found in configuration.`

**Solution:** Check `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "AppDb": "YOUR_CONNECTION_STRING_HERE"
  }
}
```

---

### Issue 6: "Cannot connect to SQL Server"
**Error:** `SqlException: A network-related or instance-specific error occurred while establishing a connection to SQL Server.`

**Solution:**
1. Verify SQL Server is running
2. Check server name in connection string
3. Test connection using SQL Server Management Studio (SSMS)
4. Ensure Windows credentials have access

---

## Learning Path for Students

### Beginner (Week 1-2):
1. Understand ORM and why EF Core is needed
2. Learn entity models and DbContext
3. Master CRUD operations (Add, Read, Update, Delete)
4. Practice basic queries with LINQ

### Intermediate (Week 3-4):
1. Understand relationships (One-to-Many, Many-to-Many)
2. Learn eager loading with Include()
3. Study migrations and database versioning
4. Practice filtering and sorting data

### Advanced (Week 5-6):
1. Master all three loading strategies
2. Learn explicit loading for complex scenarios
3. Optimize queries and prevent N+1 problems
4. Work with raw SQL and stored procedures

---

## Database Migrations Explained

### What is a Migration?
A migration is a version of your database schema. It tracks changes over time.

### Existing Migrations in This Project:
```
20260522114230_initDB          → Initial database creation
20260522121555_addednewcolumninbook → Added CreatedOn column
20260522133058_addedlanguagetable → Added Language table
20260523163257_addCurrencyTable → Added Currency & BookPrice tables
```

### Creating a New Migration:
```bash
dotnet ef migrations add AddNewColumn
dotnet ef database update
```

This creates a new migration file with Up() and Down() methods:
```csharp
public override void Up(MigrationBuilder migrationBuilder)
{
    // Changes to apply (forward)
}

public override void Down(MigrationBuilder migrationBuilder)
{
    // Changes to revert (backward)
}
```

---

## Next Steps

1. ✅ **Run the project** - Follow setup instructions
2. 📝 **Test all endpoints** - Use Swagger or Postman
3. 📚 **Study the code** - Read through BooksController.cs
4. 🔧 **Modify models** - Add new entities and relationships
5. 🚀 **Build your own** - Create a new project using EF Core

---

## Resources

- **Official EF Core Documentation**: https://learn.microsoft.com/en-us/ef/core/
- **EF Core Tutorials**: https://www.entityframeworktutorial.net/
- **LINQ Tutorial**: https://learn.microsoft.com/en-us/dotnet/csharp/linq/
- **SQL Server LocalDB**: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

---

## License

This project is open source and available for educational purposes.

---

## Author

**Rishad Islam**  
GitHub: [@rishadislam1](https://github.com/rishadislam1)

---

## Final Notes for Students

🎯 **Key Takeaways:**
- EF Core simplifies database operations significantly
- Always choose the right loading strategy for performance
- Understand relationships before writing queries
- Use migrations to track database changes
- Practice with real examples (like this project!)

📌 **Common Pitfalls to Avoid:**
- ❌ Using lazy loading in APIs (causes N+1 queries)
- ❌ Not tracking migrations (database version chaos)
- ❌ Forgetting SaveChangesAsync() (changes not persisted)
- ❌ Overfetching data (performance issues)

✅ **Best Practices:**
- Use eager loading for APIs
- Use lazy loading for interactive web apps
- Create migrations for each schema change
- Always check if related entities are null
- Test queries with actual data

Happy Learning! 🚀
