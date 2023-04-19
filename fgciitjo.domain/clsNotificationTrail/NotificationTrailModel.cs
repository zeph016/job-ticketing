using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsNotificationTrail
{
    public class NotificationTrailModel
    {
        public Int64 Id { get; set; }
        public Int64 EmployeeId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string UserAccountName { get; set; } = string.Empty;
        public DateTime LogDateTime { get; set; }
        public string Activity { get; set; } = string.Empty;
        public string TimeMoment { get; set; } = string.Empty;
        public bool isRead { get; set; }
        public Int64 TicketId { get; set;}
    }
}