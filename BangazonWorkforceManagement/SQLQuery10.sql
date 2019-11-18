	SELECT d.Name, Count(*)  employee_count, d.Budget
                    FROM Department d
                    LEFT JOIN Employee e ON e.DepartmentId = d.Id
                    GROUP BY d.Name, d.Budget