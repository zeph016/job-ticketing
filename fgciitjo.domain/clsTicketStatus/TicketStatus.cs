using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;

namespace fgciitjo.domain.clsTicketStatus
{
    public class TicketStatusModel
    {
      public TicketStatusModel()
      {
          IsActive = true;
      }
        public Int64 Id { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }
        public bool AllowUpdate { get; set; }
        public Enums.TicketStatusType StatusTypeId { get; set; }
        public string StatusColor { get; set; }
    }
}
