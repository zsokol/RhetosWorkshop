SELECT
	e.ID,
	NumberOfEducations = COUNT(ed.ID),
	ISNULL(t.TrainingCompletedPercentage,0) TrainingCompletedPercentage 
FROM
	Bookstore.Employee e
	LEFT JOIN Bookstore.Trainee t on t.ID=e.ID
	LEFT JOIN Bookstore.Education ed on e.ID=ed.EmployeeID
GROUP BY
	e.ID, t.TrainingCompletedPercentage