using FirstAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        static private List<Book> books = new List<Book>
        {
            new Book
            {
                Id = 1,
                Title = "Social Studies",
                Author = "Galileo",
                YearPublished = 1999
            },
            new Book
            {
                Id = 2,
                Title = "Social Studies",
                Author = "Galileo",
                YearPublished = 1999
            },
            new Book
            {
                Id = 3,
                Title = "Social Studies",
                Author = "Galileo",
                YearPublished = 1999
            },
            new Book
            {
                Id = 4,
                Title = "Social Studies",
                Author = "Galileo",
                YearPublished = 1999
            }
        };
        [HttpGet]
        public ActionResult<List<Book>> getBooks()
        {
            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> getBookById(int id)
        {
            var book = books.FirstOrDefault(booke => booke.Id == id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }
        [HttpPost]
        public ActionResult<Book> AddBook(Book newBook)
        {
            if(newBook == null)
            {
                return BadRequest();
            }
            books.Add(newBook);
            return CreatedAtAction(nameof(getBookById), new { id = newBook.Id }, newBook);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, Book updatedBook)
        {
            var book  = books.FirstOrDefault(book => book.Id == id);
            if(book == null)
            {
                return NotFound();
            }
            book.Id = updatedBook.Id;
            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.YearPublished = updatedBook.YearPublished;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var book = books.FirstOrDefault(book => book.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            books.Remove(book);
            return NoContent();
        }

    }
}
