using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace ClassProject.Models
{
    public class Course
    {
        private My_DB db = new My_DB();

        // =========================
        // Properties
        // =========================
        public string Mamh { get; set; }
        public string Tenmh { get; set; }
        public int Sotc { get; set; }
        public int Tuan { get; set; }
        public int Hocky { get; set; }
        public string Description { get; set; }

        // =========================
        // Constructor
        // =========================
        public Course() { }

        public Course(string mamh, string tenmh,
                      int sotc, int tuan,
                      int hocky, string description)
        {
            Mamh = mamh;
            Tenmh = tenmh;
            Sotc = sotc;
            Tuan = tuan;
            Hocky = hocky;
            Description = description;
        }

        // =========================
        // 1. Add Course
        // =========================
        public bool AddCourse()
        {
            string query = @"INSERT INTO Courses
                            (MaMH, TenMH, SoTC, Tuan, Hky, Mota)
                            VALUES
                            (@ma, @ten, @tc, @tuan, @hk, @mota)";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", Mamh);
                        cmd.Parameters.AddWithValue("@ten", Tenmh);
                        cmd.Parameters.AddWithValue("@tc", Sotc);
                        cmd.Parameters.AddWithValue("@tuan", Tuan);
                        cmd.Parameters.AddWithValue("@hk", Hocky);
                        cmd.Parameters.AddWithValue("@mota", Description ?? "");

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // =========================
        // 2. Edit Course
        // =========================
        public bool EditCourse()
        {
            string query = @"UPDATE Courses
                             SET TenMH = @ten,
                                 SoTC = @tc,
                                 Tuan = @tuan,
                                 Hky = @hk,
                                 Mota = @mota
                             WHERE MaMH = @ma";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", Mamh);
                        cmd.Parameters.AddWithValue("@ten", Tenmh);
                        cmd.Parameters.AddWithValue("@tc", Sotc);
                        cmd.Parameters.AddWithValue("@tuan", Tuan);
                        cmd.Parameters.AddWithValue("@hk", Hocky);
                        cmd.Parameters.AddWithValue("@mota", Description ?? "");

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // =========================
        // 3. Delete Course
        // =========================
        public bool DeleteCourse(string mamh)
        {
            string query = "DELETE FROM Courses WHERE MaMH = @ma";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", mamh);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // =========================
        // 4. Get All Courses
        // =========================
        public DataTable GetCourses()
        {
            DataTable table = new DataTable();

            string query = "SELECT * FROM Courses";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return table;
        }

        // =========================
        // 5. Search Course
        // =========================
        public DataTable SearchCourse(string keyword)
        {
            DataTable table = new DataTable();

            string query = @"SELECT * FROM Courses
                             WHERE MaMH LIKE @kw
                             OR TenMH LIKE @kw";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return table;
        }

        // =========================
        // 6. Get Course By Semester
        // =========================
        public DataTable GetCoursesBySemester(int semester)
        {
            DataTable table = new DataTable();

            string query = "SELECT * FROM Courses WHERE Hky = @hk";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hk", semester);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return table;
        }

        // =========================
        // 7. Check Course Exists
        // =========================
        public bool CourseExists(string mamh)
        {
            string query = "SELECT COUNT(*) FROM Courses WHERE MaMH = @ma";

            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ma", mamh);

                        int count = (int)cmd.ExecuteScalar();

                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}