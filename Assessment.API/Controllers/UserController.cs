using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Assessment.API.Models;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Assessment.API.Data;
using System.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Assessment.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRoles> _roleManager;
        private readonly IConfiguration _configuration;

        public UserController(
            UserManager<User> userManager,
            RoleManager<UserRoles> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [SwaggerOperation(Summary = "Authenticate an already existing user and get bearer token for subsequent endpoint calls")]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            var user = await _userManager.FindByNameAsync(credentials.Username);
            var validPassword = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (user != null && validPassword)
            {

                var authClaims = new HashSet<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                //role management out of scope but can be read from DB
                authClaims.Add(new Claim(ClaimTypes.Role, "Admin"));

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        [SwaggerOperation(Summary = "Create a new user.")]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            var userExists = await _userManager.FindByNameAsync(newUser.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
            
            IdentityUser user = new()
            {
                Email = newUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = newUser.UserName
            };
            var result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        }

        [HttpPut]
        [Route("update")]
        [SwaggerOperation(Summary = "Edit user details based. Requires bearer token authentication - generated after login")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            return Ok(new Response { Status = "Success", Message = "User details updated successfully!" });
        }

        [HttpDelete]
        [Route("delete")]
        [SwaggerOperation(Summary = "Delete an existing user. Requires bearer token authentication - generated after login")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var userExists = await _userManager.FindByIdAsync(userId);
            if (userExists == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User does not exist!" });

            var result = await _userManager.DeleteAsync(userExists);
            if (result.Succeeded)
                return Ok(new Response { Status = "Success", Message = "User has been deleted successfully!" });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User deletion failed! Please check the user details provided and retry." });

        }

        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
