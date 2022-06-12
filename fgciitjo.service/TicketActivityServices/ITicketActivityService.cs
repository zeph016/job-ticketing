using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketActivity;
using fgciitjo.domain.clsTicketActivityReport;

namespace fgciitjo.service.TicketActivityServices
{
    public interface ITicketActivityService
    {
        Task<List<TicketActivityModel>> GetAllActivityById(Int64 id, string token);
        Task<TicketActivityModel> AddTicketActivity(TicketActivityModel ticketActivityModel, string token);
        Task<TicketActivityModel> UpdateTicketActivity(TicketActivityModel ticketActivityModel, string token);
        Task<TicketActivityModel> GetActivityById(long Id, string token);
        Task<List<TicketActivityReportModel>> GetActivityReport(FilterParameter filterParameter, string token);
        Task<List<TicketActivityModel>> GetTicketActivityWithoutTicket(FilterParameter filterParameter, string token);
        Task<TicketActivityModel> SaveTicketActivityWithoutTicket(TicketActivityModel ticketActivityModel, string token);
        Task<TicketActivityModel> UpdateTicketActivityWithoutTicket(TicketActivityModel ticketActivityModel, string token);
    }
}