using fgciitjo.domain.clsTicket;
using fgciitjo.domain.clsUserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketServices
{
    public interface ITicketService
    {
       
        Task<TicketModel> SaveTicket(TicketModel ticketModel, string token);
        Task<TicketModel> AssignTicket(TicketModel ticketModel, string token);
        Task<TicketModel> UpdateTicketStatus(TicketModel ticketModel, string token);
        Task<TicketModel> GetTicketById(Int64 Id, string token);
        Task<TicketModel> UpdateTicket(TicketModel ticketModel, string token);
        Task<TicketModel> CancelTicket(TicketModel ticketModel, string token);
        Task<TicketModel> GetLatestTicket(string token);
    }
}
