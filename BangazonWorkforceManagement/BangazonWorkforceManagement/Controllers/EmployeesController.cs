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
            return View(employee);
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

        //// GET: Employee/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    var viewModel = new EmployeeEditViewModel()
        //    {
        //        Employee = GetById(id),
        //        Cohorts = GetAllCohorts()
        //    };

        //    return View(viewModel);
        //}

        // POST: Employee/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, EmployeeEditViewModel viewModel)
        //{
        //    Employee updatedEmployee = viewModel.Employee;
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"
        //        UPDATE Employee 
        //        SET
        //        FirstName = @firstName,
        //        LastName = @lastName,
        //        SlackHandle = @slackHandle,
        //        CohortId = @cohortId
        //        WHERE Id = @id;";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));
        //                cmd.Parameters.Add(new SqlParameter("@firstName", updatedEmployee.FirstName));
        //                cmd.Parameters.Add(new SqlParameter("@lastName", updatedEmployee.LastName));
        //                cmd.Parameters.Add(new SqlParameter("@slackHandle", updatedEmployee.SlackHandle));
        //                cmd.Parameters.Add(new SqlParameter("@cohortId", updatedEmployee.CohortId));

        //                cmd.ExecuteNonQuery();

        //            }
        //        }
        //        return RedirectToAction(nameof(Index));


        //    }
        //    catch
        //    {
        //        viewModel = new EmployeeEditViewModel()
        //        {
        //            Employee = viewModel.Employee,
        //            Cohorts = GetAllCohorts()
        //        };

        //        return View(viewModel);
        //    }
        //}

       
        //Helper Methods
        private Employee GetEmployeeById(int id)
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
               
                d.Name AS DepartmentName      			
            FROM Employee e LEFT JOIN Department d ON d.Id = e.DepartmentId           
            WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = new Employee();
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                            
                        };


                    }

                    reader.Close();

                    return employee;
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

    }
}