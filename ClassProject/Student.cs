using ClassProject.DataAccess.Db;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ClassProject
{
    public class Student
    {
        // Fields
        private int _userId;
        private int _mssv;
        private string _firstName;
        private string _lastName;
        private DateTime _dateOfBirth;
        private string _gender;
        private string _phone;
        private string _address;
        private string _hometown;
        private string _email;
        private byte[] _picture;

        // Properties
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        public int Mssv
        {
            get { return _mssv; }
            set
            {
                if (value <= 0)
                    throw new Exception("MSSV không hợp lệ!");
                _mssv = value;
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Tên không được để trống!");
                _firstName = value;
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new Exception("Họ không được để trống!");
                _lastName = value;
            }
        }

        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                if (value > DateTime.Now)
                    throw new Exception("Ngày sinh không được lớn hơn hiện tại!");
                _dateOfBirth = value;
            }
        }

        public string Gender
        {
            get { return _gender; }
            set
            {
                if (value != "Nam" && value != "Nữ")
                    throw new Exception("Giới tính chỉ được là 'Nam' hoặc 'Nữ'!");
                _gender = value;
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 10 || value.Length > 11)
                    throw new Exception("Số điện thoại không hợp lệ!");
                _phone = value;
            }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string Hometown
        {
            get { return _hometown; }
            set { _hometown = value; }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("@"))
                    throw new Exception("Email không hợp lệ!");
                _email = value;
            }
        }

        public byte[] Picture
        {
            get { return _picture; }
            set
            {
                if (value != null && value.Length > 5 * 1024 * 1024)
                    throw new Exception("Ảnh không được vượt quá 5MB!");
                _picture = value;
            }
        }

        // Constructor
        public Student(){ }
        public Student(int mssv, string firstName, string lastName, DateTime dateOfBirth,
                       string gender, string phone, string address, string hometown,
                       string email, byte[] picture = null, int userId = 0)
        {
            UserId = userId;
            Mssv = mssv;
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Phone = phone;
            Address = address;
            Hometown = hometown;
            Email = email;
            Picture = picture;
        }

        // Họ tên đầy đủ
        public string FullName => $"{LastName} {FirstName}";

        // Hiển thị thông tin
        public override string ToString()
        {
            return $"MSSV      : {Mssv}\n" +
                   $"Họ tên    : {FullName}\n" +
                   $"Ngày sinh : {DateOfBirth:dd/MM/yyyy}\n" +
                   $"Giới tính : {Gender}\n" +
                   $"SĐT       : {Phone}\n" +
                   $"Địa chỉ   : {Address}\n" +
                   $"Quê quán  : {Hometown}\n" +
                   $"Email     : {Email}\n" +
                   $"Ảnh       : {(Picture != null ? "Có" : "Chưa có")}";
        }

        public DataTable getStudents(SqlCommand command)
        {
            My_DB db = new My_DB();

            using (SqlConnection conn = db.GetConnection())
            {
                command.Connection = conn;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();

                adapter.Fill(table);
                return table;
            }
        }
        public bool AddStudent(string connectionString)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                string sql = @"INSERT INTO Students 
                               (Mssv, FirstName, LastName, DateOfBirth, Gender, Phone, Address, Hometown, Email, Picture)
                           VALUES 
                               (@Mssv, @FirstName, @LastName, @DateOfBirth, @Gender, @Phone, @Address, @Hometown, @Email, @Picture)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.Add(new SqlParameter("@Mssv", SqlDbType.Int) { Value = Mssv });
                cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar) { Value = FirstName });
                cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar) { Value = LastName });
                cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = DateOfBirth });
                cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar) { Value = Gender });
                cmd.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar) { Value = Phone });
                cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar) { Value = Address ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Hometown", SqlDbType.NVarChar) { Value = Hometown ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) { Value = Email });
                cmd.Parameters.Add(new SqlParameter("@Picture", SqlDbType.VarBinary) { Value = Picture ?? (object)DBNull.Value });

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        public bool IsMssvExist(int mssv, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Students WHERE MSSV = @mssv";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mssv", mssv);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public bool deleteStudent(int id)
        {
            My_DB db = new My_DB();
            using (SqlConnection conn = db.GetConnection())
            {
                SqlCommand command = new SqlCommand("DELETE FROM Students WHERE MSSV = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                conn.Open();
                int result = command.ExecuteNonQuery();
                return result == 1;
            }
        }
        public bool UpdateStudent()
        {
            My_DB db = new My_DB();
            string sql = @"UPDATE Students 
                   SET FirstName=@fn, LastName=@ln, DateOfBirth=@db, Gender=@gr, 
                       Phone=@ph, Address=@ad, Hometown=@ht, Email=@em, Picture=@pc 
                   WHERE MSSV=@mssv";

            // Thêm dấu () sau GetConnection
            using (SqlConnection conn = db.GetConnection())
            {
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@mssv", this.Mssv);
                command.Parameters.AddWithValue("@fn", this.FirstName);
                command.Parameters.AddWithValue("@ln", this.LastName);
                command.Parameters.AddWithValue("@db", this.DateOfBirth);
                command.Parameters.AddWithValue("@gr", this.Gender);
                command.Parameters.AddWithValue("@ph", this.Phone);
                command.Parameters.AddWithValue("@ad", this.Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ht", this.Hometown ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@em", this.Email);
                command.Parameters.AddWithValue("@pc", this.Picture ?? (object)DBNull.Value);

                conn.Open();
                bool result = (command.ExecuteNonQuery() == 1);
                return result;
            }
        }
    }
}
