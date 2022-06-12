using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicket;
using fgciitjo.domain.clsTicketAuditTrail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketListServices
{
    public class TicketListService : ITicketListService
    {
        public TicketListService(HttpClient httpClient_)
        {
            httpClient = httpClient_;
            ticketModels = new List<TicketModel>();
            ticketAuditTrails = new List<TicketAuditTrail>();
        }

        HttpClient httpClient;
        List<TicketModel> ticketModels;
        List<TicketAuditTrail> ticketAuditTrails;
        public async Task<List<TicketModel>> LoadTicket(FilterParameter filterParameter, string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("ticket/list", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketModels = await response.Content.ReadAsAsync<List<TicketModel>>();
                }
                // return ticketModels.Where(x => x.TicketStatusTypeId != domain.clsEnums.Enums.TicketStatusType.Complete).ToList();
                return ticketModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<TicketAuditTrail>> LoadTicketAuditTrail(long Id, string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync("ticket-audittrail/list/" + Id);
                if (response.IsSuccessStatusCode)
                {
                    ticketAuditTrails = await response.Content.ReadAsAsync<List<TicketAuditTrail>>();
                }
                return ticketAuditTrails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketModel>> LoadTicketv2(FilterParameter filterParameter, string token)
        {
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("ticket/dashboard-list", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketModels = await response.Content.ReadAsAsync<List<TicketModel>>();
                }
                return ticketModels;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
  }
}
