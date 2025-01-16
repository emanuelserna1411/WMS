using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Interfaces.Magister
{
    public interface ISuppliersRepository
    {
        Task<ActionResponse<Supplier>> GetAsync(long id);

        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync();

        Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Supplier>>> DownloadAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<IEnumerable<Supplier>>> GetDeleteAsync();

        Task<ActionResponse<IEnumerable<Supplier>>> GetDeleteAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<Supplier>> AddAsync(Supplier model, long Id_Local);
        Task<ActionResponse<List<Supplier>>> AddListAsync(List<Supplier> list, long Id_Local);

        Task<ActionResponse<Supplier>> DeleteAsync(long id, long Id_local);

        //Task<ActionResponse<Supplier>> ActiveAsync(long id, long Id_local);

        Task<ActionResponse<Supplier>> DeleteFullAsync(long id);

        Task<ActionResponse<Supplier>> UpdateAsync(Supplier model, long Id_Local);
    }
}
