using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using BangazonWorkforceManagement.Models;
using BangazonWorkforceManagement.Models.ViewModels;


namespace BangazonWorkforceManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }




        // GET: Employee
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
             SELECT e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSuperVisor,
                d.Name AS DepartmentName
            FROM Employee e LEFT JOIN Department d ON d.Id = e.DepartmentId
            ORDER BY d.Name, e.LastName, e.FirstName;
        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            Department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName"))                              
                            }
                        };

                        employees.Add(employee);
                    }

                    reader.Close();

                    return View(employees);
                }
            }
        }


        public ActionResult Details(int id)
        {
            var employee = GetEmployeeById(id);
            var computer = GetAssignedComputerByEmployeeId(id);
            var trainingPrograms = GetTrainingProgramsByEmployeeId(id);
            var viewModel = new EmployeeDetailsViewModel()
            {
                Employee = employee,
                Computer = computer,
                TrainingPrograms = trainingPrograms
            };
            return View(viewModel);
        }

       

        // GET: Employees/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeCreateViewModel()
            {
                Departments = GetAllDepartments()
            };
            return View(viewModel);
        }


        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeCreateViewModel viewModel)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Employee
                ( FirstName, LastName, DepartmentId, IsSuperVisor )
                VALUES
                ( @firstName, @lastName, @departmentId, @isSuperVisor )";
                    cmd.Parameters.Add(new SqlParameter("@firstName", viewModel.Employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", viewModel.Employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", viewModel.Employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@isSuperVisor", viewModel.Employee.IsSupervisor));
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            //blows up if employee does not already have computer
            var viewModel = new EmployeeEditViewModel()
            {
                Employee = GetEmployeeById(id),
                SelectedComputerId = GetAssignedComputerByEmployeeId(id).Id,
                Departments = GetAllDepartments(),
                Computers = GetAvailableComputersByEmployeeId(id)
            };

            //BUG : when this runs, viewModel info gets properly populated (see next comment)
            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EmployeeEditViewModel model)
        {
            //BUG: but here, much of the info is missing (ie, EmployeeId)

            try
            {
              Employee updatedEmployee = model.Employee;

              using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                    UPDATE ComputerEmployee 
                    SET 
                    ComputerId = ComputerId,
                    EmployeeId = @id,
                    AssignDate = AssignDate,
                    UnassignDate = GetDate()
                    WHERE EmployeeId = @id;
                
                    UPDATE Employee 
                    SET
                    FirstName = @firstName,
                    LastName = @lastName,
                    DepartmentId = @departmentId,
                    IsSupervisor = @isSupervisor
                    WHERE Id = @id;";

                        cmd.Parameters.Add(new SqlParameter("@firstName", updatedEmployee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", updatedEmployee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", updatedEmployee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", updatedEmployee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"
                    INSERT INTO ComputerEmployee (ComputerId, EmployeeId, AssignDate)
                    VALUES (@computerId, @employeeId, GetDate());
                    ";
                        cmd.Parameters.Add(new SqlParameter("@computerId", model.SelectedComputerId));
                        cmd.Parameters.Add(new SqlParameter("@employeeId", id));
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));

            }
            catch
            {

                model = new EmployeeEditViewModel()
                {
                    Employee = model.Employee,
                    Computer = model.Employee.Computer,
                    Departments = GetAllDepartments(),
                    Computers = GetAvailableComputersByEmployeeId(id),
                };

                return View(model);

            }
        }

        public ActionResult EditTraining(int id)
        {
            var employee = GetEmployeeWithTrainingProgramsById(id);
            var viewModel = new TrainingProgramEmployeeEditViewModel()
            {
                Employee = employee,
                AllTrainingPrograms = GetAllTrainingPrograms(id),
                SelectedTrainingProgramIds = employee.TrainingPrograms.Select(t => t.Id).ToList()
            };

            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTraining(int id, TrainingProgramEmployeeEditViewModel viewModel)
        {
            var updatedEmployee = viewModel.Employee;
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM EmployeeTraining WHERE EmployeeId = @id;";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = @"
                            INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId)
                                VALUES (@employeeId, @trainingProgramId)";
                        foreach (var trainingProgramId in viewModel.SelectedTrainingProgramIds)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add(new SqlParameter("@employeeId", id));
                            cmd.Parameters.Add(new SqlParameter("@trainingProgramId", trainingProgramId));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction(nameof(Details));


                }
            }
            catch
            {
                viewModel = new TrainingProgramEmployeeEditViewModel()
                {
                    Employee = viewModel.Employee,
                    AllTrainingPrograms = GetAllTrainingPrograms(id)
                };

                return View(viewModel);
            }
        }




        //Helper Methods
        private Employee GetEmployeeById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT  
                        e.Id,
                        e.FirstName,
                        e.LastName,
                        e.DepartmentId,
                        e.IsSupervisor,
                        d.Name AS DepartmentName
                        FROM Employee e LEFT JOIN Department d ON d.Id = e.DepartmentId
                        WHERE e.Id = @employeeId";
                    cmd.Parameters.Add(new SqlParameter("@employeeId", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = new Employee();
                    
                    while (reader.Read())
                    {
                        employee = new Employee()
                        {

                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Department = new Department()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName"))
                            },
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };

                    }

                    reader.Close();

                    return employee;
                }
            }
        }
        private Employee GetEmployeeWithTrainingProgramsById(int id)

        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
               SELECT e.Id,
                e.FirstName,
                e.LastName,
                e.DepartmentId,
                e.IsSupervisor,
                et.TrainingProgramId,
                t.Name AS TrainingProgramName      			
            FROM Employee e LEFT JOIN EmployeeTraining et ON e.Id = et.EmployeeId
            LEFT JOIN TrainingProgram t ON t.Id = et.TrainingProgramId
            WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;
                    while (reader.Read())
                    {
                        if (employee == null)
                        {
                            Employee newEmployee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),

                            };
                            employee = newEmployee;
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TrainingProgramName")))
                        {
                            TrainingProgram newTrainingProgram = new TrainingProgram
                            {
                                Name = reader.GetString(reader.GetOrdinal("TrainingProgramName")),
                                Id = reader.GetInt32(reader.GetOrdinal("TrainingProgramId"))
                            };
                            employee.TrainingPrograms.Add(newTrainingProgram);
                        }
                    }

                    reader.Close();

                    return employee;
                }
            }
        }
