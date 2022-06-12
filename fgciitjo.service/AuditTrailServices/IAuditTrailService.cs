using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using fgciitjo.domain.clsTicketAuditTrail;
using fgciitjo.domain.clsFilterParameter;

namespace fgciitjo.service.AuditTrailServices
{
    public interface IAuditTrailService
    {
        Task<List<TicketAuditTrail>> GetAllAuditTrails(FilterParameter filterParameter, string token);
        Task<List<TicketAuditTrail>> GetAuditTrailsByEmployee(FilterParameter filterParameter, string token);
        Task<List<TicketAuditTrail>> GetAuditTrailsByIT(FilterParameter filterParameter, string token);
    }
}