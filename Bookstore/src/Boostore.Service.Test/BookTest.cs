using Bookstore.Service.Test.Tools;
using Bookstore;
using Rhetos;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.TestCommon;

namespace Boostore.Service.Test
{
    public class BookTest
    {
        /// <summary>
        /// BookInfo.NumberOfChapters is persisted in a cache table and should be automatically updated
        /// when a chapter is added or deleted.
        /// </summary>
        [Fact]
        public void AutomaticallyUpdateNumberOfChapters()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var book = new Book { Title = Guid.NewGuid().ToString() };
                repository.Bookstore.Book.Insert(book);

                int? readNumberOfChapters() => repository.Bookstore.BookInfo
                    .Query(bi => bi.ID == book.ID)
                    .Select(bi => bi.NumberOfChapters)
                    .Single();

                Assert.Equal(0, readNumberOfChapters());

                var c1 = new Chapter { BookID = book.ID, Heading = "c1" };
                var c2 = new Chapter { BookID = book.ID, Heading = "c2" };
                var c3 = new Chapter { BookID = book.ID, Heading = "c3" };

                repository.Bookstore.Chapter.Insert(c1);
                Assert.Equal(1, readNumberOfChapters());

                repository.Bookstore.Chapter.Insert(c2, c3);
                Assert.Equal(3, readNumberOfChapters());

                repository.Bookstore.Chapter.Delete(c1);
                Assert.Equal(2, readNumberOfChapters());

                repository.Bookstore.Chapter.Delete(c2, c3);
                Assert.Equal(0, readNumberOfChapters());
            }
        }

        [Fact]
        public void CommonMisspellingValidation()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var book = new Book { Title = "x curiousity y" };

                //Assert.Throws<UserException>(() => repository.Bookstore.Book.Insert(book));
                TestUtility.ShouldFail<UserException>(
                    () => repository.Bookstore.Book.Insert(book),
                    "It is not allowed to enter misspelled word");
            }
        }

        /// <summary>
        /// This type of tests verify that there are not issues with deadlocks or incorrect computed data on parallel operations.
        /// This test also demonstrate how to manage (rare) situations when test needs to commit data into database.
        /// </summary>
        [Fact]
        public void ParallelCodeGeneration()
        {
            DeleteUnitTestBooks(); // This test needs to commit changes, so it is required to clean up any remaining previous test data, in case the test was canceled without ClassCleanup.

            // Prepare test data:

            var books = new[]
            {
                // Using specific prefix to reduce chance of conflicts with any existing data.
                new Book { Code = $"{UnitTestBookCodePrefix}+++", Title = Guid.NewGuid().ToString() },
                new Book { Code = $"{UnitTestBookCodePrefix}+++", Title = Guid.NewGuid().ToString() },
                new Book { Code = $"{UnitTestBookCodePrefix}ABC+", Title = Guid.NewGuid().ToString() },
                new Book { Code = $"{UnitTestBookCodePrefix}ABC+", Title = Guid.NewGuid().ToString() }
            };

            // Insert in parallel:

            for (int retry = 0; retry < 3; retry++) // Running the test multiple times to avoid false positive, since the results are nondeterministic.
            {
                Parallel.ForEach(books, book =>
                {
                    // Each scope represent one web request of the main application, executed in its own separate transaction.
                    // The main application should support parallel web requests.
                    using (var scope = TestScope.Create())
                    {
                        var repository = scope.Resolve<Common.DomRepository>();
                        repository.Bookstore.Book.Insert(book);
                        scope.CommitAndClose(); // Changes are committed to database, to make the test with parallel transactions more realistic.
                    }
                });

                // Review the inserted data:

                using (var scope = TestScope.Create())
                {
                    var repository = scope.Resolve<Common.DomRepository>();
                    var booksFromDb = repository.Bookstore.Book.Load(book => book.Code.StartsWith(UnitTestBookCodePrefix));
                    Assert.Equal(
                        $"{UnitTestBookCodePrefix}001, {UnitTestBookCodePrefix}002, {UnitTestBookCodePrefix}ABC1, {UnitTestBookCodePrefix}ABC2",
                        TestUtility.DumpSorted(booksFromDb, book => book.Code));
                }

                DeleteUnitTestBooks();
            }
        }

            private const string UnitTestBookCodePrefix = "UnitTestBooks";

        private void DeleteUnitTestBooks()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();
                var testBooks = repository.Bookstore.Book.Load(book => book.Code.StartsWith(UnitTestBookCodePrefix));
                repository.Bookstore.Book.Delete(testBooks);
                scope.CommitAndClose();
            }
        }
    }
}
