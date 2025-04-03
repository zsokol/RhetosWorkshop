SELECT
    b.ID,
    NumberOfTopics = COUNT(bt.TopicID)
FROM
    Bookstore.Book b
    LEFT JOIN Bookstore.BookTopic bt ON bt.BookID = b.ID
GROUP BY
    b.ID