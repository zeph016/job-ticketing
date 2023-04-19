namespace fgciitjo.Pages.SubTask
{
    public class SubTaskBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ITicketService TicketService { get; set; } = default!;
        [Inject] protected ISubTaskService SubTaskService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        #endregion
        #region Properties
        [Parameter] public long TicketId { get; set ; }
        protected List<TicketModel> listOfSubTask = new  List<TicketModel>();
        protected MudTable<TicketModel> tableVariable = new MudTable<TicketModel>();
        protected TicketModel currTicket { get; set; } = new TicketModel();
        protected bool dataFetched, readOnly, isTableLoading, filterChipsVisible;
        private string searchStatusName = string.Empty;
        protected int chip1,chip2,chip3,chip4,chip5;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Sub Tasks";
            while (GlobalList.TicketStatusList == null && GlobalClass.CurrentUserAccount == null && GlobalList.ITDept == null)
                await Task.Delay(1);

            Task t = Task.WhenAll(GetTicket(), LoadSubTask());
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
                CompletedFetch();
        }

        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }
        private async Task GetTicket()
        {
            var result = await TicketService.GetTicketById(TicketId, GlobalClass.Token);
            if (result != null)
                currTicket = result;
            else
                currTicket = new();
        }
        protected async Task<TableData<TicketModel>> LoadSubTicketsById(TableState tableState)
        {
            isTableLoading = true;
            IEnumerable<TicketModel> data = await SubTaskService.SubTaskList(TicketId, GlobalClass.Token);
            await CountTicketTypes(data);
            data = data.Where(ticket =>
            {
                if (string.IsNullOrWhiteSpace(searchStatusName))
                    return true;
                if (ticket.TicketStatusName.Contains(searchStatusName, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            switch (tableState.SortLabel)
            {
                case "SortTicket":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.TicketNumber);
                    break;
            }
            data = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            var total = data.Count();
            isTableLoading = !isTableLoading;
            StateHasChanged();
            return new TableData<TicketModel>()
            {
                TotalItems = total,
                Items = data
            };
        }
        private async Task LoadSubTask()
        {
            var result = await SubTaskService.SubTaskList(TicketId , GlobalClass.Token);
            if (result != null)
                listOfSubTask = result;
            else
                listOfSubTask = new();
        }

        protected void AddSubTask() => NavigationManager.NavigateTo($"/subtask/create/{currTicket.Id}");
        protected void Back() => NavigationManager.NavigateTo($"/ticket/list");
        protected async Task DeactivateSubTask(TicketModel model)
        {
            var parameters = new DialogParameters();
                parameters.Add("ContentText", "Remove sub ticket ?");
                parameters.Add("ButtonText", "Confirm");
                parameters.Add("Color", Color.Success);
                parameters.Add("DialogContentText", "Are you sure you want to remove this sub ticket with ticket number ' " + model.TicketNumber + "' ?");

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

                var resultDialog = await DialogService.Show<Shared.Dialogs.RemoveSubTicketDialog>("Remove sub ticket", parameters,
                options).Result;
                if (!resultDialog.Canceled)
                {
                string thisPage = "subtask";
                model.IsTicketLinkActive = false;
                model.Activity = "Removed Sub-Ticket: Parent-" + currTicket.TicketNumber + "; Sub-Ticket-" + model.TicketNumber ;
                model.UserId = GlobalClass.CurrentUserAccount.EmployeeId;
                await SubTaskService.DeactivateSubTask(model, GlobalClass.Token);
                    NavigationManager.NavigateTo($"/refresh/{thisPage}/{TicketId}");
                }
        }
        protected async Task ReloadSubTasks() 
        {
            searchStatusName = string.Empty;
            await tableVariable.ReloadServerData();
        }
        private async Task CountTicketTypes(IEnumerable<TicketModel> tickets)
        {
            await Task.Run(() => 
            {
                chip1 = tickets.Where(x=>x.TicketStatusId == 1).ToList().Count();
                chip2 = tickets.Where(x=>x.TicketStatusId == 2).ToList().Count();
                chip3 = tickets.Where(x=>x.TicketStatusId == 8).ToList().Count();
                chip4 = tickets.Where(x=>x.TicketStatusId == 5).ToList().Count();
                chip5 = tickets.Where(x=>x.TicketStatusId == 6).ToList().Count();
            });
            StateHasChanged();
        }

        protected async Task FilterTableByChip(string ticketStatusName)
        {
            searchStatusName = ticketStatusName;
            await tableVariable.ReloadServerData();
        }
    }
}