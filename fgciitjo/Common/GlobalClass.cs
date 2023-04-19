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
using fgciitjo.domain.clsTicketFileAttachment;

namespace fgciitjo.Common
{
    public static class GlobalClass
    {
        public static string Token { get; set; } = string.Empty;
        public static Task<DateTime> ServerTime { get; set; } = default!;
        public static FilterParameter filterParameter { get; set; } = new FilterParameter();
        public static int PageSize { get; set; }
        public static int PageNo = 1;
        public static bool isFromFilter { get; set; }
        public static bool AccountAuthorized { get; set; }
        public static UserAccount CurrentUserAccount { get; set; } = default!;
        public static TicketModel ticket { get; set; } = new TicketModel();
    }

    public static class GlobalContentTitle
    {
        public static string contentTitle { get; set; } = default!;
        public static DateTime contentServerTime { get; set; }
    }

    public static class GlobalList
    {
        public static List<TicketModel> Tickets { get; set; } = default!;
        public static List<TicketModel> SubTickets { get; set; } = default!;
        public static List<TicketComment> TicketComments { get; set; } = default!;
        public static List<Employee> ITDept { get; set; } = default!;
        public static List<Enums.TicketCategoryType> TicketCategoryTypeList = new List<Enums.TicketCategoryType>();
        public static List<TicketCategoryModel> ticketCategoryList { get; set; } = default!;
        public static List<TicketBranchModel> TicketBranchList { get; set; } = default!;
        public static List<Department> DepartmentList { get; set; } = default!;
        public static List<TicketStatusModel> TicketStatusList { get; set; } = default!;
        public static List<TicketFileAttachmentModel> ticketFileImages { get; set; } = new List<TicketFileAttachmentModel>();
        public static List<TicketModel> SubTaskList { get; set; } = new List<TicketModel>();
        public static List<UserAccount> TemporaryEmployeeList { get; set; } = new List<UserAccount>();
    }

    public static class GlobalVariable
    {
        public static int[] pageSize = new int[] { 15, 30, 45, 60, 75, 90, 100 };
    }
}
