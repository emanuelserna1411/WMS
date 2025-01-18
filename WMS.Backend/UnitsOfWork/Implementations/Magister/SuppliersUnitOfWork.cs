using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.UnitsOfWork.Implementations.Magister
{
    public class SuppliersUnitOfWork : ISuppliersUnitOfWork
    {
        private readonly ISuppliersRepository _repository;

        public SuppliersUnitOfWork(ISuppliersRepository repository)
        {
            _repository = repository;
        }

        public Task<ActionResponse<Supplier>> ActiveAsync(long id, long Id_local)=>_repository.ActiveAsync(id, Id_local);

        public Task<ActionResponse<Supplier>> AddAsync(Supplier model, long Id_Local)=>_repository.AddAsync(model, Id_Local);

        public Task<ActionResponse<List<Supplier>>> AddListAsync(List<Supplier> list, long Id_Local)=>_repository.AddListAsync(list, Id_Local);

        public Task<ActionResponse<Supplier>> DeleteAsync(long id, long Id_local) => _repository.DeleteAsync(id, Id_local);

        public Task<ActionResponse<Supplier>> DeleteFullAsync(long id)=>_repository.DeleteFullAsync(id);

        public Task<ActionResponse<IEnumerable<Supplier>>> DownloadAsync(PaginationDTO pagination)=>_repository.DownloadAsync(pagination);

        public Task<ActionResponse<Supplier>> GetAsync(long id)=>_repository.GetAsync(id);

        public Task<ActionResponse<IEnumerable<Supplier>>> GetAsync()=>_repository.GetAsync();

        public Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination)=>_repository.GetAsync(pagination);

        public Task<ActionResponse<IEnumerable<Supplier>>> GetDeleteAsync()=>_repository.GetDeleteAsync();

        public Task<ActionResponse<IEnumerable<Supplier>>> GetDeleteAsync(PaginationDTO pagination)=>_repository.GetDeleteAsync(pagination);

        public Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination) => _repository.GetDeleteTotalPagesAsync(pagination);

        public Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)=>_repository.GetTotalPagesAsync(pagination);

        public Task<ActionResponse<Supplier>> UpdateAsync(Supplier model, long Id_Local) => _repository.UpdateAsync(model,Id_Local);
    }
}
