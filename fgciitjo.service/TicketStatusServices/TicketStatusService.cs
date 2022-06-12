using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketStatus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketStatusServices
{
    public class TicketStatusService : ITicketStatusService
    {
        private readonly HttpClient client;
        public TicketStatusService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<TicketStatusModel> AddTicketStatus(TicketStatusModel ticketStatus, string token)
        {
            TicketStatusModel status = new TicketStatusModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , token);
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync("ticket-status", ticketStatus);
            if (responseMessage.IsSuccessStatusCode)
            {
                status = JsonConvert.DeserializeObject<TicketStatusModel>(await responseMessage.Content.ReadAsStringAsync());
            }
            return status;
        }

        public async Task<List<TicketStatusModel>> GetTicketStatus(FilterParameter filterParameter, string token)
        {
            List<TicketStatusModel> ticket = new List<TicketStatusModel>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.PostAsJsonAsync("ticket-status/list", filterParameter);
            response.EnsureSuccessStatusCode();
            ticket = JsonConvert.DeserializeObject<List<TicketStatusModel>>(await response.Content.ReadAsStringAsync());
            return ticket.OrderBy(x=>x.StatusName).ToList();
        }

        public async Task<TicketStatusModel> GetTicketStatusById(long Id, string token)
        {
            TicketStatusModel model = new TicketStatusModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.GetAsync("ticket-status/" + Id);
            if (response.IsSuccessStatusCode)
            {
                model = JsonConvert.DeserializeObject<TicketStatusModel>(await response.Content.ReadAsStringAsync());
            }
            return model;
        }

        public async Task<TicketStatusModel> UpdateTickeStatus(TicketStatusModel ticketStatus, string token)
        {
            TicketStatusModel status = new TicketStatusModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync("ticket-status", ticketStatus);
            if (responseMessage.IsSuccessStatusCode)
            {
                status = JsonConvert.DeserializeObject<TicketStatusModel>(await responseMessage.Content.ReadAsStringAsync());
            }
            return status;
        }
    }
}
