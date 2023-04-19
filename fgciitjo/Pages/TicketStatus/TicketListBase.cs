namespace fgciitjo.Pages.TicketStatus
{
    public class TicketListBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ITicketService TicketService { get; set; } = default!;
        [Inject] protected ITicketListService TicketListService { get; set; } = default!;
        [Inject] protected ApplicationState AppState { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        [Inject] protected IConfiguration config { get; set; } = default!;
        #endregion
        #region Properties
        private List<TicketComment> ticketComments = new List<TicketComment>();
        private IEnumerable<TicketModel>? pagedData;
        protected MudTable<TicketModel> tableVariable = new MudTable<TicketModel>();
        private FilterParameter filterParameter = new FilterParameter();
        private HubConnection hubConnection = default!, commentConnection = default!;
        protected Enums.AccessLevel accesslevel;
        private string searchTerm = string.Empty;
        private string searchStatusName = string.Empty;
        protected string lblViewUpdate = string.Empty;
        protected bool  dataFetched, isTableLoading, filterChipsVisible;
        protected int rowsPerPage = 15;
        protected int chip1,chip2,chip3,chip4,chip5;
        #endregion
        protected override async Task OnInitializedAsync()
        {
            AppState.OnChange += StateHasChanged;
            GlobalContentTitle.contentTitle = "Ticket List";
            while (GlobalClass.CurrentUserAccount == null)
                await Task.Delay(1);
            Task t = Task.WhenAll(GetFromGlobalClass(), LoadSignalR(), LoadTicketCommentConnection());
                await t;
            if (t.Status == TaskStatus.RanToCompletion)
                CompletedFetch();
        }
        private void CompletedFetch()
        {
            dataFetched = true;
            isTableLoading = false;
            StateHasChanged();
        }
        private async Task<List<TicketModel>> LoadTickets()
        {
            // Task<List<TicketModel>> ticketListTask = TicketListService.LoadTicket(filterParameter, GlobalClass.Token);
            Task<List<TicketModel>> ticketListTask = TicketListService.LoadTicketv2(filterParameter, GlobalClass.Token);
            await ticketListTask;
            if (ticketListTask.Status == TaskStatus.RanToCompletion)
            {
                Console.WriteLine("Loaded Tickets: " + ticketListTask.Result.Count());
                return ticketListTask.Result;
            }
            else
                return new();
        }
        protected async Task<TableData<TicketModel>> LoadData(TableState tableState)
        {
            isTableLoading = true;
            DetermineFilterParams(tableState);
            IEnumerable<TicketModel> data = new List<TicketModel>();
            if(filterParameter.IsName)
                data = await ReloadIsNameFilter();
            else
                data = await LoadTickets();
            data = await FilteredList(data);
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
                case "SortTicketNumber":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.Id);
                    break;
                case "SortTicketStatus":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.TicketStatusId);
                    break;
                case "SortTicketDate":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.TicketDate);
                    break;
                case "SortCategory":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.TicketCategoryName);
                    break;
                case "SortBranch":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.TicketBranchName);
                    break;
                case "SortDepartment":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.RequestorDepartment);
                    break;
                case "SortAssignee":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.AssigneeName);
                    break;
            }
            GlobalList.Tickets = data.ToList();
            int totalItems = data.Count();
            pagedData = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            isTableLoading = !isTableLoading;
            StateHasChanged();
            return new TableData<TicketModel>()
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }
        protected async Task Update(TicketModel ticket)
        {
            if (ticket.AssigneeId > 0)
            {
                var parameters = new DialogParameters { ["currentTicket"] = ticket };
                parameters.Add("ContentText", "Update Status");
                parameters.Add("ButtonText", "Update");
                parameters.Add("Color", Color.Success);
                parameters.Add("_action", Enums.CrudeMode.Edit);

                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader= false };

                var resultDialog = await DialogService.Show<Shared.Dialogs.UpdateTicketStatusDialog>("Update Status", parameters,
                options).Result;

                if (!resultDialog.Canceled)
                {
                    await hubConnection.InvokeAsync("GetTickets", filterParameter);
                    await hubConnection.InvokeAsync("GetTicketAuditTrail", filterParameter);
                }
            }
            else
            {
                var parameters = new DialogParameters { ["currentTicket"] = ticket };
                parameters.Add("ContentText", "Assign IT");
                parameters.Add("ButtonText", "Assign");
                parameters.Add("Color", Color.Success);

                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Small, FullWidth = true, NoHeader= false };

                var resultDialog = await DialogService.Show<Shared.Dialogs.AssignTicketDialog>("Update Status", parameters,
                options).Result;
                if (!resultDialog.Canceled)
                {
                    await hubConnection.InvokeAsync("GetTickets", filterParameter);
                    await hubConnection.InvokeAsync("GetTicketAuditTrail", filterParameter);
                }
            }
        }
        protected async Task CancelTicket(TicketModel ticket)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "Cancel ticket ?");
            parameters.Add("ButtonText", "Confirm");
            parameters.Add("Color", Color.Success);
            parameters.Add("DialogContentText", "Are you sure you want to cancel ticket with ticket number ' " + ticket.TicketNumber
            + "' ?");

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            var resultDialog = await DialogService.Show<Shared.Dialogs.RemoveSubTicketDialog>("Cancel Ticket", parameters,
            options).Result;
            if (!resultDialog.Canceled)
            {
                if (ticket.AssigneeId == null || ticket.AssigneeId == 0)
                {
                    ticket.Activity = "Cancel Ticket; " + "Employee name: " + GlobalClass.CurrentUserAccount.EmployeeName;
                    ticket.UserId = GlobalClass.CurrentUserAccount.EmployeeId;
                    ticket.TicketId = ticket.Id;
                    ticket.Id = ticket.Id;
                    ticket.PCName = "";
                    ticket.TicketStatusTypeId = Enums.TicketStatusType.Complete;
                    ticket.TicketStatusId = 7;
                    await TicketService.CancelTicket(ticket, GlobalClass.Token);
                    await hubConnection.InvokeAsync("GetTickets", filterParameter);
                    await hubConnection.InvokeAsync("GetTicketAuditTrail", filterParameter);
                }
            }
            else
            {
                ShowAlert("Ticket is already " + ticket.TicketStatusName, Severity.Normal);
            }
        }
        private async Task GetFromGlobalClass()
        {
            await Task.Run(() =>
            {
                accesslevel = GlobalClass.CurrentUserAccount.AccessLevel;
            });
        }
        private async Task LoadSignalR()
        {
            try
            {
                hubConnection = new HubConnectionBuilder()
                .WithUrl(config["SignalRConnection"] + "ticketlisthub")
                .WithAutomaticReconnect()
                .Build();

                await hubConnection.StartAsync();

                hubConnection.On<List<TicketModel>>("GetAllTicketList", (_list) =>
                {
                    tableVariable.ReloadServerData();
                    StateHasChanged();
                });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
            }
        }
        private async Task LoadTicketCommentConnection()
        {
            commentConnection = new HubConnectionBuilder()
            .WithUrl(config["SignalRConnection"] + "ticketcommenthub")
            .WithAutomaticReconnect()
            .Build();
            await commentConnection.StartAsync();

            commentConnection.On<TicketComment>("ReceiveTicketComment", (comment) =>
            {
                ticketComments.Add(comment);
                GlobalList.TicketComments = ticketComments.ToList();
            });
        }
        private void DetermineFilterParams(TableState tableState)
        {
            filterParameter.isActive = true;
            filterParameter.TicketAccessLevel = Enums.AccessLevel.Administrator; //Issue on access level endpoints not getting the IT and clients
            if (!GlobalClass.isFromFilter)
            {
                if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator 
                    || GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.IT)
                {
                    filterParameter.IsTicketStatus = true;
                    filterParameter.TicketStatusId = "1,2,8,9"; 
                }
                else if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client)
                {
                    filterParameter.IsRequestor = true;
                    filterParameter.RequestorId = GlobalClass.CurrentUserAccount.EmployeeId;
                }
            }
            else if (GlobalClass.isFromFilter)
            {
                filterParameter = GlobalClass.filterParameter;
                filterParameter.isActive = true;
                filterParameter.TicketAccessLevel = Enums.AccessLevel.Administrator;
                if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator 
                    || GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.IT)
                {
                    filterParameter = GlobalClass.filterParameter;
                }
                else if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client)
                {
                    Console.WriteLine("I am client from filter");
                    filterParameter.IsRequestor = true;
                    filterParameter.RequestorId = GlobalClass.CurrentUserAccount.EmployeeId;
                }
            }
        }
        private async Task<IEnumerable<TicketModel>> FilteredList(IEnumerable<TicketModel> data)
        {
            if (!GlobalClass.isFromFilter)
            {
                if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.IT)
                    data = await Task.Run(() => data.Where(x => x.AssigneeId == null 
                        || x.AssigneeId == GlobalClass.CurrentUserAccount.EmployeeId 
                        || x.RequestorId == GlobalClass.CurrentUserAccount.EmployeeId).ToList());
                if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client)
                    data = await Task.Run(() => data.Where(x => x.RequestorId == GlobalClass.CurrentUserAccount.EmployeeId).ToList());
            }
            if (GlobalClass.isFromFilter)
            {
                if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.IT)
                    data = await Task.Run(() => data.Where(x => x.AssigneeId == GlobalClass.CurrentUserAccount.EmployeeId || x.AssigneeId == null ).ToList());
                if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client)
                    data = await Task.Run(() => data.Where(x => x.RequestorId == GlobalClass.CurrentUserAccount.EmployeeId).ToList());
            }

            if(!string.IsNullOrWhiteSpace(searchTerm))
            {
                data = await Task.Run(() => data.Where(x=>x.TicketNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList());
            }
            await CountTicketTypes(data);
            return data;
        }
        private async Task<IEnumerable<TicketModel>> ReloadIsNameFilter()
        {
            searchTerm = filterParameter.Name;
            filterParameter = new FilterParameter();
            filterParameter.isActive = true;
            filterParameter.TicketAccessLevel = Enums.AccessLevel.Administrator;
            var data = await LoadTickets();
            return data;
        }
        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
                await hubConnection.DisposeAsync();
        }
        private async Task CountTicketTypes(IEnumerable<TicketModel> tickets)
        {
            await Task.Run(() => 
            {
                chip1 = tickets.Where(x=>x.TicketStatusId == 1).ToList().Count();
                chip2 = tickets.Where(x=>x.TicketStatusId == 2).ToList().Count();
                chip3 = tickets.Where(x=>x.TicketStatusId == 8).ToList().Count();
                chip4 = tickets.Where(x=>x.TicketStatusId == 3 || x.TicketStatusId == 4 || x.TicketStatusId == 5).ToList().Count();
                chip5 = tickets.Where(x=>x.TicketStatusId == 6).ToList().Count();
            });
        }
        protected void Print(long ticketId) => NavigationManager.NavigateTo($"/ticketreport/{ticketId}");
        protected async Task PreviewTicket(TicketModel ticket)
        {
            ViewedComments(ticket);
            var parameters = new DialogParameters()
            {
                { "DialogTitle", "Job Description" },
                { "Ticket", ticket},
                { "CancelText", "Close" },
                { "IconString", @Icons.Material.Filled.BugReport }
            };
            var options = new DialogOptions()
            {
                CloseButton = false,
                MaxWidth = MaxWidth.Large,
                FullWidth = false,
                NoHeader = true,
                DisableBackdropClick = false
            };
            var resultDialog = await DialogService.Show<Shared.Dialogs.TicketList.PreviewTicketDialog>(string.Empty, parameters, options).Result;
        }
        private void ShowAlert(string message, Severity severity)
        {
            SnackbarService.Clear();
            SnackbarService.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            SnackbarService.Configuration.SnackbarVariant = Variant.Filled;
            SnackbarService.Configuration.MaxDisplayedSnackbars = 10;
            SnackbarService.Add($"{message}", severity);
        }
        private void ViewedComments(TicketModel ticket)
        {
            ticketComments.RemoveAll(x => x.TicketId == ticket.Id);
            GlobalList.TicketComments.RemoveAll(x => x.TicketId == ticket.Id);
        }
        protected void Add() => NavigationManager.NavigateTo($"/ticket/create");
        private void ViewChatPage() => NavigationManager.NavigateTo($"/chat");
        protected bool IsVisibleToAssignedIT(TicketModel ticketModel)
        {
            return GlobalList.TicketComments.Any(x => x.TicketId == ticketModel.Id)
            && GlobalList.TicketComments.Any(x => x.UserId != GlobalClass.CurrentUserAccount.EmployeeId)
            && GlobalClass.CurrentUserAccount.EmployeeId == ticketModel.AssigneeId;
        }
        protected bool IsVisibleToClientAndNotAssigned(TicketModel ticketModel)
        {
            return ticketModel.AssigneeId == null
            && GlobalList.TicketComments.Any(x => x.UserId != GlobalClass.CurrentUserAccount.EmployeeId)
            || GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client;
        }
        protected async Task ReloadTable(bool value)
        {
            GlobalClass.isFromFilter = value;
            filterParameter = new FilterParameter();
            searchTerm = searchStatusName = string.Empty;
            await tableVariable.ReloadServerData();
        }
        public void Dispose() => AppState.OnChange -= StateHasChanged;
        protected async Task FilterTableByChip(string ticketStatusName)
        {
            searchStatusName = ticketStatusName;
            await tableVariable.ReloadServerData();
        }
    }
}