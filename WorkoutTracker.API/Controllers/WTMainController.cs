using Asp.Versioning;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Entities;
using Shared.PlannerDTO;
using Shared.WorkoutDTO;
using WorkoutTracker.API.GeneralServices;
using WorkoutTracker.API.LoginServices;

namespace WorkoutTracker.API.Controllers
{
    [Route("api/[controller]/v{version:apiVersion}")]
    [Asp.Versioning.ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    public class WTMainController : ControllerBase
    {
        private readonly LoginService _loginServices;
        private readonly IGeneralService _generalService;
        public WTMainController(LoginService loginService, IGeneralService generalService)
        {
            _loginServices = loginService;
            _generalService = generalService;
        }

        #region Login/Registration
        [AllowAnonymous]
        [HttpPost("WebLogin")]
        public async Task<IActionResult> WebLogin(WebLoginDto LoginDto)
        {
            var response = await _loginServices.WebLogin(LoginDto);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            if (response.RespCode == "401")
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("WebRegister")]
        public async Task<IActionResult> WebRegister(WebRegisterDto registerDto)
        {
            var response = await _loginServices.WebRegister(registerDto);

            if (response.RespCode == "409")
                return Conflict(response);

            if (response.RespCode != "200")
                return BadRequest(response);

            return Ok(response);
        }

        #endregion

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _generalService.GetAllUsers(HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var response = await _generalService.GetUserInfo(HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("PostUserInfo")]

        public async Task<IActionResult> PostUserInfo(User userDto)
        {
            var response = await _generalService.PostUserInfo(userDto, HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }





        #region Planner

        [HttpPost("CreateTemplate")]
        public async Task<IActionResult> CreateTemplate(TemplateUpsertDto template)
        {
            var response = await _generalService.CreateTemplate(template, HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }

        [HttpPost("UpdateTemplate")]
        public async Task<IActionResult> UpdateTemplate(long templateId, TemplateUpsertDto template)
        {
            var response = await _generalService.UpdateTemplate(templateId, template, HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }

        [HttpPost("DeleteTemplate")]
        public async Task<IActionResult> DeleteTemplate(long templateId)
        {
            var response = await _generalService.DeleteTemplate(templateId, HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }

        [HttpGet("GetTemplates")]
        public async Task<IActionResult> GetTemplates()
        {
            var response = await _generalService.GetTemplates(HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }
       

        [HttpPost("GetTemplateDetail")]
        public async Task<IActionResult> GetTemplateDetail(long templateId)
        {
            var response = await _generalService.GetTemplateDetail(templateId, HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }



        #endregion



        #region Exercises

        [HttpGet("GetExercises")]
        public async Task<IActionResult> GetExercises()
        {
            var response = await _generalService.GetExercises(HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }

        #endregion


        #region Workout Sessions

        [HttpPost("AddWorkoutSession")]
        public async Task<IActionResult> AddWorkoutSession(WorkoutSessionCreateDto workout)
        {
            var response = await _generalService.AddWorkoutSession(workout, HttpContext);
            if (response.RespCode != "200")
            {
                return BadRequest(response);
            }
            return Ok(response);


        }

        #endregion
    }
}
