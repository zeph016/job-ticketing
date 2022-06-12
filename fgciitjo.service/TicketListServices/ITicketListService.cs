using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicket;
using fgciitjo.domain.clsTicketAuditTrail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketListServices
{
    public interface ITicketListService
    {
        Task<List<TicketModel>> LoadTicket(FilterParameter filterParameter, string token);
        Task<List<TicketAuditTrail>> LoadTicketAuditTrail(Int64 Id, string token);        
        Task<List<TicketModel>> LoadTicketv2(FilterParameter filterParameter, string token);
    }
}
