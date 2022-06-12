using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using fgciitjo.domain.clsUserAccount;
using fgciitjo.domain.clsDepartment;

namespace fgciitjo.service.EmployeeAccountServices
{
    public interface IEmployeeAccountService
    {
        Task<IEnumerable<UserAccount>> GetDepartmentPersons(int deptId, string Token);
        Task<IEnumerable<Department>> GetDepartment(string Token);
        Task<UserAccount> GetEmployee(string Token, long employeeId);
    }
}