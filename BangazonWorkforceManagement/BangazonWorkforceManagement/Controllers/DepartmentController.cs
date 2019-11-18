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
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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


        // GET: Department
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
             		SELECT d.Name, d.Budget, d.Id, e.Id AS EmployeeId
                    FROM Department d
                    LEFT JOIN Employee e ON e.DepartmentId = d.Id
                                        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Department> departments = new List<Department>();
                    Department currentDepartment = null;
                    while (reader.Read())
                    {
                        int departmentId = reader.GetInt32(reader.GetOrdinal("Id"));
                        

                        if (!departments.Exists(d => d.Id == departmentId))
                            {


                            Department department = new Department
                            {
                                Id = departmentId,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            };

                            currentDepartment = department;
                            departments.Add(currentDepartment);

                        }
                        
                        if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId"))
                            };
                            currentDepartment.Employees.Add(employee);
                        }
                        
                    }

                    reader.Close();

                    return View(departments);
                }
            }
        }
        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            var department = GetDepartmentById(id);
            department.Employees = GetDepartmentEmployeesById(id);
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Department
                                        (Name, Budget)
                                        VALUES ( @name, @budget)";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));


                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

            // GET: Department/Edit/5
            public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Department/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private Department GetDepartmentById(int id)

        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT  
                                d.Id,
                                d.Name,
                                d.Budget
                                FROM department d
                                WHERE d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    Department department = new Department();
                    while (reader.Read())
                    {
                        department = new Department()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                        };
                    }
                    reader.Close();
                    return department;
                }
            }
        }
        private List<Employee> GetDepartmentEmployeesById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"   
                    SELECT  
                    d.Id,
                    d.Name,
                    e.FirstName, e.LastName,
                    e.Id AS EmployeeId
                    FROM Department d 
                    LEFT JOIN Employee e ON d.Id = e.departmentId
                    WHERE d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Employee employee = null;
                    //Employee employee = new Employee();
                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                            if (!reader.IsDBNull(reader.GetOrdinal("FirstName")))
                                employee = new Employee()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };
                        employees.Add(employee);

                    }

                    reader.Close();

                    return employees;
                }
            }
        }
    }
}