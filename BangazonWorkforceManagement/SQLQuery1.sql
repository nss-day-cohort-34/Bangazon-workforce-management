SELECT 
	c.Id,
    c.PurchaseDate,
    c.DecomissionDate,
    c.Make,
    c.Manufacturer, 
	e.FirstName + ' ' + e.LastName AS FullName
FROM Computer c
LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
LEFT JOIN Employee e ON e.Id = ce.EmployeeId

ORDER BY c.Make, c.Manufacturer;