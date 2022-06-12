using fgciitjo.domain.clsTicketCategory;
using fgciitjo.domain.clsFilterParameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fgciitjo.service.TicketCategoryServices
{
    public interface ITicketCategoryService
    {
        Task<List<TicketCategoryModel>> LoadTicketCategory(string token);
        Task<TicketCategoryModel> AddTicketCategory(TicketCategoryModel ticketCategory, string token);
        Task<TicketCategoryModel> UpdateTicketCategory(TicketCategoryModel ticketCategory, string token);
        Task<List<TicketCategoryModel>> LoadTicketCategoryByTypeId(FilterParameter filterParameter, string token);
    }
}
