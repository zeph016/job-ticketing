using fgciitjo.domain.clsDepartment;
using fgciitjo.domain.clsEmployee;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsUserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.GlobalServices
{
    public interface IGlobalService
    {
        Task<List<Department>> GetAllDepartment(string token);
        Task<List<UserAccount>> GetAllEmployees(FilterParameter filterParameter, string token);
        Task<List<Employee>> GetDepartmentEmployees(FilterParameter filterParameter, string token);
        Task<DateTime> LoadServerTime();
        Task<UserAccount> GetEmployeeV2(string token, long employeeId);
    }
}
