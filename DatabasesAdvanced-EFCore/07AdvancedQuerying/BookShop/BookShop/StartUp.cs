namespace BookShop
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using BookShop.Data;
    using BookShop.Initializer;
    using BookShop.Models;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new BookShopContext())
            {
                DbInitializer.ResetDatabase(context);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            string[] resultArr = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            var result = string.Join(Environment.NewLine, resultArr);
            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] booksArr = context.Books
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            var books = string.Join(Environment.NewLine, booksArr);
            return books;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksArr = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => $"{b.Title} - ${b.Price:F2}");

            var books = string.Join(Environment.NewLine, booksArr);
            return books;
        }

        public static string GetBooksNotRealeasedIn(BookShopContext context, int year)
        {
            var booksArr = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            var books = string.Join(Environment.NewLine, booksArr);
            return books;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split().ToArray();

            var booksArr = context.Books
                 .Select(b => new
                 {
                     b.Title,
                     Category = b.BookCategories
                        .Select(bc => bc.Category.Name)
                        .SingleOrDefault()
                 })
                 .Where(b => categories.Any(c => c.ToLower() == b.Category.ToLower()))
                 .ToArray()
                 .OrderBy(b => b.Title);

            var books = string.Join(Environment.NewLine, booksArr.Select(b => b.Title));
            return books;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var booksArr = context.Books
                .Where(b => b.ReleaseDate < dateTime)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    Result = $"{b.Title} - {b.EditionType} - ${b.Price:F2}"
                })
                .ToArray();

            var books = string.Join(Environment.NewLine, booksArr.Select(b => b.Result));
            return books;
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorsArr = context.Authors
                .Where(a => EF.Functions.Like(a.FirstName, $"%{input}"))
                .Select(a => new
                {
                    AuthorNames = $"{a.FirstName} {a.LastName}"
                })
                .OrderBy(b => b.AuthorNames)
                .ToArray();

            var authors = string.Join(Environment.NewLine, authorsArr.Select(b => b.AuthorNames));
            return authors;
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var booksArr = context.Books
                .Where(b => EF.Functions.Like(b.Title, $"%{input}%"))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            var books = string.Join(Environment.NewLine, booksArr);
            return books;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var booksArr = context.Books
                .Where(b => EF.Functions.Like(b.Author.LastName, $"{input}%"))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    BookAndAuthor = $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})"
                })
                .ToArray();

            var books = string.Join(Environment.NewLine, booksArr.Select(b => b.BookAndAuthor));
            return books;
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksArr = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToArray();

            return booksArr.Length;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsArr = context.Authors
                 .OrderByDescending(a => a.Books.Sum(b => b.Copies))
                 .Select(a => new
                 {
                     TotalBooksCopies = $"{a.FirstName} {a.LastName} - {a.Books.Sum(b => b.Copies)}"
                 })
                .ToArray();

            var authors = string.Join(Environment.NewLine, authorsArr.Select(a => a.TotalBooksCopies));
            return authors;
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categoryArr = context.Categories
                .Select(c => new
                {
                    c.Name,
                    CatgoryProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.CatgoryProfit)
                .ToArray();

            var category = string.Join(Environment.NewLine, categoryArr.Select(c => $"{c.Name} ${c.CatgoryProfit:F2}"));
            return category;
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoryArr = context.Categories
                .Select(c => new
                {
                    c.Name,
                    FirstThreeBooks = c.CategoryBooks
                        .Select(cb => new
                        {
                            cb.Book.Title,
                            Date = cb.Book.ReleaseDate.Value,
                        })
                        .OrderByDescending(cb => cb.Date)
                        .Take(3)
                        .ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            StringBuilder builder = new StringBuilder();
            foreach (var category in categoryArr)
            {
                builder.AppendLine($"--{category.Name}");
                foreach (var book in category.FirstThreeBooks)
                {
                    builder.AppendLine($"{book.Title} ({book.Date.Year})");
                }
            }

            var reslut = builder.ToString().Trim();
            return reslut;
        }

        public static void IncreasePrices(BookShopContext context)
        {
            context.Books
              .Where(b => b.ReleaseDate.Value.Year < 2010)
              .ToList()
              .ForEach(b => b.Price += 5);

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            var deletedCount = books.Length;

            context.Books.RemoveRange(books);
            context.SaveChanges();

            return deletedCount;
        }
    }
}
