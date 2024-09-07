using Authentication.Dto;
using Authentication.Models;

namespace Authentication.Contracts
{
    public interface IAuthentication
    {
        public Task<object> CheckLoginCred(Login model);

        public Task<Response> UserRegister(Register model);

        public Task<Response> AdminRegister(Register model);

        public Task<GetEmployeeDto> GetEmployeeByName(string Name);
        public  Task<string> GetTimeZoneFromCountry(string isoCountryCode);

        public Task<IEnumerable<GetEmployeeDto>> GetAllEmployees();
        public Task<GetEmployeeDto> GetEmployeeById(string employeeId);
    }
}
