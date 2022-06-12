using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketActivity;
using fgciitjo.domain.clsTicketActivityReport;

namespace fgciitjo.service.TicketActivityServices
{
    public class TicketActivityService : ITicketActivityService
    {
        HttpClient client;
        List<TicketActivityModel> ticketActivityModels;
        TicketActivityModel ticketActivityModel;
        List<TicketActivityReportModel> ticketActivityReports;
        public TicketActivityService(HttpClient client_)
        {
            client = client_;
            ticketActivityModels = new List<TicketActivityModel>();
            ticketActivityModel = new TicketActivityModel();
            ticketActivityReports = new List<TicketActivityReportModel>();
        }

        public async Task<TicketActivityModel> AddTicketActivity(TicketActivityModel ticketActivityModel, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-activity", ticketActivityModel);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModel = await response.Content.ReadAsAsync<TicketActivityModel>();
                }
                return ticketActivityModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TicketActivityModel> GetActivityById(long Id, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync("ticket-activity/activity/" + Id);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModel = await response.Content.ReadAsAsync<TicketActivityModel>();
                }
                return ticketActivityModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketActivityReportModel>> GetActivityReport(FilterParameter filterParameter, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-report/daily-accomplishment", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityReports = await response.Content.ReadAsAsync<List<TicketActivityReportModel>>();
                }
                return ticketActivityReports;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketActivityModel>> GetAllActivityById(long id, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync("ticket-activity/list/" + id);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModels = await response.Content.ReadAsAsync<List<TicketActivityModel>>();
                }
                return ticketActivityModels;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

     

        public async Task<TicketActivityModel> UpdateTicketActivity(TicketActivityModel ticketActivityModel, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PutAsJsonAsync("ticket-activity", ticketActivityModel);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModel = await response.Content.ReadAsAsync<TicketActivityModel>();
                }
                return ticketActivityModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketActivityModel>> GetTicketActivityWithoutTicket(FilterParameter filterParameter, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-activity/others", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModels = await response.Content.ReadAsAsync<List<TicketActivityModel>>();
                }
                return ticketActivityModels;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TicketActivityModel> SaveTicketActivityWithoutTicket(TicketActivityModel ticketActivityModel, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-activity/save-others", ticketActivityModel);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModel = await response.Content.ReadAsAsync<TicketActivityModel>();
                }
                return ticketActivityModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TicketActivityModel> UpdateTicketActivityWithoutTicket(TicketActivityModel ticketActivityModel, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PutAsJsonAsync("ticket-activity/update-others", ticketActivityModel);
                if (response.IsSuccessStatusCode)
                {
                    ticketActivityModel = await response.Content.ReadAsAsync<TicketActivityModel>();
                }
                return ticketActivityModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}