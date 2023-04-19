namespace fgciitjo.Pages.Activity
{
    public class OtherActivityBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ITicketActivityService TicketActivityService { get; set; } = default!;
        [Inject] protected IGlobalService GlobalService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region  Properties
        protected bool dataFetched, readOnly, _isPopOverOpen, isTableLoading, filterChipsVisible;
        protected List<TicketActivityModel> ticketActivities = new();
        protected MudTable<TicketActivityModel> tableVariable = new();
        private FilterParameter filterParameter = new();
        protected string searchTerm = string.Empty; 
        private DateRange dateRange = new DateRange(DateTime.Now.AddDays(-5).Date, DateTime.Now.Date);
        protected int chip1,chip2,chip3,chip4,chip5;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Other Activities";
            MapDefaultParams(false);
            await Task.Delay(1);
            CompletedFetch();
        }
        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }
        protected async Task<TableData<TicketActivityModel>> LoadOtherActivities(TableState tableState)
        {
            isTableLoading = true;
            IEnumerable<TicketActivityModel> data = new List<TicketActivityModel>();
            if (ticketActivities.Count != 0)
                data = await Task.Run(() =>  ticketActivities.OrderBy(x=>x.LogDatetime).ToList());
            else
                data = await TicketActivityService.GetTicketActivityWithoutTicket(filterParameter, GlobalClass.Token);
            data = await FilteredList(data);
            await CountTicketTypes(data);
            switch (tableState.SortLabel)
            {
                case "SortDate":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.ActivityDate);
                    break;
            }
            ticketActivities = await Task.Run(() => data.ToList());
            var total = data.Count();
            data = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            if (data.Count() == 0)
            {
                string dateFrom = Convert.ToDateTime(filterParameter.ActivityDateFrom).ToShortDateString();
                string dateTo = Convert.ToDateTime(filterParameter.ActivityDateTo).ToShortDateString();;
                Extensions.ShowAlertV2("No activity found on dates " + dateFrom + " - " + dateTo, Variant.Filled, 
                SnackbarService, Severity.Normal, Icons.Material.Filled.Cancel, Defaults.Classes.Position.TopRight);
            }
            isTableLoading = !isTableLoading;
            StateHasChanged();
            return new TableData<TicketActivityModel>()
            {
                TotalItems = total,
                Items = data
            };
        }
        protected async Task AddTicketActivity(Enums.CrudeMode action)
        {
            var parameters = new DialogParameters()
            {
                { "DialogTitle", "Other Activity" },
                { "IconString", @Icons.Material.Filled.Add },
                { "ButtonText", "Add"},
                { "Color", Color.Success },
                { "IsOtherActivity", true },
                { "TicketDate", DateTime.Now.Date },
                { "ActionMode", Enums.CrudeMode.Add } 
            };
            var options = new DialogOptions()
            {
                CloseButton = false,
                MaxWidth = MaxWidth.Large,
                FullWidth = false,
                NoHeader = true,
                DisableBackdropClick = false
            };
            var resultDialog = await DialogService.Show<Shared.Dialogs.TicketActivity.TicketActivityDialog>(string.Empty, parameters, options).Result;
            if (!resultDialog.Canceled)
            {
                ticketActivities.Add((TicketActivityModel)resultDialog.Data);
                await tableVariable.ReloadServerData();
            }
        }
        protected async Task UpdateTicketActivity(long Id)
        {
            var parameters = new DialogParameters()
            {
                { "DialogTitle", "Modify Ticket Activity" },
                { "IconString", @Icons.Material.Filled.Edit },
                { "ButtonText", "Update"},
                { "Color", Color.Info },
                { "Id", Id },
                { "IsOtherActivity", true },
                { "OtherActivityId", Id },
                { "TicketDate", DateTime.Now.Date },
                { "ActionMode", Enums.CrudeMode.Edit } 
            };
            var options = new DialogOptions()
            {
                CloseButton = false,
                MaxWidth = MaxWidth.Large,
                FullWidth = false,
                NoHeader = true,
                DisableBackdropClick = false
            };
            var resultDialog = await DialogService.Show<Shared.Dialogs.TicketActivity.TicketActivityDialog>(string.Empty, parameters, options).Result;
            if (!resultDialog.Canceled)
            {
                TicketActivityModel updatedItem = (TicketActivityModel)resultDialog.Data;
                var index = ticketActivities.FindIndex(x =>x.Id == updatedItem.Id);
                if (index >= 0)
                    ticketActivities[index] = updatedItem;
                await tableVariable.ReloadServerData();
            }
        
        }
        protected async Task RemoveTicketActivity(long Id, long statusid, string activity, string remarks)
        {
            var parameters = new DialogParameters();
            string dialogTitle = "Remove";
            parameters.Add("DialogTitle", dialogTitle);
            parameters.Add("ContentText", "remove activity");
            parameters.Add("actionMode", Enums.CrudeMode.Cancel);
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var resultDialog = await DialogService.Show<Shared.Dialogs.GenericDialogs.GenericDialog>(dialogTitle, parameters,
            options).Result;
            if (!resultDialog.Canceled)
            {
                TicketActivityModel activityModel = new()
                {
                    Id = Id,
                    UserId = GlobalClass.CurrentUserAccount.EmployeeId,
                    ActivityDate = await GetServerTime(),
                    IsActive = false,
                    TicketId = null,
                    StatusId = statusid,
                    ActivityName = activity,
                    Remarks = remarks
                };
                await TicketActivityService.UpdateTicketActivityWithoutTicket(activityModel, GlobalClass.Token);
                Extensions.ShowAlert("Removed tickety activity.", Variant.Filled, SnackbarService, Severity.Normal, Icons.Material.Filled.Cancel);
                await ReloadActivity(false);
            }
        }

        protected async Task ReloadActivity(bool isFromFilter)
        {
            _isPopOverOpen = false;
            ticketActivities = new List<TicketActivityModel>();
            MapDefaultParams(isFromFilter);
            await tableVariable.ReloadServerData();
        }

        protected async Task<DateTime> GetServerTime()
        {
            var serverTime = await GlobalService.LoadServerTime();
            if (serverTime != null)
                return Convert.ToDateTime(serverTime.ToString("MM/dd/yyyy h:mm:ss tt"));
            else
                return DateTime.Now;
        }
        private void MapDefaultParams(bool isFromFilter)
        {
            filterParameter.IsActivityDate = true;
            filterParameter.TicketAccessLevel = 0;
            if (isFromFilter)
            {
                filterParameter.IsActivityDate = GlobalClass.filterParameter.IsActivityDate;
                filterParameter.ActivityDateFrom = GlobalClass.filterParameter.ActivityDateFrom;
                filterParameter.ActivityDateTo = GlobalClass.filterParameter.ActivityDateTo;
            }
            else
            {
                filterParameter.ActivityDateFrom = dateRange.Start;
                filterParameter.ActivityDateTo = dateRange.End;
            }
        }
        private async Task<IEnumerable<TicketActivityModel>> FilteredList(IEnumerable<TicketActivityModel> data)
        {
            data = await Task.Run(() =>  data.Where(x=>x.UserId == GlobalClass.CurrentUserAccount.EmployeeId));
            return data;
        }

        protected void CloseFilterPopOver() =>  _isPopOverOpen = false;
        private async Task CountTicketTypes(IEnumerable<TicketActivityModel> tickets)
        {
            await Task.Run(() => 
            {
                chip1 = tickets.Where(x=>x.StatusId == 1).ToList().Count();
                chip2 = tickets.Where(x=>x.StatusId == 2).ToList().Count();
                chip3 = tickets.Where(x=>x.StatusId == 8).ToList().Count();
                chip4 = tickets.Where(x=>x.StatusId == 5).ToList().Count();
                chip5 = tickets.Where(x=>x.StatusId == 6).ToList().Count();
            });
            StateHasChanged();
        }
        protected async Task FilterTableByChip(string ticketStatusName)
        {
            searchTerm = ticketStatusName;
            await tableVariable.ReloadServerData();
        }
    }
}