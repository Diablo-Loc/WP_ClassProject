using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ClassProject.DataAccess.Repositories.Implementations
{
    public class ClassRoomRepository
    {
        private readonly My_DB _db = new My_DB();

        public ClassRoomRepository()
        {
        }

        /// <summary>
        /// [STORED PROCEDURE - ASYNC] Lấy toàn bộ danh sách lớp học kèm tên chuyên ngành
        /// </summary>
        public async Task<DataTable> GetClassroomsAsync()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("dbo.proc_GetAllClassrooms", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        // Giữ kết nối mở bất đồng bộ và nạp dữ liệu
                        await Task.Run(() => adapter.Fill(table));
                    }
                }
            }
            return table;
        }

        /// <summary>
        /// [STORED PROCEDURE - ASYNC] Lấy toàn bộ danh mục Chuyên ngành để đẩy lên ComboBox
        /// </summary>
        public async Task<DataTable> GetAllMajorsAsync()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("dbo.proc_GetAllMajors", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => adapter.Fill(table));
                    }
                }
            }
            return table;
        }

        /// <summary>
        /// [STORED PROCEDURE - ASYNC] Lấy danh sách tài khoản giảng viên hoạt động làm GVCN
        /// </summary>
        public async Task<DataTable> GetActiveTeachersAsync()
        {
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("dbo.proc_GetActiveTeachers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => adapter.Fill(table));
                    }
                }
            }
            return table;
        }

        /// <summary>
        /// [ASYNC] Thêm mới lớp học - Kiểm soát ràng buộc sớm đầu vào dữ liệu
        /// </summary>
        public async Task<bool> AddClassroomAsync(Classroom classroom)
        {
            if (classroom == null) throw new ArgumentNullException(nameof(classroom));

            if (string.IsNullOrWhiteSpace(classroom.MaLop) || classroom.MaLop.Length > 20)
                throw new InvalidOperationException("Mã lớp không hợp lệ hoặc vượt quá 20 ký tự cho phép!");
            if (string.IsNullOrWhiteSpace(classroom.TenLop) || classroom.TenLop.Length > 100)
                throw new InvalidOperationException("Tên lớp không hợp lệ hoặc vượt quá 100 ký tự cho phép!");

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"INSERT INTO dbo.Classroom (MaLop, TenLop, GVCN, MaNganh, Created_At) 
                                 VALUES (@maLop, @tenLop, @gvcn, @maNganh, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20).Value = classroom.MaLop.Trim();
                    cmd.Parameters.Add("@tenLop", SqlDbType.NVarChar, 100).Value = classroom.TenLop.Trim();
                    cmd.Parameters.Add("@gvcn", SqlDbType.NVarChar, 100).Value = (object)classroom.GVCN ?? DBNull.Value;
                    cmd.Parameters.Add("@maNganh", SqlDbType.Char, 10).Value = (object)classroom.MaNganh ?? DBNull.Value;

                    try
                    {
                        await conn.OpenAsync();
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                        {
                            if (ex.Message.Contains("UQ_Classroom_TenLop") || ex.Message.Contains("TenLop"))
                                throw new InvalidOperationException($"[TC02] Tên lớp học '{classroom.TenLop}' đã bị một tài khoản khác đăng ký trước đó!");

                            throw new InvalidOperationException($"[TC01/TC06] Mã lớp học '{classroom.MaLop}' đã tồn tại trong cơ sở dữ liệu!");
                        }
                        if (ex.Number == 547)
                        {
                            throw new InvalidOperationException($"[TC07] Không thể lưu! Chuyên ngành được chọn không còn tồn tại trên hệ thống.");
                        }
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// [ASYNC] Cập nhật thông tin lớp học - Chống đổi ngành khi lớp đã có sinh viên phân bổ (TC10)
        /// </summary>
        public async Task<bool> UpdateClassroomAsync(Classroom classroom)
        {
            if (classroom == null) throw new ArgumentNullException(nameof(classroom));

            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();

                // TC10: Bảo vệ toàn vẹn dữ liệu khi đổi mã ngành
                Classroom oldData = await GetClassroomByIdAsync(classroom.MaLop);
                if (oldData != null && oldData.MaNganh != classroom.MaNganh)
                {
                    string checkStudentQuery = "SELECT COUNT(1) FROM dbo.Students WHERE MaLop = @maLop";
                    using (SqlCommand checkCmd = new SqlCommand(checkStudentQuery, conn))
                    {
                        checkCmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20).Value = classroom.MaLop.Trim();
                        int studentCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                        if (studentCount > 0)
                        {
                            throw new InvalidOperationException($"[TC10] Nghiêm cấm thay đổi Chuyên ngành của lớp học khi lớp đang có {studentCount} sinh viên đang sinh hoạt để chống lệch pha dữ liệu!");
                        }
                    }
                }

                string query = @"UPDATE dbo.Classroom 
                                 SET TenLop = @tenLop, GVCN = @gvcn, MaNganh = @maNganh, Updated_At = GETDATE() 
                                 WHERE MaLop = @maLop";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20).Value = classroom.MaLop.Trim();
                    cmd.Parameters.Add("@tenLop", SqlDbType.NVarChar, 100).Value = classroom.TenLop.Trim();
                    cmd.Parameters.Add("@gvcn", SqlDbType.NVarChar, 100).Value = (object)classroom.GVCN ?? DBNull.Value;
                    cmd.Parameters.Add("@maNganh", SqlDbType.Char, 10).Value = (object)classroom.MaNganh ?? DBNull.Value;

                    try
                    {
                        int rows = await cmd.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601)
                            throw new InvalidOperationException($"Tên lớp học '{classroom.TenLop}' trùng với một lớp học khác trên hệ thống!");
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// [ASYNC] Tìm kiếm lớp học linh hoạt theo từ khóa
        /// </summary>
        public async Task<DataTable> SearchClassroomsAsync(string keyword)
        {
            string cleanKeyword = keyword?.Trim() ?? "";
            DataTable table = new DataTable();
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT c.MaLop, c.TenLop, c.SiSo, c.GVCN, c.MaNganh, m.TenNganh, 'Active' AS Status
                                 FROM dbo.Classroom c 
                                 LEFT JOIN dbo.Major m ON c.MaNganh = m.MaNganh
                                 WHERE c.TenLop LIKE @key OR c.MaLop LIKE @key 
                                 ORDER BY c.MaLop ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@key", SqlDbType.NVarChar, 100).Value = "%" + cleanKeyword + "%";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        await Task.Run(() => adapter.Fill(table));
                    }
                }
            }
            return table;
        }

        /// <summary>
        /// [ASYNC] Xóa lớp học - Kiểm tra sinh viên trước khi thực thi (TC05)
        /// </summary>
        public async Task<bool> DeleteClassroomAsync(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop)) return false;
            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();

                // TC05: Ngăn chặn xóa lớp hành chính nếu đang chứa sinh viên
                string checkQuery = "SELECT COUNT(1) FROM dbo.Students WHERE MaLop = @maLop";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20).Value = maLop.Trim();
                    if (Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0)
                        throw new InvalidOperationException("[TC05] Không được phép xóa lớp học này vì dữ liệu lịch sử đang có sinh viên tham chiếu!");
                }

                string deleteQuery = "DELETE FROM dbo.Classroom WHERE MaLop = @maLop";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20).Value = maLop.Trim();
                    int rows = await deleteCmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        /// <summary>
        /// [ASYNC] Truy vấn thông tin chi tiết của một lớp theo Mã Lớp
        /// </summary>
        public async Task<Classroom> GetClassroomByIdAsync(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop)) return null;
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT c.MaLop, c.TenLop, c.GVCN, c.MaNganh, 'Active' AS Status,
                                        m.TenNganh, c.SiSo
                                 FROM dbo.Classroom c 
                                 LEFT JOIN dbo.Major m ON c.MaNganh = m.MaNganh
                                 WHERE c.MaLop = @maLop";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20).Value = maLop.Trim();
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Classroom
                            {
                                MaLop = reader["MaLop"].ToString(),
                                TenLop = reader["TenLop"].ToString(),
                                GVCN = reader["GVCN"] != DBNull.Value ? reader["GVCN"].ToString() : null,
                                MaNganh = reader["MaNganh"] != DBNull.Value ? reader["MaNganh"].ToString() : null,
                                Status = reader["Status"].ToString(),
                                SiSoThucTe = Convert.ToInt32(reader["SiSo"]),
                                TenNganh = reader["TenNganh"] != DBNull.Value ? reader["TenNganh"].ToString() : null
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// [TRANSACTION - ASYNC] Thực thi import danh sách lớp học đồng loạt bảo toàn dữ liệu lớn
        /// </summary>
        public async Task BulkImportClassroomsAsync(List<Classroom> list)
        {
            if (list == null || list.Count == 0) return;

            using (SqlConnection conn = _db.GetConnection())
            {
                await conn.OpenAsync();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    string query = @"INSERT INTO dbo.Classroom (MaLop, TenLop, GVCN, MaNganh, Created_At) 
                                     VALUES (@maLop, @tenLop, @gvcn, @maNganh, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.Add("@maLop", SqlDbType.VarChar, 20);
                        cmd.Parameters.Add("@tenLop", SqlDbType.NVarChar, 100);
                        cmd.Parameters.Add("@gvcn", SqlDbType.NVarChar, 100);
                        cmd.Parameters.Add("@maNganh", SqlDbType.Char, 10);

                        try
                        {
                            foreach (var cls in list)
                            {
                                cmd.Parameters["@maLop"].Value = cls.MaLop?.Trim();
                                cmd.Parameters["@tenLop"].Value = cls.TenLop?.Trim();
                                cmd.Parameters["@gvcn"].Value = (object)cls.GVCN ?? DBNull.Value;
                                cmd.Parameters["@maNganh"].Value = (object)cls.MaNganh ?? DBNull.Value;

                                await cmd.ExecuteNonQueryAsync();
                            }
                            await Task.Run(() => transaction.Commit());
                        }
                        catch (Exception)
                        {
                            await Task.Run(() => transaction.Rollback());
                            throw;
                        }
                    }
                }
            }
        }
        // cho form add student
        public DataTable GetClassroomsByMajor(string maNganh)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                string query = "SELECT MaLop, TenLop FROM dbo.Classroom WHERE MaNganh = @MaNganh ORDER BY TenLop ASC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@MaNganh", SqlDbType.Char, 10) { Value = maNganh });
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}