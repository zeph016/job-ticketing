using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicketBranch
{
    public class TicketBranchModel
    {
      public TicketBranchModel()
      {
          IsActive = true;
      }
        public Int64 Id { get; set; }
        public string BranchName { get; set; }
        public bool IsActive { get; set; }
    }
}
