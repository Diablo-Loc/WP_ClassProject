using System.Data;
using System.Threading.Tasks;

namespace ClassProject.DataAccess.Repositories.Interfaces
{
    public interface IContactRepository
    {
        // --- CÁC HÀM HỆ THỐNG / QUẢN TRỊ (Gộp cả Giảng viên & Tất cả Contact) ---
        Task<DataTable> GetAllContactsAsync();
        Task<DataTable> SearchContactsAsync(string keyword);

        // --- CÁC HÀM THEO TÀI KHOẢN NGƯỜI DÙNG ĐANG ĐĂNG NHẬP (Bảo mật) ---
        Task<DataTable> GetAllContactsByUserAsync(int userId);
        Task<DataTable> GetContactsByGroupAsync(int groupId, int userId);
        Task<DataTable> SearchContactsByUserAsync(string keyword, int userId);

        // --- NGHIỆP VỤ CRUD CONTACT CÓ GROUP & USERID ---
        Task<bool> InsertContactAsync(string fname, string lname, string phone, string email, int? groupId, int userId);
        Task<bool> UpdateContactAsync(int contactId, string fname, string lname, string phone, string email, int? groupId, int userId);
        Task<bool> DeleteContactAsync(int contactId, int userId);
        Task<bool> IsPhoneOrEmailExistsAsync(string phone, string email, int? excludeId = null, int? userId = null);

        // --- QUẢN LÝ DANH MỤC NHÓM (GROUPS) ---
        Task<DataTable> GetGroupsByUserAsync(int userId);
        Task<bool> InsertGroupAsync(string groupName, int userId);
        Task<bool> UpdateGroupAsync(int groupId, string groupName, int userId);
        Task<bool> DeleteGroupAsync(int groupId, int userId);
    }
}