using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MOM_Project.Models;
using System.Data;

namespace MOM_Project.Controllers
{
    public class MeetingsController : Controller
    {
        #region Configuration
        private readonly IConfiguration _configuration;

        public MeetingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Meetings List + Search
        public IActionResult MeetingsList(string searchText)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";

            cmd.Parameters.AddWithValue("@SearchText", searchText ?? (object)DBNull.Value);

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            connection.Close();

            List<MeetingsModel> list = new List<MeetingsModel>();

            foreach (DataRow row in table.Rows)
            {
                MeetingsModel model = new MeetingsModel
                {
                    MeetingID = Convert.ToInt32(row["MeetingID"]),
                    MeetingDate = Convert.ToDateTime(row["MeetingDate"]),
                    MeetingTypeName = row["MeetingTypeName"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    MeetingVenueName = row["MeetingVenueName"].ToString(),
                    IsCancelled = row["IsCancelled"] == DBNull.Value ? null : Convert.ToBoolean(row["IsCancelled"])
                };

                list.Add(model);
            }

            return View(list);
        }
        #endregion

        #region Load Dropdowns
        void LoadDropdowns()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            List<MeetingsModel> typeList = new List<MeetingsModel>();
            List<MeetingsModel> departmentList = new List<MeetingsModel>();
            List<MeetingsModel> venueList = new List<MeetingsModel>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("PR_MOM_MeetingType_Dropdown", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    typeList.Add(new MeetingsModel
                    {
                        MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]),
                        MeetingTypeName = reader["MeetingTypeName"].ToString()
                    });
                }

                reader.Close();

                SqlCommand cmd2 = new SqlCommand("PR_MOM_Department_Dropdown", conn);
                cmd2.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader2 = cmd2.ExecuteReader();

                while (reader2.Read())
                {
                    departmentList.Add(new MeetingsModel
                    {
                        DepartmentID = Convert.ToInt32(reader2["DepartmentID"]),
                        DepartmentName = reader2["DepartmentName"].ToString()
                    });
                }

                reader2.Close();

                SqlCommand cmd3 = new SqlCommand("PR_MOM_MeetingVenue_Dropdown", conn);
                cmd3.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader3 = cmd3.ExecuteReader();

                while (reader3.Read())
                {
                    venueList.Add(new MeetingsModel
                    {
                        MeetingVenueID = Convert.ToInt32(reader3["MeetingVenueID"]),
                        MeetingVenueName = reader3["MeetingVenueName"].ToString()
                    });
                }
            }

            ViewBag.MeetingTypeList = typeList;
            ViewBag.DepartmentList = departmentList;
            ViewBag.VenueList = venueList;
        }
        #endregion

        #region Add Meetings
        public IActionResult MeetingsAdd()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult MeetingsAdd(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_Insert";

            cmd.Parameters.AddWithValue("@MeetingDate", model.MeetingDate);
            cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeID);
            cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
            cmd.Parameters.AddWithValue("@MeetingVenueID", model.MeetingVenueID);
            cmd.Parameters.AddWithValue("@MeetingDescription", model.MeetingDescription ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            connection.Close();

            TempData["SuccessMessage"] = "Meeting Added Successfully";

            return RedirectToAction("MeetingsList");
        }
        #endregion

        #region Edit Meeting
        public IActionResult MeetingsEditForm(int MeetingID)
        {
            LoadDropdowns();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_SelectByPK";

            cmd.Parameters.AddWithValue("@MeetingID", MeetingID);

            SqlDataReader reader = cmd.ExecuteReader();

            MeetingsModel model = new MeetingsModel();

            if (reader.Read())
            {
                model.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                model.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                model.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                model.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                model.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                model.MeetingDescription = reader["MeetingDescription"].ToString();
            }

            connection.Close();

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingsEditForm(MeetingsModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_UpdateByPK";

            cmd.Parameters.AddWithValue("@MeetingID", model.MeetingID);
            cmd.Parameters.AddWithValue("@MeetingDate", model.MeetingDate);
            cmd.Parameters.AddWithValue("@MeetingTypeID", model.MeetingTypeID);
            cmd.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
            cmd.Parameters.AddWithValue("@MeetingVenueID", model.MeetingVenueID);
            cmd.Parameters.AddWithValue("@MeetingDescription", model.MeetingDescription ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            connection.Close();

            TempData["SuccessMessage"] = "Meeting Updated Successfully";

            return RedirectToAction("MeetingList");
        }
        #endregion

        #region Delete Meeting
        public IActionResult MeetingsDeleteForm(int MeetingID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_SelectByPK";

            cmd.Parameters.AddWithValue("@MeetingID", MeetingID);

            SqlDataReader reader = cmd.ExecuteReader();

            MeetingsModel model = new MeetingsModel();

            if (reader.Read())
            {
                model.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                model.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
            }

            connection.Close();

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingsDelete(int MeetingID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_DeleteByPK";

            cmd.Parameters.AddWithValue("@MeetingID", MeetingID);

            cmd.ExecuteNonQuery();

            connection.Close();

            TempData["SuccessMessage"] = "Meeting Deleted Successfully";

            return RedirectToAction("MeetingsList");
        }
        #endregion

        #region View Meeting
        public IActionResult MeetingsView(int MeetingID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_Meetings_SelectByPK";

            cmd.Parameters.AddWithValue("@MeetingID", MeetingID);

            SqlDataReader reader = cmd.ExecuteReader();

            MeetingsModel model = new MeetingsModel();

            if (reader.Read())
            {
                model.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                model.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                model.MeetingTypeName = reader["MeetingTypeName"].ToString();
                model.DepartmentName = reader["DepartmentName"].ToString();
                model.MeetingVenueName = reader["MeetingVenueName"].ToString();
                model.MeetingDescription = reader["MeetingDescription"].ToString();
                model.Created = Convert.ToDateTime(reader["Created"]);
                model.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            connection.Close();

            return View(model);
        }
        #endregion
    }
}