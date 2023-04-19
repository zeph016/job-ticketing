namespace fgciitjo.Pages.Settings.TicketBranch
{
    public class TicketBranchBase : ComponentBase
    {
        #region Inject Services
        [Inject] protected ITicketBranchService TicketBranchService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        protected bool dataFetched, isTableLoading;
        protected MudTable<TicketBranchModel> tableVariable = new MudTable<TicketBranchModel>();
        private IEnumerable<TicketBranchModel> pagedData = new List<TicketBranchModel>();
        private FilterParameter filterParameter = new FilterParameter();
        protected string searchTerm = "";
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Ticket Branch";
            while (GlobalClass.CurrentUserAccount == null)
                await Task.Delay(1);
            CompleteFetched();
        }

        private void CompleteFetched()
        {
            dataFetched = true;
            StateHasChanged();
        }

        protected async Task<TableData<TicketBranchModel>> LoadBranches(TableState tableState)
        {
            isTableLoading = true;
            IEnumerable<TicketBranchModel> data = await TicketBranchService.GetBranch(filterParameter, GlobalClass.Token);
            switch (tableState.SortLabel)
            {
                case "SortBranchName":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.BranchName);
                    break;
            }
            data = data.Where(model =>
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return true;
                if (model.BranchName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            GlobalList.TicketBranchList = data.ToList();
            int totalItems = data.Count();
            pagedData = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            isTableLoading = !isTableLoading;
            return new TableData<TicketBranchModel>()
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        protected async Task AddTicketBranch()
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator)
            {
                var parameters = new DialogParameters();
                parameters.Add("ContentText", "Add Branch");
                parameters.Add("ButtonText", "Add");
                parameters.Add("Color", Color.Success);
                parameters.Add("_action", Enums.CrudeMode.Add);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = false };
                var resultDialog = await DialogService.Show<Shared.Dialogs.TicketBranchDialog>("Add Ticket Branch", parameters, options).Result;
                if (!resultDialog.Canceled)
                    await ReloadTable();
            }
            else
                Extensions.ShowAlert("Access is denied, Please ask Administrator for assistance. ", Variant.Filled, SnackbarService,Severity.Error, string.Empty);
        }

        protected async Task EditTicketBranch(TicketBranchModel currentBranch)
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator)
            {
                var parameters = new DialogParameters { ["currentBranch"] = currentBranch };
                parameters.Add("ContentText", "Modify Branch");
                parameters.Add("ButtonText", "Update");
                parameters.Add("Color", Color.Info);
                parameters.Add("_action", Enums.CrudeMode.Add);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = false };
                var resultDialog = await DialogService.Show<Shared.Dialogs.TicketBranchDialog>("Update Ticket Branch", parameters, options).Result;
                if (!resultDialog.Canceled)
                    await ReloadTable();
            }
            else{
                Extensions.ShowAlert("Access is denied, Please ask Administrator for assistance. ", Variant.Filled, SnackbarService,Severity.Error, string.Empty);
            }
        }

        protected async Task ReloadTable() => await tableVariable.ReloadServerData();

        protected async Task OnSearch(string text)
        {
            searchTerm = text;
            await tableVariable.ReloadServerData();
        }
    }
}