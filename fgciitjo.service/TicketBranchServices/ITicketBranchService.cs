using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketBranch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketBranchServices
{
    public interface ITicketBranchService
    {
        Task<List<TicketBranchModel>> GetBranch(FilterParameter filterParameter , string token);
        Task<TicketBranchModel> AddBranch(TicketBranchModel ticketBranch, string token);
        Task<TicketBranchModel> UpdateBranch(TicketBranchModel ticket, string token);
    }
}
