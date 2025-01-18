using Microsoft.EntityFrameworkCore;
using WMS.Backend.Data;
using WMS.Backend.Helpers;
using WMS.Backend.Repositories.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.Backend.Repositories.Implementations.Magister
{
    public class SupplierRepository : ISuppliersRepository
    {
        private readonly DataContext _context;

        public SupplierRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<Supplier>> ActiveAsync(long id, long Id_local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.Suppliers.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (!model.Delete)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "El registro ya se encuentra activo"
                };
            }
            model.UpdateUserId = Id_local;
            model.UpdateDate = DateTime.Now;
            model.Delete = false;
            _context.Update(model);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Supplier>
                {
                    WasSuccess = true,
                    Result = model
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        public async Task<ActionResponse<Supplier>> AddAsync(Supplier model, long Id_Local)
        {
            var user = _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user is null) 
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            try
            {
                model.CreateUserId = Id_Local;
                model.CreateDate = DateTime.Now;
                model.UpdateDate = DateTime.Now;
                model.UpdateUserId = Id_Local;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return new ActionResponse<Supplier>
                {
                    WasSuccess = true,
                    Result = model
                };
            }
            catch (Exception ex) 
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return new ActionResponse<Supplier>
                        {
                            WasSuccess = false,
                            Message = ex.Message
                        };
                    }
                }

                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<List<Supplier>>> AddListAsync(List<Supplier> list, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<List<Supplier>>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario",
                    Result = list
                };
            }
            using var transaction = _context.Database.BeginTransaction();

            foreach (var item in list)
            {
                try
                {
                    if (item.Update == false)
                    {

                        item.CreateUserId = Id_Local;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;
                        item.UpdateUserId = Id_Local;
                        _context.Add(item);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        Supplier? model = null;
                        if (item.Id != 0)
                        {
                            model = await _context.Suppliers.Where(w => w.Delete == false && w.Id == item.Id).FirstOrDefaultAsync();
                            if (model == null)
                            {
                                item.StrError = "No se encuentra el registro";
                                continue;
                            }
                        }
                        else
                        {
                            model = await _context.Suppliers.Where(w => w.Delete == false && w.Id == item.Id).FirstOrDefaultAsync();
                            if (model == null)
                            {
                                item.StrError = "No se encuentra el registro";
                                continue;
                            }
                        }

                        model.Id = item.Id;
                        model.Address = item.Address;
                        model.DocumentTypeUserId = item.DocumentTypeUserId;
                        model.DocumentTypeUser = item.DocumentTypeUser;
                        model.Document = item.Document;
                        model.CompanyName = item.CompanyName;
                        model.FirstName = item.FirstName;
                        model.LastName = item.LastName;
                        model.Email = item.Email;
                        model.Phone = item.Phone;
                        model.Attachment1 = item.Attachment1;
                        model.Attachment2 = item.Attachment2;
                        model.Attachment3 = item.Attachment3;
                        model.Attachment4 = item.Attachment4;
                        model.Attachment5 = item.Attachment5;
                        model.UpdateUserId = Id_Local;
                        model.UpdateDate = DateTime.Now;
                        _context.Update(model);
                        await _context.SaveChangesAsync();

                    }

                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException!.Message.Contains("duplicate"))
                        {
                            item.StrError = "Ya existe este registro o está en eliminados";
                        }
                        else
                        {
                            item.StrError = ex.Message;
                        }
                    }
                    else
                    {
                        item.StrError = ex.Message;
                    }

                }
                catch (Exception ex)
                {
                    item.StrError = ex.Message;
                }
            }
            var listerror = list.Where(w => w.StrError != null).ToList();
            if (listerror.Count > 0)
            {
                transaction.Rollback();
                return new ActionResponse<List<Supplier>>
                {
                    WasSuccess = false,
                    Message = "Revisar Listado de errores",
                    Result = listerror
                };
            }
            await _context.SaveChangesAsync();
            transaction.Commit();
            return new ActionResponse<List<Supplier>>
            {
                WasSuccess = true,
                Message = "No se encuentra usuario",
                Result = list,
            };

        }

        public async Task<ActionResponse<Supplier>> DeleteAsync(long id, long Id_local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            var model = await _context.Suppliers.FindAsync(id);
            if (model == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            if (model.Delete)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "El registro ya se encuentra eliminado"
                };
            }
            model.DeleteUserId = Id_local;
            model.DeleteDate = DateTime.Now;
            model.Delete = true;
            _context.Update(model);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Result = model
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }

        }

        public async Task<ActionResponse<Supplier>> DeleteFullAsync(long id)
        {
            try
            {
                var model = await _context.Suppliers.FindAsync(id);
                if (model == null)
                {
                    return new ActionResponse<Supplier>
                    {
                        WasSuccess = false,
                        Message = "No se encuentra el registro"
                    };
                }
                if (!model.Delete)
                {
                    return new ActionResponse<Supplier>
                    {
                        WasSuccess = false,
                        Message = "No se puede eliminar definitivamente hasta que no se elimine previamente"
                    };
                }
                try
                {
                    _context.Remove(model);
                    await _context.SaveChangesAsync();
                    return new ActionResponse<Supplier>
                    {
                        WasSuccess = true
                    };
                }
                catch
                {
                    return new ActionResponse<Supplier>
                    {
                        WasSuccess = false,
                        Message = "No se pude borrar, porque tiene registros relacionados."
                    };
                }
            }
            catch (Exception ex)
            {

                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ActionResponse<IEnumerable<Supplier>>> DownloadAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.Where(w => w.Delete == false).Include(I => I.DocumentTypeUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.DocumentTypeUserId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName!.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Supplier>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Id).ToListAsync()
            };
        }

        public async Task<ActionResponse<Supplier>> GetAsync(long id)
        {
            var model = await _context.Suppliers.Include(I => I.DocumentTypeUser).FirstOrDefaultAsync(w => w.Id == id);
            if (model == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }
            return new ActionResponse<Supplier>
            {
                WasSuccess = true,
                Result = model
            };
        }

        public async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync()
        {
            return new ActionResponse<IEnumerable<Supplier>>
            {
                WasSuccess = true,
                Result = await _context.Suppliers.Where(w => w.Delete == false).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Supplier>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.Where(w => w.Delete == false).Include(I => I.DocumentTypeUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.DocumentTypeUserId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName!.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Supplier>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(o => o.Id).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Supplier>>> GetDeleteAsync()
        {
            return new ActionResponse<IEnumerable<Supplier>>
            {
                WasSuccess = true,
                Result = await _context.Suppliers.Where(w => w.Delete == true).ToListAsync()
            };
        }

        public async Task<ActionResponse<IEnumerable<Supplier>>> GetDeleteAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.Where(w => w.Delete == true).Include(I => I.DocumentTypeUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.DocumentTypeUserId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName!.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Supplier>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetDeleteTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.Where(w => w.Delete == true).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            {
                queryable = queryable.Where(x => x.DocumentTypeUserId.ToString() == pagination.Filter1);
            }
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName!.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Suppliers.Where(w => w.Delete == false).AsQueryable();
            //if (!string.IsNullOrWhiteSpace(pagination.Filter1))
            //{
            //    queryable = queryable.Where(x => x.BranchId.ToString() == pagination.Filter1);
            //}
            //if (!string.IsNullOrWhiteSpace(pagination.Filter))
            //{
            //    queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            //}
            var count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<Supplier>> UpdateAsync(Supplier model, long Id_Local)
        {
            var user = await _context.Users.Where(w => w.Id_Local == Id_Local).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra usuario"
                };
            }
            if (model == null)
            {
                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = "No se encuentra el registro"
                };
            }

            try
            {
                var supplier = await _context.Suppliers.FindAsync(model.Id);
                
                supplier!.CompanyName = model.CompanyName;
                supplier.Document = model.Document;
                supplier.DocumentTypeUserId = model.DocumentTypeUserId;
                supplier.FirstName = model.FirstName;
                supplier.LastName = model.LastName;
                supplier.Email = model.Email;
                supplier.Address = model.Address;
                supplier.Attachment1 = model.Attachment1;
                supplier.Attachment2 = model.Attachment2;
                supplier.Attachment3 = model.Attachment3;
                supplier.Attachment4 = model.Attachment4;
                supplier.Attachment5 = model.Attachment5;

                _context.Update(supplier);
                await _context.SaveChangesAsync();
                return new ActionResponse<Supplier>
                {
                    WasSuccess = true,
                    Result = model
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException!.Message.Contains("duplicate"))
                    {
                        return DbUpdateExceptionActionResponse();
                    }
                }

                return new ActionResponse<Supplier>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
            catch (Exception exception)
            {
                return ExceptionActionResponse(exception);
            }
        }

        private ActionResponse<Supplier> DbUpdateExceptionActionResponse()
        {
            return new ActionResponse<Supplier>
            {
                WasSuccess = false,
                Message = "Ya existe el registro que estas intentando crear."
            };
        }

        private ActionResponse<Supplier> ExceptionActionResponse(Exception exception)
        {
            return new ActionResponse<Supplier>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }
}
