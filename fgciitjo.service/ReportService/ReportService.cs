using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using fgciitjo.domain.clsReport;
using fgciitjo.domain.clsFilterParameter;
using Microsoft.Extensions.Configuration;

namespace fgciitjo.service.ReportService
{
    public class ReportService : IReportService
    {
        private HttpClient client;
        private IConfiguration configuration;
        public ReportService(HttpClient _client, IConfiguration _configuration) 
        {
            client = _client;
            configuration = _configuration;
        } 
     

        public async Task<ReportModel> GetReportContent(string token, FilterParameter filterParameter)
        {
            ReportModel reportModel = new ReportModel();
            // HttpResponseMessage result = await client.PostAsJsonAsync("http://fgciservert140:9000/api/getdailyaccomplishmentreport", filterParameter);
            HttpResponseMessage result = await client.PostAsJsonAsync(configuration["ReportServer"] + "api/getdailyaccomplishmentreport", filterParameter);
            reportModel.ReportContent = "data:application/pdf;base64," + Convert.ToBase64String(await result.Content.ReadAsByteArrayAsync());
            return reportModel;
        }
        public async Task<ReportModel> GetTicketListReportContent(string token, FilterParameter filterParameter)
        {
            ReportModel reportModel = new ReportModel();
            // HttpResponseMessage result = await client.PostAsJsonAsync("http://fgciservert140:9000/api/getticketlistreport", filterParameter);
            HttpResponseMessage result = await client.PostAsJsonAsync(configuration["ReportServer"] + "api/getticketlistreport", filterParameter);
            reportModel.ReportContent = "data:application/pdf;base64," + Convert.ToBase64String(await result.Content.ReadAsByteArrayAsync());
            return reportModel;
        }
    }
}