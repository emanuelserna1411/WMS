using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Interfaces.Magister;
using WMS.Share.DTOs;

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




    }
}
