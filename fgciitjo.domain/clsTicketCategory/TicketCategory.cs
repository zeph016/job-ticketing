using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;

namespace fgciitjo.domain.clsTicketCategory
{
    public class TicketCategoryModel
    {
        public Int64 Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public Enums.TicketCategoryType CategoryTypeId { get; set; }
    }
}
