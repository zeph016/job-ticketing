using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsEnums
{
    public class Enums
    {
        public enum CrudeMode : byte
        {
            [Description("Add")]
            Add = 0,
            [Description("Edit")]
            Edit = 1,
            [Description("Delete")]
            Delete = 2,
            [Description("Voided")]
            Void = 3
        }

        public enum UserType : byte
        {
            [Description("IT")]
            IT = 0,
            [Description("Client")]
            Client = 1,
        }

        public enum PriorityLevel
        {
            [Description("Low")]
            Low = 0,
            [Description("Medium")]
            Medium = 1,
            [Description("High")]
            High = 2,
        }

         public enum AccessLevel : byte
        {
            [Description("Administrator")]
            Administrator = 0,
            [Description("IT")]
            IT = 1,
            [Description("Employee")]
            Client = 2,
        }
        public enum TicketCategoryType : byte
        {
            [Description("Hardware")]
            Hardware = 0,
            [Description("Software")]
            Software = 1,
            [Description("Admin")]
            Admin = 2,
        }
        public enum TicketStatusType : byte
        {
            [Description("None")]
            None = 0,
            [Description("Allow Update")]
            AllowUpdate = 1,
            [Description("Complete")]
            Complete = 2,
            [Description("Assign")]
            Assign = 3
        }
        public enum ProjectCategory
        {
            Project = 0,
            OtherDepartment = 1,
            OtherProject = 2,
            Equipment = 3,
            OtherEquipment = 4,
            Employee = 5,
            NonEmployee = 6,
            Section = 7,
            Company = 8,
            PrivateCustomer = 9,
            Product = 10,
            EquipmentNonEmployee = 11,
            Quarry = 12,
            FGDepartment = 13,
            EquipmentPPEType = 14,
            None = 15
        }

        public enum DialogMode
        {
            Inform = 0,
            Warning = 1,
            Error = 2,
            Success = 3
        }
    }
}
