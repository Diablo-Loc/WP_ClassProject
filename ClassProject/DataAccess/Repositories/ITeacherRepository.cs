using System;
using System.Data;

namespace ClassProject.DataAccess.Repositories
{
    public interface ITeacherRepository
    {
        // Lấy toàn bộ danh sách giảng viên để đổ lên GridView
        DataTable GetAllTeachers();

        // Tìm kiếm nâng cao (Tìm theo Mã, Tên, SĐT, Email)
        DataTable SearchTeachers(string keyword);

        // Thêm mới giảng viên (Liên kết với một tài khoản User có sẵn)
        bool InsertTeacher(int? userId, string msgv, string firstName, string lastName, DateTime? dateOfBirth, string gender, string phone, string email, string academicRank);

        // Cập nhật thông tin và trạng thái công tác
        bool UpdateTeacher(int id, string firstName, string lastName, DateTime? dateOfBirth, string gender, string phone, string email, string academicRank, int status);

        // Xóa vật lý (Chỉ dùng khi nhập liệu sai, thực tế doanh nghiệp sẽ ưu tiên đổi Status = 0)
        bool DeleteTeacher(int id);

        // Hàm kiểm tra trùng lập Mã số, Số điện thoại hoặc Email trước khi lưu
        bool IsDuplicateCheck(string msgv, string phone, string email, int? excludeId = null);
    }
}