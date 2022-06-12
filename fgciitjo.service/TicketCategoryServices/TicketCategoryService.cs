using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketCategoryServices
{
    public class TicketCategoryService : ITicketCategoryService
    {
        List<TicketCategoryModel> ticketCategoryModelList;
        TicketCategoryModel ticketCategoryModel;
        HttpClient client;
        public TicketCategoryService(HttpClient client_)
        {
            ticketCategoryModelList = new List<TicketCategoryModel>();
            ticketCategoryModel = new TicketCategoryModel();
            client = client_;
        }
        public async Task<TicketCategoryModel> AddTicketCategory(TicketCategoryModel ticketCategory, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-category", ticketCategory);
                if (response.IsSuccessStatusCode)
                {
                    ticketCategoryModel = await response.Content.ReadAsAsync<TicketCategoryModel>();
                }
                return ticketCategoryModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketCategoryModel>> LoadTicketCategory(string token)
        {
            try
            {
                FilterParameter filterParameter = new FilterParameter();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-category/list", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketCategoryModelList = await response.Content.ReadAsAsync<List<TicketCategoryModel>>();
                }
                return ticketCategoryModelList.OrderBy(x => x.CategoryName).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<TicketCategoryModel> UpdateTicketCategory(TicketCategoryModel ticketCategory, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PutAsJsonAsync("ticket-category", ticketCategory);
                if (response.IsSuccessStatusCode)
                {
                    ticketCategoryModel = await response.Content.ReadAsAsync<TicketCategoryModel>();
                }
                return ticketCategoryModel;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<TicketCategoryModel>> LoadTicketCategoryByTypeId(FilterParameter filterParameter, string token)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.PostAsJsonAsync("ticket-category/list", filterParameter);
                if (response.IsSuccessStatusCode)
                {
                    ticketCategoryModelList = await response.Content.ReadAsAsync<List<TicketCategoryModel>>();
                }
                return ticketCategoryModelList.OrderBy(x => x.CategoryName).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
