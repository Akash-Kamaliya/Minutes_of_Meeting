using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM_Project.Models;
using System.Data;

namespace MOM_Project.Controllers
{
    public class StaffController : Controller
    {
        #region Configuration
        private readonly IConfiguration _configuration;

        public StaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Staff List + Search
        public IActionResult StaffList(string searchText)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_SelectAll", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@SearchText", searchText ?? (object)DBNull.Value);

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            connection.Close();

            List<StaffModel> list = new List<StaffModel>();

            foreach (DataRow row in table.Rows)
            {
                StaffModel model = new StaffModel
                {
                    StaffID = Convert.ToInt32(row["StaffID"]),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StaffName = row["StaffName"].ToString(),
                    MobileNo = row["MobileNo"].ToString(),
                    EmailAddress = row["EmailAddress"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                    Created = Convert.ToDateTime(row["Created"]),
                    Modified = Convert.ToDateTime(row["Modified"])
                };

                list.Add(model);
            }

            return View(list);
        }
        #endregion

        void LoadDepartmentDropdown()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            List<StaffModel> departmentList = new List<StaffModel>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("PR_MOM_Department_Dropdown", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    departmentList.Add(new StaffModel
                    {
                        DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                        DepartmentName = reader["DepartmentName"].ToString()
                    });
                }
            }

            ViewBag.DepartmentList = departmentList;
        }

        #region Add Staff
        public IActionResult StaffAdd()
        {
            LoadDepartmentDropdown();
            return View();
        }

        [HttpPost]
        public IActionResult StaffAdd(StaffModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDepartmentDropdown();
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_Insert", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
            cmd.Parameters.AddWithValue("@StaffName", model.StaffName);
            cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
            cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
            cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
            connection.Close();

            TempData["SuccessMessage"] = "Staff Added Successfully";

            return RedirectToAction("StaffList");
        }
        #endregion

        #region Edit Staff

        public IActionResult StaffEditForm(int StaffID)
        {
            LoadDepartmentDropdown();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_SelectByPK", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StaffID", StaffID);

            SqlDataReader reader = cmd.ExecuteReader();

            StaffModel model = new StaffModel();

            if (reader.Read())
            {
                model.StaffID = Convert.ToInt32(reader["StaffID"]);
                model.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                model.StaffName = reader["StaffName"].ToString();
                model.MobileNo = reader["MobileNo"].ToString();
                model.EmailAddress = reader["EmailAddress"].ToString();
                model.Remarks = reader["Remarks"].ToString();
            }

            connection.Close();

            return View(model);
        }

        [HttpPost]
        public IActionResult StaffEditForm(StaffModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDepartmentDropdown();
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_UpdateByPK", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StaffID", model.StaffID);
            cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
            cmd.Parameters.AddWithValue("@StaffName", model.StaffName);
            cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
            cmd.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
            cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
            connection.Close();

            TempData["SuccessMessage"] = "Staff Updated Successfully";

            return RedirectToAction("StaffList");
        }

        #endregion

        #region Delete Staff Member
        public IActionResult StaffDeleteForm(int StaffID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Staff_SelectByPK";

            cmd.Parameters.AddWithValue("@StaffID", StaffID);

            SqlDataReader reader = cmd.ExecuteReader();

            StaffModel model = new StaffModel();

            if (reader.Read())
            {
                model.StaffID = Convert.ToInt32(reader["StaffID"]);
                model.StaffName = reader["StaffName"].ToString();
                model.DepartmentName = reader["DepartmentName"].ToString();
                model.EmailAddress = reader["EmailAddress"].ToString();
            }

            connection.Close();

            return View(model);
        }

        [HttpPost]
        public IActionResult StaffDelete(int StaffID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Staff_DeleteByPK";

            cmd.Parameters.AddWithValue("@StaffID", StaffID);

            cmd.ExecuteNonQuery();

            connection.Close();

            TempData["SuccessMessage"] = "Staff Deleted Successfully";

            return RedirectToAction("StaffList");
        }
        #endregion  
        public IActionResult StaffView(int StaffID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("PR_MOM_Staff_SelectByPK", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StaffID", StaffID);

            SqlDataReader reader = cmd.ExecuteReader();

            StaffModel model = new StaffModel();

            if (reader.Read())
            {
                model.StaffID = Convert.ToInt32(reader["StaffID"]);
                model.StaffName = reader["StaffName"].ToString();
                model.DepartmentName = reader["DepartmentName"].ToString();
                model.MobileNo = reader["MobileNo"].ToString();
                model.EmailAddress = reader["EmailAddress"].ToString();
                model.Remarks = reader["Remarks"].ToString();
            }

            connection.Close();

            return View(model);
        }
    }
}