using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicketStatus
{
    public class TicketStatusCBModel
    {
        public long Id { get; set; }
        public bool IsChecked { get; set; }
        public TicketStatusModel selectedTicketStatus = new TicketStatusModel();
        public double[] MonthCount { get; set; }
    }
}
