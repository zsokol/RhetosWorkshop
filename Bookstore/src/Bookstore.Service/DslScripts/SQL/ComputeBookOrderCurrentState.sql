SELECT
    bo.ID,
    StatusID = lastEvent.NewStatusID
FROM
    Bookstore.BookOrder bo
    OUTER APPLY
    (
        SELECT TOP 1 *
        FROM Bookstore.BookOrderEvent oe
        WHERE oe.BookOrderID = bo.ID
        ORDER BY oe.EffectiveSince DESC
    ) lastEvent