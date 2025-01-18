using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;

namespace WMS.Backend.Controllers.Magister
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISuppliersUnitOfWork _unitOfWork;
        private readonly IValidateSession _validateSession;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileStorage _fileStorage;

        public SupplierController(ISuppliersUnitOfWork unitOfWork, IValidateSession validateSession, IWebHostEnvironment webHostEnvironment, IFileStorage fileStorage)
        {
            _unitOfWork = unitOfWork;
            _validateSession = validateSession;
            _webHostEnvironment = webHostEnvironment;
            _fileStorage = fileStorage;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("getasync")]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetTotalPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _unitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync(Supplier model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Create");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            FileStorageHelper.FilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
            Supplier supplier = await FileStorageHelper.RegisterFilesAsync(model, _fileStorage);
            supplier.DocumentTypeUserId = 1;
            var user = AuthForm.Result;
            var action = await _unitOfWork.AddAsync(supplier, user!.Id_Local);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpGet("downloadasync")]
        public async Task<IActionResult> DownloadAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.DownloadAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpDelete("deleteasync/{id}")]
        public virtual async Task<IActionResult> DeleteAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Delete");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var response = await _unitOfWork.DeleteAsync(id, user!.Id_Local);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }

        [HttpGet("deletefull")]
        public async Task<IActionResult> GetDeleteAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetDeleteAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("getdeleteasync")]
        public async Task<IActionResult> GetDeleteAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.GetDeleteAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("deletetotalPages")]
        public async Task<IActionResult> GetDeleteTotalPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var action = await _unitOfWork.GetDeleteTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpDelete("deletefullasync/{id}")]
        public virtual async Task<IActionResult> DeleteFullAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Delete");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.DeleteFullAsync(id);
            if (response.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }

        [HttpPut]
        public virtual async Task<IActionResult> PutAsync(Supplier model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Update");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var action = await _unitOfWork.UpdateAsync(model, user!.Id_Local);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpGet("restoreasync/{id}")]
        public async Task<IActionResult> RestoreAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 16, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var response = await _unitOfWork.ActiveAsync(id, user!.Id_Local);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }


    }
}
