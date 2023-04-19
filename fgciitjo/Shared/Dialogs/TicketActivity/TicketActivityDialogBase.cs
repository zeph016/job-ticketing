namespace fgciitjo.Shared.Dialogs.TicketActivity
{
    public class TicketActivityDialogBase : ComponentBase
    {
        #region Services
        [Inject] private ITicketActivityService TicketActivityService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = new();
        [Parameter] public string DialogTitle { get; set; } = string.Empty;
        [Parameter] public string IconString { get; set; } = string.Empty;
        [Parameter] public string ButtonText { get; set; } = string.Empty;
        [Parameter] public string TicketNumber { get; set; } = string.Empty;
        [Parameter] public Color Color { get; set; }
        [Parameter] public long Id { get; set; }
        [Parameter] public long TicketId { get; set; }
        [Parameter] public long OtherActivityId { get; set; }
        [Parameter] public bool IsOtherActivity { get; set; }
        [Parameter] public Enums.TicketStatusType TicketStatusTypeId { get; set; }
        [Parameter] public Enums.CrudeMode ActionMode { get; set; }
        [Parameter] public DateTime TicketDate { get; set; }
        protected TicketActivityModel ticketActivity = new();
        protected bool dataFetched, isSaving;
        protected DateTime? selectedDate = DateTime.Now;
        protected DateTime? minDate;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (ActionMode == Enums.CrudeMode.Add)
            {
                SelectDefaultStatus();
                ticketActivity.TicketId = TicketId;
                TicketDate = TicketDate.AddDays(-1);
                minDate = TicketDate;
                CompletedFetch();
            }
            else if (ActionMode == Enums.CrudeMode.Edit)
            {
                minDate = TicketDate.AddDays(-1);
                Task t = GetActivityById();
                await t;
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    selectedDate = ticketActivity.ActivityDate;
                    CompletedFetch();
                }
            }

            if (IsOtherActivity)
            {
                minDate = DateTime.Now.AddDays(-20);
                CompletedFetch();
            }
                
        }

        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }

        private async Task GetActivityById()
        {
            if (OtherActivityId != 0)
                Id = OtherActivityId;
            if (ActionMode == Enums.CrudeMode.Edit)
            {
                Task<TicketActivityModel> GetTicketActivity = TicketActivityService.GetActivityById(Id, GlobalClass.Token);
                await GetTicketActivity;
                if (GetTicketActivity.Status == TaskStatus.RanToCompletion)
                    ticketActivity = GetTicketActivity.Result;
            }
        }

        private void SelectDefaultStatus()
        {
            var result = GlobalList.TicketStatusList.Where(x=>x.Id == 2).FirstOrDefault();
            if (result != null)
            {
                ticketActivity.StatusId = result.Id;
                ticketActivity.StatusName = result.StatusName;
            }
        }

        protected async Task Submit()
        {
            if (Validation())
            {
                isSaving = true;
                MapAdditionalProperties();
                if (!IsOtherActivity)
                {
                    Task<TicketActivityModel> ActivityTask = (ticketActivity.Id == 0 ? AddTicketActivity() : UpdateTicketActivity());
                    await ActivityTask;
                    if (ActivityTask.Status == TaskStatus.Faulted)
                         ShowErrorAlert("Code: Activity method", string.Empty);
                    else
                        MudDialog.Close(DialogResult.Ok(ActivityTask.Result));
                }
                else
                {
                    Task<TicketActivityModel> OtherActivityTask = (OtherActivityId == 0 ? SaveActivityWithoutTicket() : UpdateActivityWithoutTicket());
                    await OtherActivityTask;
                    if (OtherActivityTask.Status ==TaskStatus.Faulted)
                        ShowErrorAlert("Code: Other Activity method", string.Empty);
                    else
                        MudDialog.Close(DialogResult.Ok(OtherActivityTask.Result));
                }
            }
        }

        protected void Cancel() => MudDialog.Cancel();

        private void MapAdditionalProperties()
        {
            ticketActivity.UserId = GlobalClass.CurrentUserAccount.EmployeeId;
            ticketActivity.UserAccountName = GlobalClass.CurrentUserAccount.EmployeeName; 

            DateTime date =  Convert.ToDateTime(selectedDate); 
            ticketActivity.ActivityDate = Convert.ToDateTime(date.ToShortDateString());

            ticketActivity.IsActive = true;
            var selectedStatus = GlobalList.TicketStatusList.Where(x=>x.Id == ticketActivity.StatusId).FirstOrDefault();
            if (selectedStatus != null)
                ticketActivity.StatusName = selectedStatus.StatusName;
        }

        private bool Validation()
        {
            if (TicketStatusTypeId == Enums.TicketStatusType.AllowUpdate)
                return false;
            else
                return true;
        }
        private async Task<TicketActivityModel> AddTicketActivity()
        {
            ticketActivity.Activity = "Add activity";
            Task<TicketActivityModel> AddActivityTask = TicketActivityService.AddTicketActivity(ticketActivity, GlobalClass.Token);
            await AddActivityTask;
            if (AddActivityTask.Status == TaskStatus.RanToCompletion)
            {
                  ShowAlert("Activity has been added", Severity.Success, string.Empty); 
                  return AddActivityTask.Result;
            }
            else
            {
                ShowErrorAlert("Code: Add activity endpoint", string.Empty);
                return new();
            }
                
        }
        private async Task<TicketActivityModel> UpdateTicketActivity()
        {
            ticketActivity.Activity = "Update activity";
            Task<TicketActivityModel> UpdateActivityTask = TicketActivityService.UpdateTicketActivity(ticketActivity, GlobalClass.Token);
            await UpdateActivityTask;
            if (UpdateActivityTask.Status == TaskStatus.RanToCompletion)
            {
                ShowAlert("Activity has been updated", Severity.Info, string.Empty);
                return UpdateActivityTask.Result;
            }
            else
            {
                ShowErrorAlert("Code: Update activity endpoint", string.Empty);
                return new();
            }
                
        }
        private async Task<TicketActivityModel> SaveActivityWithoutTicket()
        {
            ticketActivity.TicketId = null;
            Task<TicketActivityModel> AddActivityTask = TicketActivityService.SaveTicketActivityWithoutTicket(ticketActivity, GlobalClass.Token);
            await AddActivityTask;
            if (AddActivityTask.Status == TaskStatus.RanToCompletion)
            {
                ShowAlert("Activity has been added", Severity.Success, string.Empty);
                return AddActivityTask.Result;
            }
            else
            {
                ShowErrorAlert("Code: Save other activity endpoint", string.Empty);
                return new();
            }
        }
        private async Task<TicketActivityModel> UpdateActivityWithoutTicket()
        {
            ticketActivity.TicketId = null;
            Task<TicketActivityModel> UpdateActivityTask = TicketActivityService.UpdateTicketActivityWithoutTicket(ticketActivity, GlobalClass.Token);
            await UpdateActivityTask;
            if (UpdateActivityTask.Status == TaskStatus.RanToCompletion)
            {
                ShowAlert("Activity has been updated", Severity.Info, string.Empty);
                return UpdateActivityTask.Result;
            }
            else
            {
                ShowErrorAlert("Code: Save other activity endpoint", string.Empty);
                return new();
            }
               
        }

        private void ShowAlert(string message, Severity severity, string iconStr) => Extensions.ShowAlert(message, Variant.Filled, SnackbarService, severity, string.Empty);
        private void ShowErrorAlert(string message, string iconStr) => Extensions.ShowAlert($"An error has occurred, Please contact IT Administrator. {message}", Variant.Filled, SnackbarService, Severity.Error, string.Empty);

        protected async Task CopyTextToClipboard(string text)
        {
            await JSRuntime.InvokeAsync<object>("copyToClipboard", text);
            Extensions.ShowAlert("Copied to clipboard.", Variant.Filled, SnackbarService, Severity.Normal, Icons.Material.Filled.ContentCopy);
        }
    }
}