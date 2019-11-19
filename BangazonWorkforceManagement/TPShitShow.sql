SELECT t.Id, t.Name, t.StartDate, t.EndDate, t.MaxAttendees
                                          FROM TrainingProgram t
                                     LEFT JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
									     WHERE et.EmployeeId = 4 OR et.TrainingProgramId IN
											  (SELECT t.Id
											  FROM TrainingProgram t
											  LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = t.Id
											  GROUP BY t.Id, t.MaxAttendees
											  HAVING t.MaxAttendees > COUNT(et.TrainingProgramId))
											  OR et.TrainingProgramId IS Null
											  AND t.Id IN
											  (SELECT t.Id
											   FROM TrainingProgram t
											  WHERE t.StartDate > GETDATE())
