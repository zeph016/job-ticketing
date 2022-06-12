using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fgciitjo.domain.clsFilterParameter;
using fgciitjo.domain.clsTicketMRItem;

namespace fgciitjo.service.TicketMRItemServices
{
   public interface ITicketMRItemService
   {
     Task<List<TicketMRItemModel>> LoadMRItem(FilterParameter filterParameter, string token);
     Task<TicketMRItemModel> SaveMRItem(TicketMRItemModel ticketMRItemModel, string token);
     Task<TicketMRItemModel> UpdateMRItem(TicketMRItemModel ticketMRItemModel, string token);
     Task<List<TicketMRItemModel>> GetCurrentMRItem(long activityId , string token);
   }
}