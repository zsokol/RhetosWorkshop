using Common.Queryable;

namespace Bookstore.Service
{
    public static class RatingSystem
    {
        public static ComputeBookRating[] ComputeRating(
            IEnumerable<Guid> booksIds,
            IQueryable<Bookstore_Book> booksQuery,
            IQueryable<Bookstore_Person> personQuery)
        {
            var ratingResults = new List<ComputeBookRating>();

            var booksData = booksQuery.Where(book => booksIds.Contains(book.ID))
                .Select(book => new
                {
                    BookId = book.ID,
                    book.AuthorID,
                    book.Title,
                    IsForeign = book.Extension_ForeignBook != null
                })
                .ToList();

            var top3Names = personQuery.OrderByDescending(p => p.Name.Length)
                .Select(p => p.ID)
                .Take(3)
                .ToHashSet();

            foreach (var book in booksData)
            {
                decimal rating = 0;

                if (book.Title?.IndexOf("super", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    rating += 100;

                if (book.Title?.IndexOf("great", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    rating += 50;

                if (book.IsForeign)
                    rating *= 1.2m;

                if (book.AuthorID != null && top3Names.Contains(book.AuthorID.Value))
                    rating += 1;

                ratingResults.Add(new ComputeBookRating { ID = book.BookId, Rating = rating });
            }

            return ratingResults.ToArray();
        }
    }
}