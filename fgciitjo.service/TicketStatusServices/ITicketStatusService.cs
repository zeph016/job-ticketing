using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketStatusServices
{
    public interface ITicketStatusService
    {
        Task<List<TicketStatusModel>> GetTicketStatus(FilterParameter filterParameter, string token);
        Task<TicketStatusModel> AddTicketStatus(TicketStatusModel ticketStatus, string token);
        Task<TicketStatusModel> UpdateTickeStatus(TicketStatusModel ticketStatus, string token);
        Task<TicketStatusModel> GetTicketStatusById(long Id, string token);
    }
}
