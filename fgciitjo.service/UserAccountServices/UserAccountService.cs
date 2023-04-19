using fgciitjo.domain.clsUserAccount;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace fgciitjo.service.UserAccountServices
{
    public class UserAccountService : IUserAccountService
    {
        public UserAccountService(HttpClient _client) => client = _client;

        #region Properties

        private IConfiguration configuration;
        private HttpClient client;

        #endregion

        #region Methods

        #region Authenticate User
        public async Task<UserAccount> AuthenticateUser()
        {
            try
            {
                using (client = new HttpClient())
                {
                    
                    HttpResponseMessage response = await client.PostAsJsonAsync(
                     configuration.GetValue<string>("Connection:FGCIWebAPIConnection") + "/hub/accounts/authenticate", new
                     {
                         //username = userAccount.UserName,
                         //password = userAccount.Password,
                         //systemName = userAccount.SystemName
                     });
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<UserAccount>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        #endregion

        #region Authenticate Token

        public async Task<UserAccount> AuthenticateToken(UserAccount userAccount, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var parameter = JsonConvert.SerializeObject(new { systemname = userAccount.SystemName});
                var bodyContent = new StringContent(parameter, Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync("/hub/accounts/authenticate/system", bodyContent);
                if (httpResponse.IsSuccessStatusCode)
                {
                    userAccount = await httpResponse.Content.ReadAsAsync<UserAccount>();
                    userAccount.httpResponse = httpResponse.StatusCode;
                }
                else
                    userAccount.httpResponse = httpResponse.StatusCode;
                return userAccount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #endregion
    }
}
