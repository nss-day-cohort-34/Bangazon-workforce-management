<<<<<<< HEAD
﻿SELECT 
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
=======
﻿2019
select * from EmployeeTraining



SELECT c.Id
FROM Computer c
LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
WHERE ce.Id IS NULL


SELECT ComputerId 
FROM ComputerEmployee
WHERE ComputerId = 35;

select * from computer

SELECT c.Id,
                            c.PurchaseDate,
                            c.DecomissionDate,
                            c.Make,
                            c.Manufacturer
                        FROM Computer c
                        WHERE Id = 1;

						UPDATE Computer
                                            SET PurchaseDate = 2019-11-15,
                                               --DcomissionDate = '',
                                                Make = 'TestMake',
                                                Manufacturer = 'Tstfacturer'
                                            WHERE Id = 37
>>>>>>> master
