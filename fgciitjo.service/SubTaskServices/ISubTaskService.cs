using fgciitjo.domain.clsTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.SubTaskServices
{
    public interface ISubTaskService
    {
        Task<List<TicketModel>> SubTaskList(Int64 ticketId, string token);
        Task<TicketModel> DeactivateSubTask(TicketModel ticketModel, string token);
    }
}
