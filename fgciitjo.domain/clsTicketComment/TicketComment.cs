using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicketComment
{
    public class TicketComment
    {
        public Int64 Id { get; set; }
        public Int64 TicketId { get; set; }
        public DateTime DateTimeLog { get; set; }
        public string Comment { get; set; }
        public Int64 UserId { get; set; }
        public string EmployeeName { get; set; }
        public byte[] Picture { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
    }
}
