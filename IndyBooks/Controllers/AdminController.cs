﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        public AdminController(IndyBooksDataContext db) { _db = db; }

        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook() => View("AddBook");
        [HttpPost]
        public IActionResult CreateBook(AddBookViewModel newBook)
        {
            //TODO: Use ViewModel to build the Author and then the Book; 
            //      Add to DbSets; SaveChanges


            //Shows the new book using the Search Listing 
            return RedirectToAction("Index");
        }
        /***
         * READ
         * TODO: Note the use of a Lambda expression in the method definition        
         */
        [HttpGet]
        public IActionResult Index() => View("SearchResults", _db.Books.Include(b => b.Author));
        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult DeleteBook(long id)
        {
            //TODO: Remove the Book associated with the given id number; Save Changes


            return RedirectToAction("Search");
        }

        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books; // start with entire collection

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                            .Where(b => b.Title.Contains(search.Title))
                            .OrderBy(b => b.Title)
                            ;
            }

            //Author's Last Name Search
            if (search.AuthorLastName != null)
            {
                //Use the Name property of the Book's Author entity
                foundBooks = foundBooks
                            .Include(b => b.Author)
                            .Where(b => b.Author.Name.EndsWith(search.AuthorLastName, StringComparison.CurrentCulture))
                            ;
            }
            //Priced Between Search (min and max price entered)
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice)
                            .Select(b => new Book { Author = b.Author })
                            .Distinct()
                            ;
            }
            //Highest Priced Book Search (only max price entered)
            if (search.MinPrice == 0 && search.MaxPrice > 0)
            {
                decimal max = _db.Books.Max(b => b.Price);
                foundBooks = foundBooks
                            .Where(b => b.Price == max)
                            ;
            }
            //Composite Search Results
            return View("SearchResults", foundBooks.Include(b => b.Author));
        }

    }
}
