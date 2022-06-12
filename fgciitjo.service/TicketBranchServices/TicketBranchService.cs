using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketBranch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketBranchServices
{
    public class TicketBranchService : ITicketBranchService
    {
        private readonly HttpClient client;
        public TicketBranchService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<TicketBranchModel> AddBranch(TicketBranchModel ticketBranch, string token)
        {
            TicketBranchModel branchTicket = new TicketBranchModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.PostAsJsonAsync("ticket-branch", ticketBranch);
            if (response.IsSuccessStatusCode)
            {
                branchTicket = JsonConvert.DeserializeObject<TicketBranchModel>(await response.Content.ReadAsStringAsync());
            }
            return branchTicket;
        }

        public async Task<List<TicketBranchModel>> GetBranch(FilterParameter filterParameter, string token)
        {
            List<TicketBranchModel> ticket = new List<TicketBranchModel>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.PostAsJsonAsync("ticket-branch/list" , filterParameter);
            response.EnsureSuccessStatusCode();
            ticket = JsonConvert.DeserializeObject<List<TicketBranchModel>>(await response.Content.ReadAsStringAsync());
            return ticket;
        }

        public async Task<TicketBranchModel> UpdateBranch(TicketBranchModel ticketBranch, string token)
        {
            TicketBranchModel branchTicket = new TicketBranchModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.PutAsJsonAsync("ticket-branch", ticketBranch);
            if (response.IsSuccessStatusCode)
            {
                branchTicket = JsonConvert.DeserializeObject<TicketBranchModel>(await response.Content.ReadAsStringAsync());
            }
            return branchTicket;
        }
    }
}
