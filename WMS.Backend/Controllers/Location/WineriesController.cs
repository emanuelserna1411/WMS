﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMS.Backend.Helpers;
using WMS.Backend.UnitsOfWork.Interfaces.Location;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;

namespace WMS.Backend.Controllers.Location
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class WineriesController : Controller
    {
        private readonly IWineriesUnitOfWork _unitOfWork;
        private readonly IValidateSession _validateSession;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileStorage _fileStorage;
        public WineriesController(IWineriesUnitOfWork unitOfWork, IValidateSession validateSession, IWebHostEnvironment webHostEnvironment, IFileStorage fileStorage)
        {
            _unitOfWork = unitOfWork;
            _validateSession = validateSession;
            _webHostEnvironment = webHostEnvironment;
            _fileStorage = fileStorage;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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

        [HttpGet("genericsearch")]
        public async Task<IActionResult> GetGenericSearchAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 8, "Read");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var response = await _unitOfWork.DownloadAsync(pagination);
            if (response.WasSuccess)
            {
                var List = (List<Winery>)response.Result!;
                var ListReturn =
                    (from item in List
                     select new GenericSearchDTO
                     {
                         Id = item.Id,
                         Name = item.Name,
                         Description = item.Description
                     }).ToList();
                return Ok(ListReturn);
            }
            return BadRequest();
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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

        [HttpGet("downloadasync")]
        public async Task<IActionResult> DownloadAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetTotalPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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

        [HttpGet("deletefull")]
        public async Task<IActionResult> GetDeleteAsync()
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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

        [HttpPost]
        public virtual async Task<IActionResult> PostAsync(Winery model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Create");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            //WineryHelper.FilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
            //Winery winery = await WineryHelper.RegisterFilesAsync(model, _fileStorage);
            var user = AuthForm.Result;
            var action = await _unitOfWork.AddAsync(model, user!.Id_Local);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpDelete("deleteasync/{id}")]
        public virtual async Task<IActionResult> DeleteAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Delete");
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

        [HttpGet("restoreasync/{id}")]
        public async Task<IActionResult> RestoreAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Read");
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

        [HttpDelete("deletefullasync/{id}")]
        public virtual async Task<IActionResult> DeleteFullAsync(long id)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Delete");
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
        public virtual async Task<IActionResult> PutAsync(Winery model)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Update");
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

        [HttpPost("uploadasync")]
        public virtual async Task<IActionResult> UploadAsync(List<Winery> list)
        {
            var AuthForm = await _validateSession.GetValidateSession(HttpContext, 9, "Create");
            if (!AuthForm.WasSuccess)
            {
                return BadRequest(AuthForm.Message);
            }
            var user = AuthForm.Result;
            var action = await _unitOfWork.AddListAsync(list, user!.Id_Local);
            return Ok(action);
        }
    }
}
