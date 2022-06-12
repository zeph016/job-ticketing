using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;

namespace fgciitjo.domain.clsTicketMRItem
{
    public class TicketMRItemModel
    {
      public TicketMRItemModel()
      {
        Id = 0;
      }

        public Int64 Id { get; set; }
        public bool IsActive { get; set; }
        public Int64 TicketActivityId { get; set; }
        public Int64 MRItemdDetailId { get; set; }
        public string MRItemName { get; set; }
        public string MRAssetTag { get; set; }
        public string MREmployeeName { get; set; }
        public Int64 MRProjectId { get; set; }
        public Int64 MRProjectDepartmentId { get; set; }
        public Int64 MRProjectOthersId { get; set; }
        public string MRProject { get; set; }
        public Int64 MREmployeeId { get; set; }
        public string Remarks { get; set; }
        public bool IsSelected {get; set;}
        public Enums.ProjectCategory MRProjectCategoryId { get; set; }
 
    }
}
