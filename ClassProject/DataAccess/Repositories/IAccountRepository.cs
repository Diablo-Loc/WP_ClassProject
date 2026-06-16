using System.Collections.Generic;
using System.Data;
using ClassProject.Commons.DTOs;

namespace ClassProject.Data.Repositories
{
    public interface IAccountRepository
    {
        List<UserDTO> GetAllAccounts();
        DataTable GetRoles();
        bool CreateAccount(string username, string email, string password, int roleId, int status);
        bool UpdateAccount(int id, string email, int roleId, int status, int valid);
        bool ChangePassword(int id, string newHashPassword);
        bool DeleteAccount(int id);
        bool IsUsernameOrEmailExists(string username, string email, int? excludeId = null);

        bool UpdateSingleStatus(string username, int targetStatus, int? targetValid = null);
        bool UpdateBulkStatus(List<string> usernames, int targetStatus, int? targetValid = null);
    }
}