private List<TrainingProgram> GetTrainingProgramsByEmployeeId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                     SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate
                                    FROM TrainingProgram tp LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                                    GROUP BY tp.Id, tp.Name, tp.StartDate, tp.EndDate, et.EmployeeId
                                    HAVING et.EmployeeId = @id
                                    ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    var trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                            trainingPrograms.Add(
                                new TrainingProgram()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                }
                            );
                    }

                    reader.Close();

                    return trainingPrograms;
                }
            }
        }

        private Computer GetAssignedComputerByEmployeeId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    
                    cmd.CommandText = @"SELECT ce.Id, ce.ComputerId, ce.EmployeeId, ce.AssignDate,
                                        c.Make, c.Manufacturer 
                                        FROM ComputerEmployee ce LEFT JOIN Computer c ON c.Id = ce.ComputerId
                                        WHERE ce.EmployeeId = @id AND ce.UnassignDate IS NULL
                                        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    Computer computer = new Computer();
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            computer = new Computer()
                            {

                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                                Make = reader.GetString(reader.GetOrdinal("Make"))

                            };

                        }
                    }
                    reader.Close();

                    return computer;
                }
            }
        }


        private List<Department> GetAllDepartments()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Budget FROM Department";
                    var reader = cmd.ExecuteReader();

                    var departments = new List<Department>();
                    while (reader.Read())
                    {
                        departments.Add(
                                new Department()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                }
                            );
                    }

                    reader.Close();

                    return departments;
                }
            }
        }
        private List<TrainingProgram> GetAllTrainingPrograms(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT t.Id, t.Name, t.StartDate, t.EndDate, t.MaxAttendees
                                             , et.EmployeeId
                                          FROM TrainingProgram t
                                     LEFT JOIN EmployeeTraining et ON t.Id = et.TrainingProgramId
                                         WHERE et.EmployeeId = @id OR t.StartDate > GETDATE()";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    var trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {
                        int currentId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!trainingPrograms.Exists(t => t.Id == currentId))
                        {
                            trainingPrograms.Add(
                                new TrainingProgram()
                                {
                                    Id = currentId,
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                                }
                            );
                        }
                    }

                    reader.Close();

                    return trainingPrograms;
                }
            }
        }
        private List<Computer> GetAvailableComputersByEmployeeId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //write SQL query to get only computers where count of employeeId in ComputerEmployee table is 0
                    cmd.CommandText = @"SELECT c.Id, c.Make, c.Manufacturer 
                                        FROM Computer c LEFT JOIN ComputerEmployee ce ON ce.ComputerId = c.Id
                                        GROUP BY c.Id, c.Make, c.Manufacturer, ce.EmployeeId
                                        HAVING COUNT(ce.EmployeeId) = 0 OR ce.EmployeeId = @id;
                                        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    var reader = cmd.ExecuteReader();

                    var computers = new List<Computer>();
                    while (reader.Read())
                    {
                        computers.Add(
                                new Computer()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Make = reader.GetString(reader.GetOrdinal("Make")),
                                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                }
                            );
                    }

                    reader.Close();

                    return computers;
                }
            }
        }

    }
}