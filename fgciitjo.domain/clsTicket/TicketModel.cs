using fgciitjo.domain.clsEnums;
using fgciitjo.domain.clsTicketAuditTrail;
using fgciitjo.domain.clsTicketFileAttachment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

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
            TicketBranchId = 0  ;
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
        public string TicketNumber { get; set; } = string.Empty;
        public DateTime? TicketDate { get; set; }
        [Required(ErrorMessage = "Issue summary is required.")]
        [MinLength(5 , ErrorMessage = "Issue summary is too short.")]
        public string IssueSummary { get; set; }
        [Required(ErrorMessage = "Task description is required")]
        [MinLength(15, ErrorMessage = "Task description is too short.")]
        public string TaskDescription { get; set; }
        public Enums.PriorityLevel PriorityLevelId { get; set; }
        public Enums.TicketStatusType StatusTypeId {get; set;}
        public Enums.TicketStatusType TicketStatusTypeId { get; set; }
        [Required(ErrorMessage = "A category type is required.")]
        public Enums.TicketCategoryType TicketCategoryTypeId { get; set; }
        public string DocumentReference { get; set; }
        [Required(ErrorMessage = "An issue category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select an issue category.")]
        public Int64 TicketCategoryId { get; set; }
        // [MinLength(3 , ErrorMessage = "Select a category for your issue.")]
        public string TicketCategoryName { get; set; }
        [Required(ErrorMessage = "Branch location is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a branch.")]
        public Int64 TicketBranchId { get; set; }
        [Required(ErrorMessage = "Branch location is required")]
        [MinLength(2, ErrorMessage = "Branch name is too short.")]
        public string TicketBranchName { get; set; } = string.Empty;
        public Int64 TicketStatusId { get; set; }
        public string TicketStatusName { get; set; }
        [Required(ErrorMessage = "Requestor is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select a requestor.")]
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
        public List<TicketFileAttachmentModel> TicketFileAttachmentModels { get; set; } = new List<TicketFileAttachmentModel>();
        public List<TicketFileAttachmentModel> RemovedFileAttachmentModels { get; set; } = new List<TicketFileAttachmentModel>();
        public DateTime? ReleasedDate { get; set; }
        public Int64? ReleasedById { get; set; }
        public string ReleasedByName { get; set; }
        public int DurationDay { get; set; }
        public int WorkingDay { get; set; }
        public Int64 TicketParentId { get; set; }
        public Int64 TicketChildId { get; set; }
        public Int64 TicketLinkId { get; set; }
        public bool IsTicketLinkActive { get; set; }
        [Required(ErrorMessage = "Location is required")]
        [MinLength(2, ErrorMessage = "Location description is too short.")]
        public string Location { get; set; } = string.Empty;
        public bool ShowDetails { get; set; }
        public string ContactInformation { get; set; } = string.Empty;
    }
}
