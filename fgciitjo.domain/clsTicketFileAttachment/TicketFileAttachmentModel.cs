using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.domain.clsTicketFileAttachment
{
    public class TicketFileAttachmentModel
    {
        public Int64 Id { get; set; }
        public Int64 TicketId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] FileAttachment { get; set; } = new byte[]{};
        public Int64 UserAccountId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
