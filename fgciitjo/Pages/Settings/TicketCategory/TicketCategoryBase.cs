namespace fgciitjo.Pages.Settings.TicketCategory
{
    public class TicketCategoryBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ITicketCategoryService TicketCategoryService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        protected MudTable<TicketCategoryModel> tableVariable = new MudTable<TicketCategoryModel>();
        private IEnumerable<TicketCategoryModel> pagedData = new List<TicketCategoryModel>();
        protected bool dataFetched, isTableLoading;
        protected string searchTerm = "";
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Ticket Category";
            while (GlobalClass.CurrentUserAccount == null)
                await Task.Delay(1);
            CompletedFetch();
        }
        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }
        protected async Task<TableData<TicketCategoryModel>> LoadTicketCategories(TableState tableState)
        {
            isTableLoading = true;
            IEnumerable<TicketCategoryModel> data = await TicketCategoryService.LoadTicketCategory(GlobalClass.Token);
            switch (tableState.SortLabel)
            {
                case "SortCategoryType":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.CategoryTypeId);
                    break;
                case "SortCategoryName":
                    data = data.OrderByDirection(tableState.SortDirection, x=>x.CategoryName);
                    break;
            }
            data = data.Where(model => 
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return true;
                if (model.CategoryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }).ToArray();
            GlobalList.ticketCategoryList = data.ToList();
            int totalItems = data.Count();
            pagedData = data.Skip(tableState.Page * tableState.PageSize).Take(tableState.PageSize).ToArray();
            isTableLoading = !isTableLoading;
            return new TableData<TicketCategoryModel>()
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }
        protected async Task AddTicketCategory()
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator)
            {
                var parameters = new DialogParameters();
                parameters.Add("ContentText", "Add Category");
                parameters.Add("ButtonText", "Add");
                parameters.Add("Color", Color.Success);
                parameters.Add("_action", Enums.CrudeMode.Add);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = false };

                var resultDialog = await DialogService.Show<Shared.Dialogs.TicketCategoryDialog>("Add Ticket", parameters,
                options).Result;
                if (!resultDialog.Canceled)
                    await ReloadTable();
            }
            else
                Extensions.ShowAlert("Access is denied, Please ask Administrator for assistance. ", Variant.Filled, SnackbarService,Severity.Error, string.Empty);
        }

        protected async Task EditTicketCategory(TicketCategoryModel currTicketCategory)
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Administrator)
            {
                var parameters = new DialogParameters { ["currTicketCategory"] = currTicketCategory };
                parameters.Add("ContentText", "Modify Category");
                parameters.Add("ButtonText", "Update");
                parameters.Add("Color", Color.Info);
                parameters.Add("_action", Enums.CrudeMode.Edit);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = false };
                var resultDialog = await DialogService.Show<Shared.Dialogs.TicketCategoryDialog>("Update Category", parameters,
                options).Result;
                if (!resultDialog.Canceled)
                    await ReloadTable();
            }
            else
                Extensions.ShowAlert("Access is denied, Please ask Administrator for assistance.", Variant.Filled, SnackbarService, Severity.Error, string.Empty);  
        }

        private bool FilterItems(TicketCategoryModel items)
        {
            if (items.CategoryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                return true;
            else if(items.CategoryTypeId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        protected async Task ReloadTable() => await tableVariable.ReloadServerData();

        protected async Task OnSearch(string text)
        {
            searchTerm = text;
            await tableVariable.ReloadServerData();
        }
    }
}