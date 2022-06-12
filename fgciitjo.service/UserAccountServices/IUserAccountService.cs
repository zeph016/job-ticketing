using fgciitjo.domain.clsUserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.UserAccountServices
{
    public interface IUserAccountService
    {
        Task<UserAccount> AuthenticateUser();
        Task<UserAccount> AuthenticateToken(UserAccount userAccount, string token);
    }
}
