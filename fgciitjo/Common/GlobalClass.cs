using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fgciitjo.domain.clsTicket;
using fgciitjo.domain.clsUserAccount;
using fgciitjo.domain.clsTicketComment;
using fgciitjo.domain.clsEmployee;
using fgciitjo.domain.clsEnums;
using fgciitjo.domain.clsTicketCategory;
using fgciitjo.domain.clsTicketBranch;
using fgciitjo.domain.clsDepartment;
using fgciitjo.domain.clsTicketStatus;
using fgciitjo.domain.clsFilterParameter;

namespace fgciitjo.Common
{
    public static class GlobalClass
    {
        public static UserAccount CurrentUserAccount { get; set; } = default!;
        public static Task<DateTime> ServerTime { get; set; } = default!;
        public static List<UserAccount> DepartmentList { get; set; } = default!;
        public static List<TicketModel> Tickets { get; set; } = default!;
        public static List<TicketComment> TicketComments { get; set; } = default!;
        public static FilterParameter filterParameter { get; set; } = new FilterParameter();
        public static int PageSize { get; set; }
        public static int PageNo = 1;
        public static string token { get; set; } = default!;
        public static bool isFromFilter { get; set; } = default!;
    }

    public static class GlobalContentTitle
    {
        public static string contentTitle { get; set; } = default!;
        public static DateTime contentServerTime { get; set; }
    }

    public static class GlobalList
    {
        public static List<Employee> ITDept { get; set; } = default!;
        public static List<TicketCategoryModel> ticketCategoryList { get; set; } = default!;
        public static List<TicketBranchModel> ticketBranchList { get; set; } = default!;
        public static List<Department> departmentList { get; set; } = default!;
        public static List<TicketStatusModel> ticketStatusList { get; set; } = default!;
    }
}
