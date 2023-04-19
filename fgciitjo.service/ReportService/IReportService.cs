using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsReport;
using fgciitjo.domain.clsFilterParameter;

namespace fgciitjo.service.ReportService
{
    public interface IReportService
    {
        Task<ReportModel> GetReportContent(string token, FilterParameter filterParameter);
        Task<ReportModel> GetTicketListReportContent(string token, FilterParameter filterParameter);
    }
}