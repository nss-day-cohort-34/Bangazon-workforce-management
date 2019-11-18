SELECT d.Name, d.Budget, e.Id AS EmployeeId
                    FROM Department d
                    LEFT JOIN Employee e ON e.DepartmentId = d.Id