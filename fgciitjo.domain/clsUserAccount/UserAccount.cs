using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsEnums;

namespace fgciitjo.domain.clsUserAccount
{
    public class UserAccount
    {
        public UserAccount()
        {
            SystemName = "FGCITicketing";
        }
        public UserAccount(UserAccount userAccount)
        {
            EmployeeId = userAccount.EmployeeId;
            FirstName = userAccount.FirstName;
            MiddleName = userAccount.MiddleName;
            LastName = userAccount.LastName;
            NameExtension = userAccount.NameExtension;
            Token = userAccount.Token;
        }
        public Int64 EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string NameExtension { get; set; }
        public string Designation { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public string IdNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string CompanyName { get; set; }
        public string StatusName { get; set; }
        public DateTime DateHired { get; set; }
        public string Address { get; set; }
        public byte[] Picture { get; set; }
        public string EmployeeName
        {
            get
            {
                var _employeename = "";
                if (!string.IsNullOrEmpty(NameExtension))
                    _employeename = FirstName + " " + LastName + " " + NameExtension;
                else
                    _employeename = FirstName + " " + LastName;
                return _employeename;
            }
        }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SystemName { get; set; }
        public string Token { get; set; }
        public Enums.AccessLevel AccessLevel { get; set; }     
    }
}
