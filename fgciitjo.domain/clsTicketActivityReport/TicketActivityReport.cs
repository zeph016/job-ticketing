using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;
using fgciitjo.domain.clsTicket;

namespace fgciitjo.domain.clsTicketActivityReport
{
    public class TicketActivityReportModel : TicketModel
    {
        public Int64 Id { get; set; }
        public Int64 TicketId { get; set; }
        public DateTime ActivityDate { get; set; }
        public string ActivityName { get; set; }
        public Int64 StatusId { get; set; }
        public string StatusName { get; set; }
        public Enums.TicketStatusType StatusTypeId { get; set; }
        public Int64 UserId { get; set; }
        public string UserName { get; set; }
        public string UserNameDepartment { get; set; }
        public string UserNameSection { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}