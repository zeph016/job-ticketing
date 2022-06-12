using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketUserAccount;

namespace fgciitjo.service.TicketUserAccountServices
{
    public interface ITicketUserAccountService
    {
        Task<List<TicketUserAccount>> LoadTicketUserAccount(FilterParameter filterParameter, string token);
        Task<TicketUserAccount> UpdateTicketUserAccount(TicketUserAccount ticketUserAccount, string token);
        Task<TicketUserAccount> AddTicketUserAccount(TicketUserAccount ticketUserAccount, string token);
        Task<TicketUserAccount> GetOneTicketUserAccount(Int64 HubUserId, string token);
    }
}
