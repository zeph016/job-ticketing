using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;
using fgciitjo.domain.clsTicketAuditTrail;
using System.ComponentModel.DataAnnotations;

namespace fgciitjo.domain.clsTicketActivity
{
    public class TicketActivityModel : TicketAuditTrail
    {
        public TicketActivityModel()
        {
            TicketId = null;
        }
        public Int64 Id { get; set; }
        [Required(ErrorMessage = "Ticket reference missing.")]
        public Int64? TicketId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Activity status is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a status.")]
        public Int64 StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Int64 UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool IsActive {get; set;}
        public Enums.TicketStatusType StatusTypeId { get; set; }
    }
}