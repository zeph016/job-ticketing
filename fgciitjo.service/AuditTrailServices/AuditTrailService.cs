using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using fgciitjo.domain.clsUserAccount;
using fgciitjo.domain.clsDepartment;
using fgciitjo.domain.clsTicketAuditTrail;
using fgciitjo.domain.clsFilterParameter;

namespace fgciitjo.service.AuditTrailServices
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly HttpClient _httpClient;

        public AuditTrailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            auditTrails = new List<TicketAuditTrail>();
        }

        List<TicketAuditTrail> auditTrails;

        public async Task<List<TicketAuditTrail>> GetAllAuditTrails(FilterParameter filterParameter, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("ticket-audittrail/list", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    auditTrails = await response.Content.ReadAsAsync<List<TicketAuditTrail>>();
                }
                // return ticketModels.Where(x => x.TicketStatusTypeId != domain.clsEnums.Enums.TicketStatusType.Complete).ToList();
                return auditTrails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<TicketAuditTrail>> GetAuditTrailsByEmployee(FilterParameter filterParameter, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("ticket-audittrail/employee", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    auditTrails = await response.Content.ReadAsAsync<List<TicketAuditTrail>>();
                }
                return auditTrails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<TicketAuditTrail>> GetAuditTrailsByIT(FilterParameter filterParameter, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("ticket-audittrail/it", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    auditTrails = await response.Content.ReadAsAsync<List<TicketAuditTrail>>();
                }
                return auditTrails;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}