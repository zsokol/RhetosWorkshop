SELECT
	e.ID,
	e.Code,
	e.FirstName,
	e.LastName,
	e.Iban,
	e.Oib,
	t.TrainingCompletedPercentage
FROM
	Bookstore.Employee e
	INNER JOIN Bookstore.Trainee t on e.ID=t.ID
WHERE
	DATEADD(m,isnull(e.TestPeriod,1),e.WorkStarted)>GETDATE()