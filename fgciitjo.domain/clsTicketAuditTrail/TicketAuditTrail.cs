using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicketAuditTrail
{
    public class TicketAuditTrail
    {
        public TicketAuditTrail()
        {
            LogDatetime = DateTime.Now;
        }
        public Int64 Id { get; set; }
        public Int64 TicketId { get; set; }
        public Int64 UserId { get; set; }
        public string TicketNumber { get; set; }
        public string UserAccountName { get; set; }
        public DateTime LogDatetime { get; set; }
        public string PCName { get; set; }
        public string Activity { get; set; }
    }
}
