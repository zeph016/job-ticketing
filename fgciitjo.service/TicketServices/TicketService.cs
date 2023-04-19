using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicket;
using fgciitjo.domain.clsUserAccount;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketServices
{
    public class TicketService : ITicketService
    {
        public TicketService(HttpClient _client)
        {
            this.client = _client;
        }

        #region Properties

        HttpClient client;


        #endregion

        #region Methods

        #region Save Ticket
        public async Task<TicketModel> SaveTicket(TicketModel ticketModel, string token)
        {
            TicketModel ticket = new TicketModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync("ticket", ticketModel);
            if (responseMessage.IsSuccessStatusCode)
            {
                ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());
            }
            return ticket;
        }


        #endregion

        #region
        public async Task<TicketModel> AssignTicket(TicketModel ticketModel, string token)
        {
            TicketModel ticket = new TicketModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync("ticket/assign", ticketModel);
            if (responseMessage.IsSuccessStatusCode)
                ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());

            return ticket;
        }

        public async Task<TicketModel> UpdateTicketStatus(TicketModel ticketModel, string token)
        {
          
          try
          {
            TicketModel ticket = new TicketModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
             HttpResponseMessage responseMessage = await client.PutAsJsonAsync("ticket/update-status/", ticketModel);
            if (responseMessage.IsSuccessStatusCode)
              ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());
            else
               throw new ApplicationException($"{"Please add Ticket Activity to update status of this ticket."}");

                return new TicketModel();
          }
          catch (System.Exception ex)
          {
             throw ; //or add a custom logic here
          }
        
        }
        #endregion

        #region Get Ticket by ID
        public async Task<TicketModel> GetTicketById(long Id, string token)
        {
            TicketModel ticket = new TicketModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage = await client.GetAsync("/ticket/" + Id);
            if (responseMessage.IsSuccessStatusCode)
                ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());
            else 
                return null;
            return ticket;
        }
        #endregion

        #region Update Ticket
        public async Task<TicketModel> UpdateTicket(TicketModel ticketModel, string token)
        {
            TicketModel ticket = new TicketModel();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage = await client.PutAsJsonAsync("/ticket" , ticketModel);
            if (responseMessage.IsSuccessStatusCode)
                ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());

            return ticket;
        }

    
    #endregion

    #region Employee Ticket Cancellation

    public async Task<TicketModel> CancelTicket(TicketModel ticketModel, string token)
    {
       TicketModel ticket = new TicketModel();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      HttpResponseMessage responseMessage = await client.PutAsJsonAsync("/ticket/cancel-ticket" , ticketModel);
      if (responseMessage.IsSuccessStatusCode)
        ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());

      return ticket;
    }

   
    #endregion

    #region 
    public async Task<TicketModel> GetLatestTicket( string token)
    {
      TicketModel ticket = new TicketModel();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      HttpResponseMessage responseMessage = await client.GetAsync("/ticket/last-ticket");
      if (responseMessage.IsSuccessStatusCode)
        ticket = JsonConvert.DeserializeObject<TicketModel>(await responseMessage.Content.ReadAsStringAsync());

      return ticket;
    }

    #endregion

    #endregion
  }
    
}
