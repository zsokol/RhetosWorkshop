SELECT
    b.ID,
    NumberOfChapters = COUNT(c.ID)
FROM
    Bookstore.Book b
    LEFT JOIN Bookstore.Chapter c ON c.BookID = b.ID
GROUP BY
    b.ID