namespace fgciitjo.Pages.Settings.TicketStatus
{
    public class TicketStatusBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ITicketStatusService TicketStatusService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        private IEnumerable<TicketStatusModel> pagedData = new List<TicketStatusModel>();
        protected MudTable<TicketStatusModel> tableVariable = new MudTable<TicketStatusModel>();
        private FilterParameter filterParameter = new FilterParameter();
        protected bool dataFetched, isTableLoading;
        protected string searchTerm = "";
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Ticket Status";
            while (GlobalClass.CurrentUserAccount == null)
                await Task.Delay(1);
            CompletedFetch();
        }
        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }
        protected async Task<TableData<TicketStatusModel>> LoadTicketStatus(TableState tableState)
        {
            isTableLoading = true;
            IEnumerable<TicketStatusModel> data = await TicketStatusService.GetTicketStatus(filterParameter, GlobalClass.Token);
            switch (tableState.SortLabel)
            {
                case "SortStatus":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.Id);
                    break;
                case "SortStatusType":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.StatusTypeId);
                    break;
            }
            data = data.Where(model =>
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return true;
                if (model.StatusName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            GlobalList.TicketStatusList = data.ToList();
            int totalItems = data.Count();
            pagedData = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            isTableLoading = !isTableLoading;
            return new TableData<TicketStatusModel>()
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        protected async Task AddTicketStatus()
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator)
            {
                var parameters = new DialogParameters();
                parameters.Add("ContentText", "Add Status");
                parameters.Add("ButtonText", "ADD");
                parameters.Add("Color", Color.Success);
                parameters.Add("_action", Enums.CrudeMode.Add);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

                var resultDialog = await DialogService.Show<Shared.Dialogs.TicketStatusDialog>("Add Ticket", parameters,
                options).Result;
                if (!resultDialog.Canceled)
                    await tableVariable.ReloadServerData();
                else
                    Extensions.ShowAlert("An error has occurred check with administrator. ", Variant.Filled, SnackbarService, Severity.Error, string.Empty);
                
            }
        }
        protected async Task EditTicketStatus(TicketStatusModel currTicketStatus)
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator)
            {
                var parameters = new DialogParameters { ["currTicketStatus"] = currTicketStatus };
                parameters.Add("ContentText", "Modify Status");
                parameters.Add("ButtonText", "UPDATE");
                parameters.Add("Color", Color.Info);
                parameters.Add("_action", Enums.CrudeMode.Edit);
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
                var resultDialog = await DialogService.Show<Shared.Dialogs.TicketStatusDialog>("Update Ticket", parameters,
                options).Result;
                if (!resultDialog.Canceled)
                await tableVariable.ReloadServerData();
            }
            else
                Extensions.ShowAlert("Access is denied, Please ask Administrator for assistance.", Variant.Filled, SnackbarService, Severity.Error, string.Empty);  
        }
        protected async Task ReloadTable() =>  await tableVariable.ReloadServerData();
        protected async Task OnSearch(string text)
        {
            searchTerm = text;
            await tableVariable.ReloadServerData();
        }
    }
    
}