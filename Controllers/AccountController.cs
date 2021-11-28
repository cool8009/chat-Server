using BackgammonChat.Services;
using ChatService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatService
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly TokenService tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            
            var user = await userManager.FindByNameAsync(loginDto.UserName);
            if(user == null) return Unauthorized();

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password , false);

            if(result.Succeeded)
            {
                return new UserDto
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    Token = tokenService.CreateToken(user)
                };
            }
            return Unauthorized();
        }

         [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.UserName,
                FirstName = registerDto.FirstName    
            };
            var result = await userManager.CreateAsync(user, registerDto.Password);

            if(result.Succeeded)
            {
             return CreateUserObject(user);   
            }
            return BadRequest(" problam registering user");
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Name));
            return CreateUserObject(user);
        }
     
        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName
                };
        }
    }
}