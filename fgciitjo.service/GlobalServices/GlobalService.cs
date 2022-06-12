using fgciitjo.domain.clsDepartment;
using fgciitjo.domain.clsEmployee;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsUserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace fgciitjo.service.GlobalServices
{
    public class GlobalService : IGlobalService
    {
        public GlobalService(HttpClient _client)
        {
            this.client = _client;
        }

        #region Properties
        HttpClient client;
        #endregion

        #region Methods

        #region Get All Employees
        public async Task<List<UserAccount>> GetAllEmployees(FilterParameter filterParameter, string token)
        {
            try
            {
                List<UserAccount> userAccounts = new List<UserAccount>();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("masterlist/employees/fgciemployees", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    userAccounts = await response.Content.ReadAsAsync<List<UserAccount>>();
                }
                return userAccounts.OrderBy(x => x.EmployeeName).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Get All Department
        public async Task<List<Department>> GetAllDepartment(string token)
        {
            try
            {
                List<Department> departments = new List<Department>();

                FilterParameter filterParameter = new FilterParameter();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync("masterlist/employees/departments");
                if (response.IsSuccessStatusCode)
                {
                    departments = await response.Content.ReadAsAsync<List<Department>>();
                }
                return departments;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        public async Task<List<Employee>> GetDepartmentEmployees(FilterParameter filterParameter, string token)
        {
            try
            {
                List<Employee> employees = new List<Employee>();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("masterlist/employees/department-employee", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    employees = await response.Content.ReadAsAsync<List<Employee>>();
                }
                return employees.OrderBy(x => x.EmployeeName).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DateTime> LoadServerTime()
        {
            try
            {
                DateTime serverDate = new DateTime();
                HttpResponseMessage responseMessage = await client.GetAsync("/ticket/server-date");
                if (responseMessage.IsSuccessStatusCode)
                {
                    serverDate = await responseMessage.Content.ReadAsAsync<DateTime>();
                }
                return serverDate;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return DateTime.Now;
            }
        }

        public async Task<UserAccount> GetEmployeeV2(string token, long employeeId)
        {
            try
            {
                UserAccount userAccount = new UserAccount();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.GetAsync($"masterlist/employees/{employeeId}");
                if (responseMessage.IsSuccessStatusCode)
                    userAccount = JsonConvert.DeserializeObject<UserAccount>(await responseMessage.Content.ReadAsStringAsync());
                return userAccount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion
    }
}
