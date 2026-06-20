using ClassProject.DataAccess.Db;
using ClassProject.DataAccess.Repositories.Implementations;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ClassProject.Business.Services
{

    public class TeachingAssignmentService
    {
        private readonly My_DB db = new My_DB();
        private readonly TeachingAssignmentRepository _repo;
        public TeachingAssignmentService()
        {
            // Khởi tạo Repo tại tầng này thay vì trên Form
            _repo = new TeachingAssignmentRepository(db.GetConnection().ConnectionString);
        }

        public TeachingAssignmentService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Chuỗi kết nối không được để trống.");

            _repo = new TeachingAssignmentRepository(connectionString);
        }
        /// Lấy toàn bộ danh sách phân công (Bất đồng bộ)
        public async Task<DataTable> GetAllAssignmentsAsync()
        {
            return await _repo.GetAssignmentsAsync();
        }

        /// Lấy danh sách giảng viên phục vụ ComboBox (Bất đồng bộ)
        public async Task<DataTable> GetDropdownTeachersAsync()
        {
            return await _repo.GetTeacherListAsync();
        }

        /// Lấy danh sách môn học phục vụ ComboBox (Bất đồng bộ)
        public async Task<DataTable> GetDropdownCoursesAsync()
        {
            return await _repo.GetCourseListAsync();
        }

        /// Xử lý nghiệp vụ phân công giảng dạy bất đồng bộ (Áp dụng các luật kiểm tra)
        public async Task<(bool Success, string Message)> AssignTeacherToCourseAsync(int teacherId, string maMH)
        {
            if (string.IsNullOrWhiteSpace(maMH))
            {
                return (false, "Mã môn học không hợp lệ.");
            }

            try
            {
                // Luật 1: Kiểm tra trùng lặp gán lịch
                bool isDuplicate = await _repo.IsDuplicateAssignmentAsync(teacherId, maMH);
                if (isDuplicate)
                {
                    return (false, "Giảng viên này đã được phân công dạy môn học này rồi!");
                }

                // Luật 2: Kiểm tra giới hạn số môn (Tối đa 5 môn)
                int currentCourses = await _repo.GetCurrentCourseCountAsync(teacherId);
                if (currentCourses >= 5)
                {
                    return (false, $"Không thể phân công! Giảng viên đã đạt giới hạn dạy tối đa ({currentCourses}/5 môn).");
                }

                // Đạt hết điều kiện -> Tiến hành lưu xuống DB thông qua Repo
                bool result = await _repo.InsertAssignmentAsync(teacherId, maMH);
                if (result)
                {
                    return (true, "Phân công giảng dạy thành công!");
                }

                return (false, "Đã xảy ra lỗi hệ thống trong quá trình lưu dữ liệu.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                return (false, $"Lỗi xử lý nghiệp vụ: {ex.Message}");
            }
        }

        /// Xóa phân công giảng dạy (Bất đồng bộ)
        public async Task<(bool Success, string Message)> RemoveAssignmentAsync(int id)
        {
            try
            {
                bool result = await _repo.DeleteAssignmentAsync(id);
                if (result)
                {
                    return (true, "Xóa phân công thành công!");
                }

                return (false, "Không tìm thấy dữ liệu phân công để xóa hoặc lỗi kết nối database.");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi khi xóa phân công: {ex.Message}");
            }
        }
        /// Lấy danh sách phân công phục vụ báo cáo (Bất đồng bộ)
        public async Task<DataTable> GetReportDataAsync(int? teacherId, string maMH)
        {
            return await _repo.GetAssignmentsReportAsync(teacherId, maMH);
        }
    }
}