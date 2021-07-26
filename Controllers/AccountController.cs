using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _usermanager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper  _mapper;
        private readonly IAuthManager  _auhManager;
        public AccountController(UserManager<ApiUser> usermanager,ILogger<AccountController> logger, IMapper mapper, IAuthManager auhManager)
        {
            _usermanager = usermanager;
            _logger = logger;
            _mapper = mapper;
            _auhManager = auhManager;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)

        {
            _logger.LogInformation($"Register attempt {userDto.Email}");
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.FirstName+userDto.LastName;
                user.Email =  userDto.Email;
                var result = await _usermanager.CreateAsync(user,userDto.Password);
                if(!result.Succeeded)
                {
                    foreach (var error in result.Errors)

                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }
              var res=  await _usermanager.AddToRolesAsync(user,userDto.Roles);
                return Accepted();


            }
            catch (Exception ex)
            {
                return Problem(ex.ToString(), statusCode: 500);


            }

        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDto)

        {
            _logger.LogInformation($"Login attempt {userDto.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {


                if (!await _auhManager.ValidateUser(userDto))
                {
                    return Unauthorized();
                }
                return Accepted(new { Token=await _auhManager.CreateToken()});

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "dadadad");
                return Problem($"something went wrong { nameof(Login)}", statusCode: 500);


            }

        }
    }
    
}
