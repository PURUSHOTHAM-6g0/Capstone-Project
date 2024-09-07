using Authentication.Contracts;
using Authentication.Dto;
using Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Repository
{
    public class AuthenticationRepository : IAuthentication
    {
        private readonly UserManager<Register> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthenticationRepository(UserManager<Register> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<Response> AdminRegister(Register model)
        {
            var userExists = await _userManager.FindByIdAsync(model.EmployeeId);
            string timeZone = await GetTimeZoneFromCountry(model.Country);
            if (userExists != null)
                return new Response { Status = "Error", Message = "User already exists!" };
            Register user = new()
            {
                EmployeeId = model.EmployeeId,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Country = model.Country,
                TimeZone = timeZone
            };
            var result = await _userManager.CreateAsync(user, model.PasswordHash);
            if (!result.Succeeded)
                return new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." };
            if (!await _roleManager.RoleExistsAsync(Roles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            if (!await _roleManager.RoleExistsAsync(Roles.User))
                await _roleManager.CreateAsync(new IdentityRole(Roles.User));
            if (await _roleManager.RoleExistsAsync(Roles.Admin))
            {
                await _userManager.AddToRoleAsync(user, Roles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(Roles.User))
            {
                await _userManager.AddToRoleAsync(user, Roles.User);
            }
            return new Response { Status = "Success", Message = "User created successfully!" };
        }

        public async Task<object> CheckLoginCred(Login model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = GetToken(authClaims);
                return new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    roles=userRoles
                };
            }
            return null;
        }

        public async Task<Response> UserRegister(Register model)
        {
            var userExists = await _userManager.FindByIdAsync(model.EmployeeId);
            string timeZone = await GetTimeZoneFromCountry(model.Country);
            if (userExists != null)
                return new Response { Status = "Error", Message = "User already exists!" };
            Register user = new()
            {
                EmployeeId = model.EmployeeId,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Country = model.Country,
                TimeZone = timeZone
            };
            var result = await _userManager.CreateAsync(user, model.PasswordHash);
            if (!result.Succeeded)
            {
                return new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." };
            }
            return new Response { Status = "Success", Message = "User created successfully!" };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTAuth:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWTAuth:ValidIssuerURL"],
                audience: _configuration["JWTAuth:ValidAudienceURL"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
            return token;
        }

        public async Task<string> GetTimeZoneFromCountry(string isoCountryCode)
        {
            string apiKey = "7Q2Q2081QMOO"; 
            using (var httpClient = new HttpClient())
            {
                string apiUrl = $"https://api.timezonedb.com/v2.1/list-time-zone?key={apiKey}&format=json&country={isoCountryCode}";

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseData);
                    var timeZones = jsonResponse?.zones;
                    return timeZones?[0]?.zoneName?.ToString();
                }
            }
            return null;
        }

        public async Task<GetEmployeeDto> GetEmployeeByName(string Name)
        {
            var getEmployee = await _userManager.FindByNameAsync(Name);
            GetEmployeeDto employee = new GetEmployeeDto()
            {
                UserName = getEmployee.UserName,
                Email = getEmployee.Email,
                EmployeeId = getEmployee.EmployeeId,
                TimeZone = getEmployee.TimeZone,
            };
            return employee;

        }

        public async Task<IEnumerable<GetEmployeeDto>> GetAllEmployees()
        {
            List<GetEmployeeDto> getEmployeeDtos = new List<GetEmployeeDto>();
            foreach (var item in _userManager.Users)
            {
                GetEmployeeDto employeeDto = new GetEmployeeDto()
                {
                    Email=item.Email,
                    EmployeeId=item.EmployeeId,
                    TimeZone = item.TimeZone,
                    UserName=item.UserName
                };
                getEmployeeDtos.Add(employeeDto);
            }
            return getEmployeeDtos;
        }

        public async Task<GetEmployeeDto> GetEmployeeById(string employeeId)
        {
            var getEmployee = await _userManager.Users
                        .FirstOrDefaultAsync(u => u.EmployeeId == employeeId);

            GetEmployeeDto employee = new GetEmployeeDto()
            {
                UserName = getEmployee.UserName,
                Email = getEmployee.Email,
                EmployeeId = getEmployee.EmployeeId,
                TimeZone = getEmployee.TimeZone,
            };
            return employee;
        }
    }
}
