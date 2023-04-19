namespace fgciitjo.Pages.Activity
{
    public class TicketActivityBase : ComponentBase
    {
        #region Inject Services
        [Inject] protected IGlobalService GlobalService { get; set; } = default!;
        [Inject] protected ITicketService TicketService { get; set; } = default!;
        [Inject] protected ITicketActivityService TicketActivityService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager  { get; set; } = default!;
        #endregion
        #region Properties
        [Parameter] public long TicketId { get; set; }
        protected MudTable<TicketActivityModel> tableVariable = new();
        protected TicketModel currentTicket = new();
        protected List<TicketActivityModel> ticketActivities = new();
        protected bool dataFetched, readOnly, isTableLoading, filterChipsVisible;
        protected string searchTerm = string.Empty;
        private string searchStatusName = string.Empty;
        protected int chip1,chip2,chip3,chip4,chip5;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Ticket Activity";
            while (GlobalList.TicketStatusList == null 
                && GlobalClass.CurrentUserAccount == null
                && GlobalList.ITDept == null)
                await Task.Delay(1);
            
            Task t = GetTicket();
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
            {
                DisplayStatus();
                CompletedFetch();
            }
        }

        protected void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }
        private async Task GetTicket() => currentTicket = await TicketService.GetTicketById(TicketId, GlobalClass.Token);

        protected void DisplayStatus()
        {
            if (currentTicket.TicketStatusTypeId == Enums.TicketStatusType.Complete)
                readOnly = true;
            else
                readOnly = false;
        }

        protected async Task<TableData<TicketActivityModel>> LoadTicketActivitiesById(TableState tableState)
        {
            isTableLoading = true;
            IEnumerable<TicketActivityModel> data = new List<TicketActivityModel>();
            Task<List<TicketActivityModel>> GetActivityTask = (ticketActivities.Count != 0 ? OrderByLogDatetime() : GetActivities(currentTicket.Id, GlobalClass.Token));
            await GetActivityTask;
            if (GetActivityTask.Status == TaskStatus.RanToCompletion)
                data = GetActivityTask.Result;
            await CountTicketTypes(data);
            Console.WriteLine("Reloaded activity");
            data = data.Where(activity =>
            {
                if (string.IsNullOrWhiteSpace(searchStatusName))
                    return true;
                if (activity.StatusName.Contains(searchStatusName, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            switch (tableState.SortLabel)
            {
                case "SortDate":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.ActivityDate);
                    break;
                case "SortByLog":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.LogDatetime);
                    break;
                case "SortByActivity":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.Activity);
                    break;
                case "SortByRemarks":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.Remarks);
                    break;
            }
            ticketActivities = await Task.Run(() => data.ToList());
            var total = data.Count();
            data = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            isTableLoading = false;
            StateHasChanged();
            return new TableData<TicketActivityModel>()
            {
                TotalItems = total,
                Items = data
            };
        }

        protected async Task CountTicketTypes(IEnumerable<TicketActivityModel> tickets)
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

        protected async Task<List<TicketActivityModel>> GetActivities(long ticketId, string token)
        {
            var result = await TicketActivityService.GetAllActivityById(ticketId, GlobalClass.Token);
            if (result != null)
                return result;
            else 
                return new();
        }

        protected async Task<List<TicketActivityModel>> OrderByLogDatetime()
        {
          
            var result = await Task.Run(() => ticketActivities.OrderByDescending(x=>x.LogDatetime).ToList());
            if (result != null)
                return result;
            else
                return new();
        }
        protected async Task AddTicketActivity(Enums.CrudeMode crudeMode)
        {
            GlobalClass.ticket = currentTicket;
            var parameters = new DialogParameters()
            {
                { "DialogTitle", "Add Ticket Activity" },
                { "IconString", @Icons.Material.Filled.Add },
                { "ButtonText", "Add"},
                { "Color", Color.Success },
                { "TicketId", currentTicket.Id },
                { "TicketNumber", currentTicket.TicketNumber },
                { "TicketDate", currentTicket.TicketDate },
                { "ActionMode", crudeMode } 
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
                await ReloadActivity(false);
            }
        }
        protected async Task UpdateTicketActivity(long Id, Enums.CrudeMode crudeMode)
        {
            GlobalClass.ticket = currentTicket;
            var parameters = new DialogParameters()
            {
                { "DialogTitle", "Modify Ticket Activity" },
                { "IconString", @Icons.Material.Filled.Edit },
                { "ButtonText", "Update"},
                { "Color", Color.Info },
                { "Id", Id },
                { "TicketId", currentTicket.Id },
                { "TicketNumber", currentTicket.TicketNumber },
                { "TicketDate", currentTicket.TicketDate },
                { "ActionMode", crudeMode } 
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
                var index = ticketActivities.FindIndex(x=>x.Id == updatedItem.Id);
                if (index >= 0)
                    ticketActivities[index] = updatedItem;
                await ReloadActivity(false);
            }
        }

        protected async Task AddMRItem(long activityId)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "Ticket MR Assets");
            parameters.Add("ButtonText", "Save");
            parameters.Add("Color", Color.Success);
            parameters.Add("ticketActivityId", activityId);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
            var resultDialog = await DialogService.Show<Shared.Dialogs.TicketMRItemDialog>("Add Assets", parameters,
            options).Result;
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
                TicketActivityModel activityToRemove = new TicketActivityModel
                {
                    Id = Id,
                    Activity = "Remove ticket activity",
                    UserId = GlobalClass.CurrentUserAccount.EmployeeId,
                    ActivityDate = await GetServerTime(),
                    IsActive = false,
                    TicketId = currentTicket.Id,
                    StatusId = statusid,
                    ActivityName = activity,
                    Remarks = remarks
                };
                
                Task t = UpdateTicketActivity(activityToRemove, GlobalClass.Token);
                await t;
                if (t.Status == TaskStatus.RanToCompletion)
                    await ReloadActivity(true);
            }
        }

        protected async Task ReloadActivity(bool isFullReload)
        {
            searchStatusName = string.Empty;
            if (isFullReload)
                ticketActivities = new List<TicketActivityModel>();
            await tableVariable.ReloadServerData();
        }


        protected void Back()
        {
            NavigationManager.NavigateTo($"/ticket/list");
        }

        protected async Task UpdateTicketActivity(TicketActivityModel ticketActivity, string token)
        {
            Task RemoveTask = TicketActivityService.UpdateTicketActivity(ticketActivity, token);
            await RemoveTask;
            if (RemoveTask.Status == TaskStatus.RanToCompletion)
                Extensions.ShowAlert("Removed Ticket Activity.", Variant.Filled, SnackbarService, Severity.Error, "");
            else
                ShowErrorAlert("Code: Remove ticket endpoint.");
        }

        protected async Task<DateTime> GetServerTime()
        {
            GlobalContentTitle.contentServerTime = await GlobalService.LoadServerTime();
            return Convert.ToDateTime(GlobalContentTitle.contentServerTime.ToString("MM/dd/yyyy h:mm:ss tt"));
        }
        protected void ShowErrorAlert(string message) => Extensions.ShowAlert($"An error has occurred, Please contact IT Administrator. {message}", Variant.Filled, SnackbarService, Severity.Error, string.Empty);
        protected async Task FilterTableByChip(string ticketStatusName)
        {
            searchStatusName = ticketStatusName;
            await tableVariable.ReloadServerData();
        }
    }
}