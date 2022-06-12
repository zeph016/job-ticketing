using fgciitjo.domain.clsEnums;
using fgciitjo.domain.clsTicketAuditTrail;
using fgciitjo.domain.clsTicketFileAttachment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicket
{
    public class TicketModel : TicketAuditTrail
    {
        public TicketModel()
        {
            Id = 0;
            IsActive = true;
            ControlCount = 0;
            TicketNumber = "";
            TicketDate = DateTime.Now;
            IssueSummary = "";
            TaskDescription = "";
            PriorityLevelId = Enums.PriorityLevel.Low;
            DocumentReference = "";
            TicketCategoryId = 0;
            TicketCategoryName = "";
            TicketBranchId = 0;
            TicketBranchName = "";
            TicketStatusId = 0;
            TicketStatusName = "";
            Remarks = "";
            TicketParentId = 0;
            ReleasedById = 0;
            TicketFileAttachmentModels = new List<TicketFileAttachmentModel>();
        }
        public Int64 Id { get; set; }
        public bool IsActive { get; set; }
        public short ControlCount { get; set; }
        public string TicketNumber { get; set; }
        public DateTime? TicketDate { get; set; }
        public string IssueSummary { get; set; }
        public string TaskDescription { get; set; }
        public Enums.PriorityLevel PriorityLevelId { get; set; }
        public Enums.TicketStatusType StatusTypeId {get; set;}
        public Enums.TicketStatusType TicketStatusTypeId { get; set; }
        public Enums.TicketCategoryType TicketCategoryTypeId { get; set; }
        public string DocumentReference { get; set; }
        public Int64 TicketCategoryId { get; set; }
        public string TicketCategoryName { get; set; }
        public Int64 TicketBranchId { get; set; }
        public string TicketBranchName { get; set; }
        public Int64 TicketStatusId { get; set; }
        public string TicketStatusName { get; set; }
        public Int64 RequestorId { get; set; }
        public string RequestorName { get; set; }
        public string RequestorCompany { get; set; }
        public string RequestorDepartment { get; set; }
        public string RequestorSection { get; set; }
        public string RequestorPosition { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public Int64? AssigneeId { get; set; }
        public string AssigneeName { get; set; }
        public string AssigneeSection { get; set; }
        public string AssigneeDepartment { get; set; }
        public string AssigneeDesignation { get; set; }
        public DateTime? AssignDate { get; set; }
        public string Remarks { get; set; }
        public List<TicketFileAttachmentModel> TicketFileAttachmentModels { get; set; }
        public List<TicketFileAttachmentModel> RemovedFileAttachmentModels { get; set; }
        public DateTime? ReleasedDate { get; set; }
        public Int64? ReleasedById { get; set; }
        public string ReleasedByName { get; set; }
        public int DurationDay { get; set; }
        public int WorkingDay { get; set; }
        public Int64 TicketParentId { get; set; }
        public Int64 TicketChildId { get; set; }
        public Int64 TicketLinkId { get; set; }
        public bool IsTicketLinkActive { get; set; }
        public string Location { get; set; }
        public bool ShowDetails { get; set; }
        public string ContactInformation { get; set; }
    }
}
