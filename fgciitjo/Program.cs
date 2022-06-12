using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Blazored.LocalStorage;
using fgciitjo.service.UserAccountServices;
using fgciitjo.service.EmployeeAccountServices;
using fgciitjo.service.TicketUserAccountServices;
using fgciitjo.service.TicketStatusServices;
using fgciitjo.service.TicketCategoryServices;
using fgciitjo.service.TicketBranchServices;
using fgciitjo.service.TicketServices;
using fgciitjo.service.TicketListServices;
using fgciitjo.service.GlobalServices;
using fgciitjo.service.TicketActivityServices;
using fgciitjo.service.SubTaskServices;
using fgciitjo.service.AuditTrailServices;
using fgciitjo.service.TicketMRItemServices;
using fgciitjo.service.TicketCommentServices;

namespace fgciitjo
{
  public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAPIUrl")) });
            // builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>(apiUrl)) });
            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<IUserAccountService, UserAccountService>();
            builder.Services.AddScoped<ITicketUserAccountService, TicketUserAccountService>();
            builder.Services.AddScoped<ITicketStatusService, TicketStatusService>();
            builder.Services.AddScoped<ITicketCategoryService, TicketCategoryService>();
            builder.Services.AddScoped<ITicketBranchService, TicketBranchService>();
            builder.Services.AddScoped<ITicketBranchService, TicketBranchService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ITicketListService, TicketListService>();
            builder.Services.AddScoped<IGlobalService, GlobalService>();
            builder.Services.AddScoped<ITicketActivityService, TicketActivityService>();
            builder.Services.AddScoped<ISubTaskService, SubTaskService>();
            builder.Services.AddScoped<IAuditTrailService, AuditTrailService>();
            builder.Services.AddScoped<IEmployeeAccountService, EmployeeAccountService>();
            builder.Services.AddScoped<ITicketMRItemService, TicketMRItemService>();
            builder.Services.AddScoped<ITicketCommentService, TicketCommentService>();
            await builder.Build().RunAsync();
        }
    }
}
