using fgciitjo.domain.clsTicketComment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketCommentServices
{
    public class TicketCommentService : ITicketCommentService
    {
        public TicketCommentService(HttpClient client)
        {
            this.client = client;
        }
        private readonly HttpClient client;
        public async Task<List<TicketComment>> LoadTicketComments(TicketComment ticketComment)
        {
            var ticketComments = new List<TicketComment>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ticketComment.Token);
            HttpResponseMessage response = await client.GetAsync("/ticket-comment/list/" + ticketComment.TicketId);
            if (response.IsSuccessStatusCode)
                ticketComments = await response.Content.ReadAsAsync<List<TicketComment>>();

            return ticketComments;

        }
    }
}
