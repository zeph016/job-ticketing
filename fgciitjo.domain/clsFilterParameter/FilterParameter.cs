using fgciitjo.domain.clsEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsFilterParameter
{
    public class FilterParameter
    {
        public FilterParameter()
        {
            DateFrom = Convert.ToDateTime("2018-03-29");
            DateTo = Convert.ToDateTime("2018-03-29");
            ActivityDateFrom = Convert.ToDateTime("2018-03-29");
            ActivityDateTo = Convert.ToDateTime("2018-03-29");
            TicketCategoryTypeId = "0";
            TicketAccessLevel = (Enums.AccessLevel)1;
            TicketUserAccountId = 0;
            RequestorId = 0;
            ReportHeader = "";
        }
        public bool isActive {get; set;}
        public bool IsName { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; } = "FGCITicketing";
        public bool IsDepartment { get; set; }
        public string DepartmentId { get; set; }
        public bool IsTicketStatus { get; set; }
        public string TicketStatusId { get; set; }
        public bool IsTicketBranch { get; set; }
        public string TicketBranchId { get; set; }
        public bool IsDate { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool IsRequestor { get; set; }
        public Int64? RequestorId { get; set; }
        public bool IsLookUp { get; set; }
        public bool IsTicketCategory { get; set; }
        public string TicketCategory { get; set; }
        public bool IsAssignee { get; set; }
        public string AssigneeId { get; set; }
        public Enums.AccessLevel TicketAccessLevel { get; set; }
        public Int64 TicketUserAccountId { get; set; }
        public bool IsTicketCategoryType { get; set; }
        public string TicketCategoryTypeId { get; set; }
        public Enums.TicketCategoryType TicketCategoryType { get; set; }
        public string TicketCategoryId { get; set; }
        public string Token { get; set; }
        public bool IsTicketPriorityLevel { get; set; }
        public Enums.PriorityLevel TicketPriorityLevel { get; set; }
        public bool IsActivityDate {get;set;}
        public DateTime? ActivityDateFrom {get;set;}
        public DateTime? ActivityDateTo {get;set;}
        public string ReportHeader { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedByDesignation { get; set; }
        public long EmployeeId { get; set;}
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int OffSet
        {
            get
            {
                return PageSize * PageNo;
            }
        }
        public string SortColumnName { get; set; }
        public string SortDirection { get; set; }
    }
}
