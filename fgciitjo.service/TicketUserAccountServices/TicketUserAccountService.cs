using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketUserAccount;

namespace fgciitjo.service.TicketUserAccountServices
{
    public class TicketUserAccountService : ITicketUserAccountService
    {
        private HttpClient client;
        List<TicketUserAccount> ticketuserList;
        TicketUserAccount ticketUser;
        public TicketUserAccountService(HttpClient client_)
        {
            client = client_;
            ticketuserList = new List<TicketUserAccount>();
            ticketUser = new TicketUserAccount();
        }

        public async Task<TicketUserAccount> AddTicketUserAccount(TicketUserAccount ticketUserAccount, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-useraccount", ticketUserAccount);
                if (response.IsSuccessStatusCode)
                {
                    ticketUser = await response.Content.ReadAsAsync<TicketUserAccount>();
                }
                return ticketUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TicketUserAccount> GetOneTicketUserAccount(long HubUserId, string token)
        {
            try
            {
                //TicketUserAccount ticketUser = new TicketUserAccount();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync("ticket-useraccount/" + HubUserId);
                if (response.IsSuccessStatusCode)
                {
                    ticketUser = await response.Content.ReadAsAsync<TicketUserAccount>();
                }
                return ticketUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketUserAccount>> LoadTicketUserAccount(FilterParameter filterParameter, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-useraccount/list", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketuserList = await response.Content.ReadAsAsync<List<TicketUserAccount>>();
                }
                return ticketuserList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TicketUserAccount> UpdateTicketUserAccount(TicketUserAccount ticketUserAccount, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PutAsJsonAsync("ticket-useraccount", ticketUserAccount);
                if (response.IsSuccessStatusCode)
                {
                    ticketUser = await response.Content.ReadAsAsync<TicketUserAccount>();
                }
                return ticketUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
