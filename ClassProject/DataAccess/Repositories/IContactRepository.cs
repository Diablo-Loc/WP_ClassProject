using System.Data;
using System.Threading.Tasks;

namespace ClassProject.DataAccess.Repositories
{
    public interface IContactRepository
    {
        Task<DataTable> GetAllContactsAsync();

        Task<DataTable> SearchContactsAsync(string keyword);

        Task<bool> InsertContactAsync(string name, string phone, string email);

        Task<bool> UpdateContactAsync(int contactId, string name, string phone, string email);

        Task<bool> DeleteContactAsync(int contactId);

        Task<bool> IsPhoneOrEmailExistsAsync(string phone, string email, int? excludeId = null);
    }
}