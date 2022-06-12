using fgciitjo.domain.clsTicket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.SubTaskServices
{
    public class SubTaskService : ISubTaskService
    {
        private readonly HttpClient client;
        public SubTaskService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<TicketModel> DeactivateSubTask(TicketModel ticketModel, string token)
        {
            try
            {
                TicketModel model = new TicketModel();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync("ticket-subtask/deactivate", ticketModel);
                if (responseMessage.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());
                }
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketModel>> SubTaskList(long ticketId, string token)
        {
            try
            {
                List<TicketModel> listSubTask = new List<TicketModel>();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.GetAsync("ticket-subtask/list/" + ticketId);
                if (responseMessage.IsSuccessStatusCode)
                {
                    listSubTask = JsonConvert.DeserializeObject<List<TicketModel>>(await responseMessage.Content.ReadAsStringAsync());
                }
                return listSubTask;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
