namespace fgciitjo.Pages.Reports
{
    public class TicketListReportBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected IReportService ReportService { get; set; } = default!;
        [Inject] protected ApplicationState AppState { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        protected bool dataFetched, ShowLoadingOverlay, isTableReset;
        private FilterParameter filterParameter = new FilterParameter();
        protected string pdfContent = string.Empty;
        protected double sizeInKb;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Ticket List Report";
            AppState.OnChange += StateHasChanged;
            ShowLoadingOverlay = true;
            while (GlobalClass.CurrentUserAccount == null)
                await Task.Delay(1);
            CompletedFetch();
        }
        private void CompletedFetch()
        {
            dataFetched = true;
            ShowLoadingOverlay = false;
            StateHasChanged();
        }

        public void Dispose() => AppState.OnChange -= StateHasChanged;
        private async Task GenerateReport()
        {
            ShowLoadingOverlay = true;
            filterParameter.Token = GlobalClass.Token;
            filterParameter.PreparedBy = GlobalClass.CurrentUserAccount.EmployeeName;
            filterParameter.PreparedByDesignation = GlobalClass.CurrentUserAccount.Designation;
            var response = await ReportService.GetTicketListReportContent(GlobalClass.Token, filterParameter);
            if (response != null)
                pdfContent = response.ReportContent;
            else
                Extensions.ShowAlertV2("No data found.", Variant.Filled, SnackbarService,Severity.Warning,
                    Icons.Material.Filled.SearchOff, Defaults.Classes.Position.TopRight);
            GlobalClass.filterParameter = new FilterParameter();
            CompletedFetch();
        }

        protected async Task ReloadData(bool value)
        {
            filterParameter = GlobalClass.filterParameter;
            await GenerateReport();
        }

        protected async void ResetReport()
        {
            pdfContent = string.Empty;
            isTableReset = true;
            GlobalClass.filterParameter = new FilterParameter();
            await Task.Delay(1000);
            isTableReset = false;
            StateHasChanged();
        }
    }
}