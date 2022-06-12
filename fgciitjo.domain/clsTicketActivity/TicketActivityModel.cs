using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;
using fgciitjo.domain.clsTicketAuditTrail;

namespace fgciitjo.domain.clsTicketActivity
{
    public class TicketActivityModel : TicketAuditTrail
    {
        public TicketActivityModel()
        {
            TicketId = null;
        }
        public Int64 Id { get; set; }
        public Int64? TicketId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ActivityName { get; set; }
        public Int64 StatusId { get; set; }
        public string StatusName { get; set; }
        public Int64 UserId { get; set; }
        public string UserName { get; set; }
        public string Remarks { get; set; }
        public bool IsActive {get; set;}
        public Enums.TicketStatusType StatusTypeId { get; set; }
    }
}