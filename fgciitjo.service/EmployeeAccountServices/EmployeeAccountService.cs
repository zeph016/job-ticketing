using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using fgciitjo.domain.clsUserAccount;
using fgciitjo.domain.clsDepartment;

namespace fgciitjo.service.EmployeeAccountServices
{
    public class EmployeeAccountService : IEmployeeAccountService
    {
        private readonly HttpClient _httpClient;

        public EmployeeAccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<UserAccount>> GetDepartmentPersons(int deptId, string Token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var apiResponse = await _httpClient.GetStreamAsync($"hub/accounts/department={deptId}");
            return await JsonSerializer.DeserializeAsync<IEnumerable<UserAccount>>
                    (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<IEnumerable<Department>> GetDepartment(string Token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            var apiResponse = await _httpClient.GetStreamAsync($"masterlist/employees/departments");
            return await JsonSerializer.DeserializeAsync<IEnumerable<Department>>
                    (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<UserAccount> GetEmployee(string Token, long employeeId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var apiResponse = await _httpClient.GetStreamAsync($"masterlist/employees/{employeeId}");
                return await JsonSerializer.DeserializeAsync<UserAccount>
                    (apiResponse, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "I am error");
                return null;
            }
        }
    }
}