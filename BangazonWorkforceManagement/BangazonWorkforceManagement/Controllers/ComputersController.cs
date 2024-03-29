﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BangazonWorkforceManagement.Models;
using BangazonWorkforceManagement.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BangazonWorkforceManagement.Controllers
{
    public class ComputersController : Controller
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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

        // GET: Computers
        public ActionResult Index(string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (q == null)
                    {
                        cmd.CommandText = @"
                     SELECT c.Id,
                        c.PurchaseDate,
                        c.DecomissionDate,
                        c.Make,
                        c.Manufacturer
                    FROM Computer c

                    ORDER BY c.Make, c.Manufacturer;
                    ";
                    }
                    else
                    {
                        cmd.CommandText = @"
                            SELECT Id, Make, Manufacturer, PurchaseDate, DecomissionDate FROM Computer
                            WHERE Make LIKE '%' + @q + '%' OR Manufacturer LIKE '%' + @q + '%'
                            ORDER BY Make, Manufacturer
                    ";
                        cmd.Parameters.Add(new SqlParameter("@q", q));

                    }

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        int computerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            Computer Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Purchasedate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                            computers.Add(Computer);
                        }
                        else
                        {
                            Computer Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Purchasedate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                            computers.Add(Computer);
                        }
                    }

                    reader.Close();
                    return View(computers);


                }
            }
        }

        // GET: Computers/Details/5
        public ActionResult Details(int id)
        {
            Computer computer = GetComputerById(id);
            return View(computer);
        }

        // GET: Computers/Create
        public ActionResult Create()    
        {
            var viewModel = new ComputerEmployeeViewModel();

            var employees = GetAllEmployees();
            var selectedEmployees = employees
                .Select(e => new SelectListItem
                {
                    Text = $"{e.FirstName} {e.LastName}",
                    Value = e.Id.ToString()
                }).ToList();

            selectedEmployees.Insert(0, new SelectListItem
            {
                Text = "Choose Employee....",
                Value = "0"
            });
            viewModel.Employees = selectedEmployees;
            return View(viewModel);
        }

        // POST: Computers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ComputerEmployeeViewModel viewModel)
        {
            try
            {
                // TODO: Add insert logic here
                var computer = viewModel.Computer;
                var selectedEmployeeId = viewModel.SelectedEmployeeId;
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Computer
                                            ( PurchaseDate, Make, Manufacturer )
                                            OUTPUT Inserted.ID
                                            VALUES
                                            ( @PurchaseDate, @Make, @Manufacturer );
                                            ";
                        cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));

                        cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));
                        int computerId = (Int32)cmd.ExecuteScalar();
                        if (selectedEmployeeId != 0)
                        {
                            cmd.CommandText = @"INSERT INTO ComputerEmployee
                                            ( ComputerId, EmployeeId, AssignDate )
                                            VALUES
                                            ( @ComputerId, @EmployeeId, GetDate() );
                                            ";
                            cmd.Parameters.Add(new SqlParameter("@ComputerId", computerId));
                            cmd.Parameters.Add(new SqlParameter("@EmployeeId", selectedEmployeeId));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                    return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                var exception = ex;
                return View();
            }
        }

        // GET: Computers/Edit/5
        public ActionResult Edit(int id)
        {
            Computer computer = GetComputerById(id);
            return View(computer);
        }

        // POST: Computers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Computer computer)
        {
            try
            {
                // TODO: Add update logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        if (computer.DecomissionDate != null)
                        {
                            cmd.CommandText = @"UPDATE Computer
                                                SET PurchaseDate = @PurchaseDate,
                                                    DecomissionDate = @DecomissionDate,
                                                    Make = @Make,
                                                    Manufacturer = @Manufacturer
                                                WHERE Id = @id";

                            cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                            cmd.Parameters.Add(new SqlParameter("@DecomissionDate", computer.DecomissionDate));
                            cmd.Parameters.Add(new SqlParameter("@id", computer.Id));
                            cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));

                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = @"UPDATE Computer
                                                SET PurchaseDate = @PurchaseDate,
                                                    
                                                    Make = @Make,
                                                    Manufacturer = @Manufacturer
                                                WHERE Id = @id";

                            cmd.Parameters.Add(new SqlParameter("@PurchaseDate", computer.PurchaseDate));
                            cmd.Parameters.Add(new SqlParameter("@id", computer.Id));
                            cmd.Parameters.Add(new SqlParameter("@Make", computer.Make));
                            cmd.Parameters.Add(new SqlParameter("@Manufacturer", computer.Manufacturer));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var exception = ex;
                return View();
            }
        }

        // GET: Computers/Delete/5
        public ActionResult Delete(int id)
        {
            Computer computer = GetComputerByIdForDelete(id);
            return View(computer);
        }

        // POST: Computers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Computer computer)
        {
            try
            {
                // TODO: Add delete logic here
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT ComputerId 
                                            FROM ComputerEmployee
                                            WHERE ComputerId = @Id;";
                        cmd.Parameters.Add(new SqlParameter("@Id", id));
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            //in this condition the ComputerId was found in the table 
                            //which means there has been a relationship
                            reader.Close();
                           
                            return Ok("This computer has had a previous relationship with an employee and cannot be deleted. " +
                                "This needs a different view");

                        }
                        else
                        {
                            //there has not been a relationship and delete can proceed
                            reader.Close();
                            cmd.CommandText = @"DELETE FROM Computer WHERE Id = @Id";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        private Computer GetComputerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id,
                            c.PurchaseDate,
                            c.DecomissionDate,
                            c.Make,
                            c.Manufacturer
                        FROM Computer c
                        WHERE Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;
                    while (reader.Read())
                    {
                        int computerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            Computer Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Purchasedate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                            computer = Computer;
                        }
                        else
                        {
                            Computer Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Purchasedate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                            computer = Computer;
                        }
                    }
                    reader.Close();

                    return computer;
                }
            }
        }

        private Computer GetComputerByIdForDelete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id,
                            c.PurchaseDate,
                            c.DecomissionDate,
                            c.Make,
                            c.Manufacturer
                        FROM Computer c
                        WHERE Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Computer computer = null;
                    while (reader.Read())
                    {
                        int computerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            Computer Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Purchasedate")),
                                DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                            computer = Computer;
                        }
                        else
                        {
                            Computer Computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("Purchasedate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            };
                            computer = Computer;
                        }
                    }
                    reader.Close();

                    return computer;
                }
            }
        }

        private List<Employee> GetAllEmployees()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
             	SELECT DISTINCT e.Id, e.FirstName, e.LastName
                    FROM Employee e LEFT JOIN ComputerEmployee ce ON ce.EmployeeId = e.Id
                    GROUP BY e.Id, e.FirstName, e.LastName, ce.UnassignDate, ce.EmployeeId
                    HAVING Count(ce.EmployeeId) = 0
                    OR ce.UnassignDate IS NOT NULL AND ce.EmployeeId NOT IN
                    ( SELECT ce.EmployeeId
                    FROM ComputerEmployee ce
                    WHERE ce.UnassignDate IS NULL);
                    ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
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