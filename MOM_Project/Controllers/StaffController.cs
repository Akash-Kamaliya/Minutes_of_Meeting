using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_Project.Models;
using System.Data;

namespace MOM_Project.Controllers
{
    public class StaffController : Controller
    {
        #region Configuration Injection
        private readonly IConfiguration _configuration;
        public StaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion





        #region Get All Departments
        [HttpGet]
        public IActionResult StaffList()
        {
            List<StaffModel> staffList = new List<StaffModel>();
            string sqlConnString = _configuration.GetConnectionString("DefaultConnection");
            using (var sqlConnection = new SqlConnection(sqlConnString))
            using (var sqlCommand = sqlConnection.CreateCommand())
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_MOM_Staff_SelectAll";

                sqlConnection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var staff = new StaffModel
                        {
                            StaffID = Convert.ToInt32(reader["StaffID"]),
                            DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                            StaffName = reader["StaffName"].ToString(),
                            DepartmentName = reader["DepartmentName"].ToString(),
                            MobileNo = reader["MobileNo"].ToString(),
                            EmailAddress = reader["EmailAddress"].ToString(),
                            Remarks = reader["Remarks"].ToString(),
                            Created = Convert.ToDateTime(reader["Created"]),
                            Modified = Convert.ToDateTime(reader["Modified"])
                        };
                        staffList.Add(staff);
                    }
                }
            }
            return View("StaffList",staffList);
        }
        #endregion

        public IActionResult StaffAdd()
        {
            return View();
        }

        #region Add Staff Operation
        [HttpPost]
        public IActionResult StaffAdd(StaffModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                model.Created = DateTime.UtcNow;
                model.Modified = DateTime.UtcNow;

                string sqlConnString = _configuration.GetConnectionString("DefaultConnection");
                using (var sqlConnection = new SqlConnection(sqlConnString))
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "PR_MOM_Staff_Insert";
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    //sqlCommand.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                    //sqlCommand.Parameters.AddWithValue("@Modified", model.Modified);

                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();

                }

                TempData["SucessMessage"] = "Record Inserted Successfully";
                return RedirectToAction("StaffList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }
        #endregion

        public IActionResult StaffEditForm()
        {
            var model = new StaffModel
            {
                StaffName = "Raju Patel",
                DepartmentName = "CSE",
                MobileNo = "9876543210",
                EmailAddress = "raju@email.com"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult StaffEditForm(StaffModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return RedirectToAction("StaffList");
        }

        public IActionResult StaffView()
        {
            return View();
        }

        public IActionResult StaffDeleteForm()
        {
            return View();
        }
    }
}
