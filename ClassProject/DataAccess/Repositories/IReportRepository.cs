using System.Data;
using System.Threading.Tasks;

namespace ClassProject.DataAccess.Repositories
{
    public interface IReportRepository
    {
        // Sử dụng Task để chạy bất đồng bộ (Async), bảo vệ UI không bị đơ
        Task<DataTable> GetCoursesAsync();
        Task<DataTable> GetScoreReportDataAsync(string maMH);
    }
}