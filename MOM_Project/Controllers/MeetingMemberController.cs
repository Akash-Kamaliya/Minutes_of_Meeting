using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MOM_Project.Models;
using System.Data;

namespace MOM_Project.Controllers
{
    public class MeetingMemberController : Controller
    {
        #region Configuration
        private readonly IConfiguration _configuration;

        public MeetingMemberController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region MeetingMember List + Search
        public IActionResult MeetingMemberList(string searchText)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_MeetingMember_SelectAll";

            cmd.Parameters.AddWithValue("@SearchText", searchText ?? (object)DBNull.Value);

            SqlDataReader reader = cmd.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            connection.Close();

            List<MeetingMemberModel> list = new List<MeetingMemberModel>();

            foreach (DataRow row in table.Rows)
            {
                MeetingMemberModel model = new MeetingMemberModel
                {
                    MeetingMemberID = Convert.ToInt32(row["MeetingMemberID"]),
                    MeetingName = row["MeetingName"].ToString(),
                    StaffName = row["StaffName"].ToString(),
                    IsPresent = Convert.ToBoolean(row["IsPresent"]),
                    Remarks = row["Remarks"].ToString(),
                    Created = Convert.ToDateTime(row["Created"]),
                    Modified = Convert.ToDateTime(row["Modified"])
                };

                list.Add(model);
            }

            return View(list);
        }
        #endregion

        void LoadDropdowns()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            List<MeetingMemberModel> meetingList = new List<MeetingMemberModel>();
            List<MeetingMemberModel> staffList = new List<MeetingMemberModel>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                
                SqlCommand cmd = new SqlCommand("PR_MOM_Meeting_Dropdown", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    meetingList.Add(new MeetingMemberModel
                    {
                        MeetingID = Convert.ToInt32(reader["MeetingID"]),
                        MeetingTypeName = reader["MeetingTypeName"].ToString()
                    });
                }

                reader.Close();

                
                SqlCommand cmd2 = new SqlCommand("PR_MOM_Staff_Dropdown", conn);
                cmd2.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader2 = cmd2.ExecuteReader();

                while (reader2.Read())
                {
                    staffList.Add(new MeetingMemberModel
                    {
                        StaffID = Convert.ToInt32(reader2["StaffID"]),
                        StaffName = reader2["StaffName"].ToString()
                    });
                }
            }

            ViewBag.MeetingList = meetingList;
            ViewBag.StaffList = staffList;
        }

        #region Add Meeting Member
        public IActionResult MeetingMemberAdd()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult MeetingMemberAdd(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(model);
            }

            return RedirectToAction("MeetingMemberList");
        }
        #endregion


        #region Edit Meeting Member
        public IActionResult MeetingMemberEditForm(int MeetingMemberID)
        {
            LoadDropdowns();   // important for dropdown

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_MeetingMember_SelectByPK";

            cmd.Parameters.AddWithValue("@MeetingMemberID", MeetingMemberID);

            SqlDataReader reader = cmd.ExecuteReader();

            MeetingMemberModel model = new MeetingMemberModel();

            if (reader.Read())
            {
                model.MeetingMemberID = Convert.ToInt32(reader["MeetingMemberID"]);
                model.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                model.StaffID = Convert.ToInt32(reader["StaffID"]);
                model.IsPresent = Convert.ToBoolean(reader["IsPresent"]);
                model.Remarks = reader["Remarks"].ToString();
            }

            connection.Close();

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingMemberEditForm(MeetingMemberModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(); // reload dropdown
                return View(model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_MeetingMember_UpdateByPK";

            cmd.Parameters.AddWithValue("@MeetingMemberID", model.MeetingMemberID);
            cmd.Parameters.AddWithValue("@MeetingID", model.MeetingID);
            cmd.Parameters.AddWithValue("@StaffID", model.StaffID);
            cmd.Parameters.AddWithValue("@IsPresent", model.IsPresent);
            cmd.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();

            connection.Close();

            TempData["SuccessMessage"] = "Meeting Member Updated Successfully";

            return RedirectToAction("MeetingMemberList");
        }
        #endregion

        #region Delete Meeting Member
        public IActionResult MeetingMemberDelete(int MeetingMemberID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MOM_MeetingMember_DeleteByPK";

            cmd.Parameters.AddWithValue("@MeetingMemberID", MeetingMemberID);

            cmd.ExecuteNonQuery();

            connection.Close();

            TempData["SuccessMessage"] = "Meeting Member Deleted Successfully";

            return RedirectToAction("MeetingMemberList");
        }
        #endregion


        #region View Meeting Member
        public IActionResult MeetingMemberView(int MeetingMemberID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MeetingMember_SelectByID";

            cmd.Parameters.AddWithValue("@MeetingMemberID", MeetingMemberID);

            SqlDataReader reader = cmd.ExecuteReader();

            MeetingMemberModel model = new MeetingMemberModel();

            if (reader.Read())
            {
                model.MeetingMemberID = Convert.ToInt32(reader["MeetingMemberID"]);
                model.MeetingName = reader["MeetingName"].ToString();
                model.StaffName = reader["StaffName"].ToString();
                model.IsPresent = Convert.ToBoolean(reader["IsPresent"]);
                model.Remarks = reader["Remarks"].ToString();
                model.Created = Convert.ToDateTime(reader["Created"]);
                model.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            connection.Close();

            return View(model);
        }
        #endregion
    }
}