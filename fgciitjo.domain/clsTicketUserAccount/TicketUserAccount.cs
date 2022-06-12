using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicketUserAccount
{
    public class TicketUserAccount
    {
        public Int64 Id { get; set; }
        public Int64 HubUserAccountId { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Designation { get; set; }
        public string EmployeeStatus { get; set; }
        public string EmailAddress { get; set; }
        public string ContactInfo { get; set; }
    }
}
