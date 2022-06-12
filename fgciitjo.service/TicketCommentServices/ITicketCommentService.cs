using fgciitjo.domain.clsTicketComment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketCommentServices
{
    public interface ITicketCommentService
    {
        Task<List<TicketComment>> LoadTicketComments(TicketComment ticketComment);
    }
}
