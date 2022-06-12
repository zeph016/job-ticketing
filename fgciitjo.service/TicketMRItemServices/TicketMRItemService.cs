using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketMRItem;
using Newtonsoft.Json;

namespace fgciitjo.service.TicketMRItemServices
{
  public class TicketMRItemService : ITicketMRItemService
  {
    public TicketMRItemService(HttpClient client)
    {
      this.client = client;
    }

    #region Properties
    HttpClient client;

    #endregion

    #region Methods

    #region Load MR Item
    public async Task<List<TicketMRItemModel>> LoadMRItem(FilterParameter filterParameter, string token)
    {
      List<TicketMRItemModel> mrList = new List<TicketMRItemModel>();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      HttpResponseMessage responseMessage = await client.PostAsJsonAsync("mrledger/mr-items", filterParameter);
      if(responseMessage.IsSuccessStatusCode)
          mrList = JsonConvert.DeserializeObject<List<TicketMRItemModel>>(await responseMessage.Content.ReadAsStringAsync());

      return mrList;
    }

    #endregion

    #region Save MR Item
    public async Task<TicketMRItemModel> SaveMRItem(TicketMRItemModel ticketMRItemModel, string token)
    {
      TicketMRItemModel mrItem = new TicketMRItemModel();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      HttpResponseMessage responseMessage  = await client.PostAsJsonAsync("/ticket-mritem", ticketMRItemModel);
      if(responseMessage.IsSuccessStatusCode)
          mrItem = JsonConvert.DeserializeObject<TicketMRItemModel>(await responseMessage.Content.ReadAsStringAsync());
          return mrItem;
    }

    #endregion

    #region Get Current MR Item
     public async Task<List<TicketMRItemModel>> GetCurrentMRItem(long activityId, string token)
    {
      List<TicketMRItemModel> currentListMR = new List<TicketMRItemModel>();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      HttpResponseMessage responseMessage = await client.GetAsync("ticket-mritem/list/" + activityId);
      if(responseMessage.IsSuccessStatusCode)
          currentListMR = JsonConvert.DeserializeObject<List<TicketMRItemModel>>(await responseMessage.Content.ReadAsStringAsync());

          return currentListMR;
    }

    #endregion

    #region Update MR Item
    public async Task<TicketMRItemModel> UpdateMRItem(TicketMRItemModel ticketMRItemModel, string token)
    {
      TicketMRItemModel ticketMRItem = new TicketMRItemModel();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      HttpResponseMessage responseMessage = await client.PutAsJsonAsync("/ticket-mritem", ticketMRItemModel);
      if(responseMessage.IsSuccessStatusCode)
         ticketMRItem = JsonConvert.DeserializeObject<TicketMRItemModel>(await responseMessage.Content.ReadAsStringAsync());
      else
        throw new ApplicationException($"Error! Please ask Administrator for assistance, Thank you");
     

      return ticketMRItem;
    }

    #endregion

    #endregion

  }
}