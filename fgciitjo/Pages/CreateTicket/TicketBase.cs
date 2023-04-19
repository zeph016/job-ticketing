using fgciitjo.domain.clsEnums;

namespace fgciitjo.Pages.CreateTicket
{
    public class TicketBase : ComponentBase
    {
        #region Services Injection
        [Inject] protected ITicketService ticketService { get; set;} = default!;
        [Inject] protected IGlobalService globalService { get; set; } = default!;
        [Inject] protected ISnackbar snackbarService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected IConfiguration _config { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        #endregion

        #region Properties
        [CascadingParameter] public AppStoreState ApplicationState { get; set; } = new();
        [Parameter] public long ticketId { get; set; }
        [Parameter] public long parentId { get; set; }
        protected bool dataFetched, isPriorityReadOnly, isTitleReadOnly, isCategoryReadOnly, isBranchReadOnly, isDescriptionReadOnly,
        isEmailReadOnly, isLocationReadOnly, isCategoryTypeReadOnly, isBtnDisabled, isClearable, isRadioDisabled, _isPopUpOpen, isUploading, _processing;
        protected DateTime? dateTimeToday;
        protected TicketModel currTicket { get; set; } = new TicketModel();
        protected HubConnection hubConnection = default!;
        protected string currentUrl = string.Empty, btnSubmitLabel = string.Empty, issueTitleHeader = string.Empty,
        acceptedFileFormat = ".jpg,.jpeg,.png,.pdf,.xls,.xlsx,.csv,.doc,.docx";
        protected Enums.CrudeMode _action;
        protected UserAccount requestorEmployee = new();
        protected TicketCategoryModel selectedTicketCategory = new();
        protected List<TicketFileAttachmentModel> FilesAttachedList = new(), removedFileList = new();
        protected long maxAllowedSize = 5242880;
        protected int progressValue;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Ticket";
            CheckUrl();
            await MapGlobalTime();
            await LoadConnection();
            if (_action == Enums.CrudeMode.Add)
            {
                Task t = Task.WhenAll(GetRequestor());         
                await t;
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    MapDefaults();
                    currentUrl = NavigationManager.Uri;
                    string toCheck = "subtask";
                    if (currentUrl.Contains(toCheck))
                    {
                        await GetSubTicketValues();
                    }
                    CheckActionsMode();
                    CompletedFetch();
                }
            }
            else
            {
                Task u = Task.WhenAll(GetTicketId());
                await u;
                if (u.Status == TaskStatus.RanToCompletion)
                {
                    CheckActionsMode();
                    CompletedFetch();
                }
            }
        }
        protected void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }

        #region Mappers
        protected async void MapDefaults()
        {
            currTicket = new TicketModel();
            dateTimeToday =  GlobalContentTitle.contentServerTime;
            var ticketStatus = GlobalList.TicketStatusList.Where(x=>x.Id == 1).FirstOrDefault();
            if (ticketStatus != null) 
            {
                currTicket.TicketStatusId = ticketStatus.Id;
                currTicket.TicketStatusName = ticketStatus.StatusName;
            }
            var defaultBranch = GlobalList.TicketBranchList.Where(x=>x.Id == 1).FirstOrDefault();
            if (defaultBranch != null)
            {
                currTicket.TicketBranchId = defaultBranch.Id;
                currTicket.TicketBranchName = defaultBranch.BranchName;
            }
            var defaultTicketStatus = GlobalList.TicketStatusList.Where(x=>x.Id == 1).FirstOrDefault();
            if (defaultTicketStatus != null)
            {
                currTicket.TicketStatusId = defaultTicketStatus.Id;
                currTicket.TicketStatusName = defaultTicketStatus.StatusName;
            }
            currTicket.RequestorId = GlobalClass.CurrentUserAccount.EmployeeId;
            currTicket.RequestorName = GlobalClass.CurrentUserAccount.EmployeeName;
            currTicket.PriorityLevelId = Enums.PriorityLevel.Low;
            currTicket.ContactInformation = "None";
            currTicket.TicketDate = await GetServerTime();
        }
        protected async Task<TicketModel> MapProperties()
        {
            currTicket.RequestorId = requestorEmployee.EmployeeId;
            currTicket.RequestorName = requestorEmployee.EmployeeName;
            Task<DateTime> dateTimeGet = GetServerTime();
            await dateTimeGet;
            if (dateTimeGet.Status == TaskStatus.RanToCompletion)
            {
                currTicket.TicketDate = Convert.ToDateTime(Convert.ToDateTime(dateTimeToday).ToString("MM/dd/yyyy") + " " + dateTimeGet.Result.ToString("h:mm:ss tt"));
                Console.WriteLine(currTicket.TicketDate);
            }
            currTicket.DocumentReference = "";
            currTicket.UserId = GlobalClass.CurrentUserAccount.EmployeeId;

            var selTicketBranch = GlobalList.TicketBranchList.Where(x=>x.Id == currTicket.TicketBranchId).FirstOrDefault();
            if (selTicketBranch != null)
                currTicket.TicketBranchName = selTicketBranch.BranchName;
            
            var selTicketStatus = GlobalList.TicketStatusList.Where(x=>x.Id == currTicket.TicketStatusId).FirstOrDefault();
            if (selTicketStatus != null)
                currTicket.TicketStatusName = selTicketStatus.StatusName;

            if (_action == Enums.CrudeMode.Add)
            {
                currTicket.IsActive = true;
                if (parentId != 0 && _action == Enums.CrudeMode.Add)
                {
                    currTicket.TicketParentId = parentId;
                    currTicket.IsTicketLinkActive = true;
                    currTicket.Activity = "Add Sub Task";
                }
                else
                {
                    currTicket.Activity = "Create ticket"; currTicket.Activity = "Create ticket";
                }
            }
            else if (_action == Enums.CrudeMode.Edit)
            {
                var newFiles = currTicket.TicketFileAttachmentModels.Where(x=>x.TicketId == 0).ToList();
                if (newFiles != null && newFiles.Count() != 0)
                {
                    currTicket.TicketFileAttachmentModels = newFiles;
                    currTicket.TicketFileAttachmentModels.ForEach(x => x.TicketId = currTicket.Id);
                }
                currTicket.Activity = "Update ticket";
            }
            return currTicket;
        }
        protected async Task GetTicketId()
        {
            currTicket = await ticketService.GetTicketById(ticketId, GlobalClass.Token);
            if(currTicket.TicketStatusId > 1)
                _action = Enums.CrudeMode.View;
                
            var employee = await GetEmployee(currTicket.RequestorId);
            if (employee != null)
                requestorEmployee = employee;

            var siteBranch = GlobalList.TicketBranchList.Where(x=>x.Id == currTicket.TicketBranchId).FirstOrDefault();
            if (siteBranch != null) 
            {
                currTicket.TicketBranchId = siteBranch.Id;
                currTicket.TicketBranchName = siteBranch.BranchName;
            }
            var ticketCategory = GlobalList.ticketCategoryList.Where(x=>x.Id == currTicket.TicketCategoryId).FirstOrDefault();
            if (ticketCategory != null) 
            {
                currTicket.TicketCategoryId = ticketCategory.Id;
                currTicket.TicketCategoryName = ticketCategory.CategoryName;
                currTicket.TicketCategoryTypeId = ticketCategory.CategoryTypeId;
            }
            var ticketStatus = GlobalList.TicketStatusList.Where(x=>x.Id == currTicket.TicketStatusId).FirstOrDefault();
            if (ticketStatus != null)
            {
                currTicket.TicketStatusId = ticketStatus.Id;
                currTicket.TicketStatusName = ticketStatus.StatusName;
            } 
            dateTimeToday = currTicket.TicketDate;

            if (currTicket.TicketStatusId != 1)
                DisableControls();
        }
        protected void SelectedPriority(Enums.PriorityLevel selPriority)
        {
            if (AccountCheckPriority())
                currTicket.PriorityLevelId = selPriority;
        }
        protected async Task<DateTime> GetServerTime()
        {
            var dateTime = await globalService.LoadServerTime();
            if (dateTime != null)
                return Convert.ToDateTime(dateTime.ToString("MM/dd/yyyy h:mm:ss tt"));
            else
                return DateTime.Now;
        }
        #endregion

        #region Resetters
        protected void ResetFileAttachments()
        {
            currTicket.TicketFileAttachmentModels = new List<TicketFileAttachmentModel>();
            Extensions.ShowAlert("Removed attached files.", Variant.Filled, snackbarService, Severity.Normal, Icons.Material.Filled.DeleteForever);
        }

        protected void ResetTicketCategoryList()
        {
            var defaultCategory = GlobalList.ticketCategoryList.FirstOrDefault();
            if (defaultCategory != null)
            {
                currTicket.TicketCategoryId = defaultCategory.Id;
                currTicket.TicketCategoryName = defaultCategory.CategoryName;
                currTicket.TicketCategoryTypeId = defaultCategory.CategoryTypeId;
            }
        }
        #endregion

        #region Services Access
        protected async Task SaveTicket()
        {
            var _ticket = Task.Run(async () => { return await ticketService.SaveTicket(await MapProperties(), GlobalClass.Token); });
            await _ticket;
            if (_ticket.Status == TaskStatus.RanToCompletion)
            {
                currentUrl = NavigationManager.Uri;
                string toCheck = "subtask";
                currTicket = _ticket.Result ?? new TicketModel();
                currTicket.TicketNumber = currTicket.TicketNumber;

                if (!currentUrl.Contains(toCheck))
                    NavigationManager.NavigateTo($"/ticket/list");
                else
                    NavigationManager.NavigateTo($"/subtask/{parentId}");
                _processing = false;
                Extensions.ShowAlert("Ticket has been succcesfully submitted.", Variant.Filled, snackbarService, Severity.Success, string.Empty);
                await InvokeSignalR();
            }
            else
            {
                _processing = false;
                Extensions.ShowAlert("Error submitting ticket.", Variant.Filled, snackbarService, Severity.Error, string.Empty);
            }
        }
        protected async Task UpdateTicket()
        {
            Task t = UpdateTicketTask();
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
            {
                _processing = false;
                NavigationManager.NavigateTo($"/ticket/list");
                Extensions.ShowAlert("Ticket has been succcesfully updated.", Variant.Filled, snackbarService, Severity.Info, string.Empty);
                await InvokeSignalR();
            }
            else
            {
                _processing = false;
                Extensions.ShowAlert("Error updating ticket.", Variant.Filled, snackbarService, Severity.Error, string.Empty);
            }
        }
        protected async Task MapGlobalTime()
        {
            DateTime serverTime = await globalService.LoadServerTime();
            GlobalContentTitle.contentServerTime = serverTime;
        }
        protected async Task<UserAccount> GetEmployee(long employeeId)
        {
            var filterParameter = new FilterParameter()
            {
                IsName = false,
                IsLookUp = true
            };
            var employee = await globalService.GetEmployeeV2(GlobalClass.Token, employeeId);
            return employee;
        }
        protected async Task<IEnumerable<UserAccount>> GetAllEmployees(string employeeName)
        {
            var filterParameter = new FilterParameter()
            {
                IsName = true,
                Name = employeeName,
                IsLookUp = true
            };
            var employees = await globalService.GetAllEmployees(filterParameter, GlobalClass.Token);
            return employees;
        }
        protected async Task UpdateTicketTask() => await ticketService.UpdateTicket(await MapProperties(), GlobalClass.Token);
        protected async Task GetRequestor()
        {
            await Task.Run(() => {
                currentUrl = NavigationManager.Uri;
                string toCheck = "subtask";
                if (!currentUrl.Contains(toCheck))
                {
                    requestorEmployee = GlobalClass.CurrentUserAccount;
                }
            });
        }
        protected async Task GetTicketCategoryByType()
        {
            var ticketCategoryList = await Task.Run(() => GlobalList.ticketCategoryList.Where(x=>x.CategoryTypeId == currTicket.TicketCategoryTypeId).ToList());
            var defaultCategory = ticketCategoryList.FirstOrDefault();
            if (defaultCategory != null)
            {
                currTicket.TicketCategoryId = defaultCategory.Id;
                currTicket.TicketCategoryName = defaultCategory.CategoryName;
                currTicket.TicketCategoryTypeId = defaultCategory.CategoryTypeId;
            }
            StateHasChanged();
        }
        protected async Task<IEnumerable<TicketCategoryModel>> SearchCategoryList(string searchText)
        {
            return await Task.FromResult(GlobalList.ticketCategoryList.Where(x => x.CategoryName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList());
        }
        protected async Task GetSubTicketValues()
        {
            var sub = new TicketModel();
            sub = await ticketService.GetTicketById(parentId, GlobalClass.Token);
            //Map to viewing and for updating;
            var employee = await GetEmployee(sub.RequestorId);
            if (employee != null)
                requestorEmployee = employee;

            var siteBranch = GlobalList.TicketBranchList.Where(x=>x.Id == sub.TicketBranchId).FirstOrDefault();
            if (siteBranch != null) 
            {
                currTicket.TicketBranchId = siteBranch.Id;
                currTicket.TicketBranchName = siteBranch.BranchName;
            }

            var ticketCategory = GlobalList.ticketCategoryList.Where(x=>x.Id == sub.TicketCategoryId).FirstOrDefault();
            if (ticketCategory != null) 
            {
                currTicket.TicketCategoryId = ticketCategory.Id;
                currTicket.TicketCategoryName = ticketCategory.CategoryName;
                currTicket.TicketCategoryTypeId = ticketCategory.CategoryTypeId;
            }
            currTicket.IssueSummary = sub.IssueSummary;
            currTicket.TaskDescription = sub.TaskDescription;
            
            currTicket.Location = sub.Location;
        }
        #endregion

        #region SignalR Initilization
        protected async Task LoadConnection()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(_config["SignalRConnection"] + "ticketlisthub")
                .WithAutomaticReconnect()
                .Build();
                await hubConnection.StartAsync();
        }

        protected async Task InvokeSignalR()
        {
            var filterParameter = new FilterParameter()
            {
                Token = GlobalClass.Token,
                PageSize = GlobalClass.PageSize
            };
            Task t = Task.WhenAll(GetTicketHubConnection(filterParameter), GetTicketAuditTrailHubConnection(filterParameter));
            await t;
        }

        protected async Task GetTicketHubConnection(FilterParameter filter) => await hubConnection.InvokeAsync("GetTickets", filter);
        protected async Task GetTicketAuditTrailHubConnection(FilterParameter filter) => await hubConnection.InvokeAsync("GetTicketAuditTrail", filter);
        #endregion
        
        #region Validators
        protected void CheckUrl()
        {
            currentUrl = NavigationManager.Uri;
            if (currentUrl.Contains("create"))
            {
                _action = Enums.CrudeMode.Add;
                btnSubmitLabel = "Submit";
            }
            else
            {
                _action = Enums.CrudeMode.Edit;
                btnSubmitLabel = "Update";
            }
        }
        
        protected void CheckActionsMode()
        {
            if (_action == Enums.CrudeMode.Add) {
                issueTitleHeader = "REPORT A PROBLEM OR CONCERN";
                isClearable = true;
                CheckAccessLevel();
            }
            else if (currTicket.TicketStatusId == 1 && _action == Enums.CrudeMode.Edit) {
                issueTitleHeader = "ISSUE / CONCERN";
                isClearable = true;
                CheckAccessLevel();
            }
            else if (currTicket.TicketStatusId > 1 && _action == Enums.CrudeMode.Edit) {
                issueTitleHeader = "ISSUE / CONCERN";
                isBtnDisabled = true;
                isClearable = false;
                isRadioDisabled =  true;
            }
        }
        protected bool CheckAccessLevel()
        {
            if(GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client)
                return isPriorityReadOnly = true;
            else
                return isPriorityReadOnly = false;
        }
        protected void DisableControls()
        {
            isTitleReadOnly = true;
            isCategoryReadOnly = true;
            isBranchReadOnly = true;
            isDescriptionReadOnly = true;
            isEmailReadOnly = true;
            isPriorityReadOnly = true;
            isLocationReadOnly = true;
            isCategoryTypeReadOnly = true;
        }

        protected bool AccountCheckPriority()
        {
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client) 
            {
                Extensions.ShowAlertV2("Contact IT Support for escalation of priority.",Variant.Filled,snackbarService,Severity.Normal,Icons.Material.Filled.Security,Defaults.Classes.Position.TopRight);
                return false;
            }
            return true;
        }
        #endregion
       
        #region Dialogs
        protected async Task Cancel()
        {
            var parameters = new DialogParameters();
            string dialogTitle = "Cancel";
            parameters.Add("DialogTitle", dialogTitle);
            parameters.Add("ContentText", "Cancel Entry");
            parameters.Add("ButtonText", "Confirm");
            parameters.Add("actionMode", Enums.CrudeMode.Cancel);
            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var resultDialog = await DialogService.Show<Shared.Dialogs.GenericDialogs.GenericDialog>(dialogTitle, parameters, options).Result;
            if (!resultDialog.Canceled)
                NavigateToPage("ticket-list");
        }

        protected async Task CreateNew()
        {
            var parameters = new DialogParameters();
            string dialogTitle = "Cancel";
            parameters.Add("DialogTitle", dialogTitle);
            parameters.Add("ContentText", "Create new ticket?");
            parameters.Add("ButtonText", "Create");
            parameters.Add("actionMode", Enums.CrudeMode.Cancel);
            var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
            var resultDialog = await DialogService.Show<Shared.Dialogs.GenericDialogs.GenericDialog>(dialogTitle, parameters, options).Result;
            if (!resultDialog.Canceled)
                NavigateToPage("create-ticket");
        }
        #endregion
        
        #region Navigations
        protected void ToTicketList() => NavigationManager.NavigateTo($"/ticket/list");
        protected void NavigateToPage(string urlKeyword) => NavigationManager.NavigateTo($"/refresh/{urlKeyword}");
        #endregion

        #region Ticket file attachments
        protected async Task DownloadFile(TicketFileAttachmentModel file)
        {
            string jpg = "data:image/jpeg;base64,",
            png = "data:image/png;base64,",
            pdf = "data:application/pdf;base64,",
            doc = "data:application/msword;base64,",
            docx = "data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,",
            xls = "data:application/vnd.ms-excel;base64,",
            xlsx = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,",
            contentType = "", fileName = file.FileName;
            if (file.FileName.Contains(".jpg", StringComparison.OrdinalIgnoreCase) || file.FileName.Contains(".jpeg", StringComparison.OrdinalIgnoreCase))
                contentType = jpg + Convert.ToBase64String(file.FileAttachment);
            if (file.FileName.Contains(".png", StringComparison.OrdinalIgnoreCase) || file.FileName.Contains(".PNG", StringComparison.OrdinalIgnoreCase))
                contentType = png + Convert.ToBase64String(file.FileAttachment);
            if (file.FileName.Contains(".pdf", StringComparison.OrdinalIgnoreCase))
                contentType = pdf + Convert.ToBase64String(file.FileAttachment);
            if (file.FileName.Contains(".doc", StringComparison.OrdinalIgnoreCase))
                contentType = doc + Convert.ToBase64String(file.FileAttachment);
            if (file.FileName.Contains(".docx", StringComparison.OrdinalIgnoreCase))
                contentType = docx + Convert.ToBase64String(file.FileAttachment);
            if (file.FileName.Contains(".xls", StringComparison.OrdinalIgnoreCase) || file.FileName.Contains(".csv", StringComparison.OrdinalIgnoreCase))
                contentType = xls + Convert.ToBase64String(file.FileAttachment);
            if (file.FileName.Contains(".xlsx", StringComparison.OrdinalIgnoreCase))
                contentType = xlsx + Convert.ToBase64String(file.FileAttachment);
            await JSRuntime.InvokeAsync<object>("saveFile", fileName, contentType);
        }
        protected async Task UploadFiles(InputFileChangeEventArgs e)
        {
            progressValue = 0;
            isUploading = true;
            foreach (var file in e.GetMultipleFiles(int.MaxValue))
            {
                //Lazy Loader
                progressValue += 100/e.FileCount;
                StateHasChanged();
                await Task.Delay(500);
                if (isAllowedSize(file) && !isDuplicateFile(file)) 
                {
                    var fileSize = new byte[file.Size];
                    await file.OpenReadStream(maxAllowedSize).ReadAsync(fileSize);
                    string fileAttachment = Convert.ToBase64String(fileSize);
                    currTicket.TicketFileAttachmentModels.Add(new TicketFileAttachmentModel 
                    {
                        FileName = file.Name.ToLowerInvariant(), //Convert to lower case for compatibility
                        FileAttachment = Convert.FromBase64String(fileAttachment),
                        UserAccountId = GlobalClass.CurrentUserAccount.EmployeeId,
                        Remarks = string.Empty,
                        IsActive = true
                    });
                }
            }
            isUploading = false;
        }
        protected void RemoveFile(TicketFileAttachmentModel file)
        {
            currTicket.TicketFileAttachmentModels.Remove(file);

            //remove existing file
            if (_action == Enums.CrudeMode.Edit)
                currTicket.RemovedFileAttachmentModels.Add(file);
            Extensions.ShowAlert("File " + file.FileName + " removed", Variant.Filled, snackbarService, Severity.Normal, string.Empty);
        }
        protected bool isAllowedSize(IBrowserFile file)
        {
            if (file.Size > maxAllowedSize)
            {
                Extensions.ShowAlert($"File -{file.Name}- size is too large.", Variant.Filled, snackbarService, Severity.Warning, string.Empty);
                return false;
            }
            return true;
        }
        protected bool isDuplicateFile(IBrowserFile file)
        {
            foreach(var images in currTicket.TicketFileAttachmentModels)
            {
                if (images.FileName == file.Name)
                {
                    Extensions.ShowAlert($"File -{file.Name}- is a duplicate file.", Variant.Filled, snackbarService, Severity.Warning, string.Empty);
                    return true;
                }
            }
            return false;
        }
        #endregion
    
        #region Extensions
        protected string ConverToUpperCase(string stringchar)
        {
            return Extensions.ConvertToUpperCase(stringchar);
        }

        protected async Task CopyTextToClipboard(string ticketNo)
        {
            await JSRuntime.InvokeAsync<object>("copyToClipboard", ticketNo);
            Extensions.ShowAlert("Copied to clipboard.", Variant.Filled, snackbarService, Severity.Normal, Icons.Material.Filled.ContentCopy);
        }
        #endregion
    
        protected async Task OpenDialog()
        {
            var parameters = new DialogParameters
            {
                { "DialogTitle", (_action == Enums.CrudeMode.Add ? "Create Ticket?" : "Update Ticket?") },
                { "IconString", Icons.Material.Filled.Add },
                { "CancelText", "Cancel" },
                { "SubmitText", (_action == Enums.CrudeMode.Add ? "Create ticket" : "Update Ticket") },
                { "ContentText", (_action == Enums.CrudeMode.Add ? "Create job request ticket." : "Updating ticket #") },
                { "DialogAction", _action },
                { "Ticket", currTicket }
            };
            var options = new DialogOptions 
            { 
                NoHeader = true,
                CloseOnEscapeKey = true,
                DisableBackdropClick = true
            };
            var resultDialog = await DialogService.Show<Shared.Components.Dialog>(string.Empty, parameters, options).Result;
            if (!resultDialog.Canceled) 
            {
                _processing = true;
                if (_action == Enums.CrudeMode.Add)
                    await SaveTicket();
                else if (_action == Enums.CrudeMode.Edit)
                    await UpdateTicket();
            }
        }
    }
}