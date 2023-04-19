namespace fgciitjo.Shared.Dialogs.TicketList
{
    public class PreviewTicketDialogBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected IEmployeeAccountService EmployeeAccountService { get; set; } = default!;
        [Inject] protected ITicketService TicketService { get; set; } = default!;
        [Inject] protected ITicketCommentService TicketCommentService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set;} = default!;
        [Inject] protected IConfiguration Config { get; set; } = default!;
        [Inject] protected Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;
        #endregion

        #region Properties
        [CascadingParameter] MudDialogInstance MudDialog { get; set; } = new();
        [Parameter] public TicketModel Ticket { get; set; } = new();
        [Parameter] public string IconString { get; set; } = string.Empty; 
        [Parameter] public string DialogTitle { get; set; } = string.Empty; 
        [Parameter] public string CancelText { get; set; } = string.Empty;
        protected bool dataFetched, isAttachmentsOpen, isMessagesOpen, isSending;
        protected TicketComment ticketMessage = new();
        protected UserAccount requestorAccount = new(), assigneeAccount = new();
        protected List<TicketComment> ticketMessages { get; set; } = new List<TicketComment>();
        protected List<TicketFileAttachmentModel> FilesAttachedList = new List<TicketFileAttachmentModel>();
        protected AppStoreState ApplicationState { get; set; } = new();
        protected int commentCount;
        protected HubConnection hubConnection = default!;
        protected string currentTimeDifference = string.Empty, filesHashStr = string.Empty, messageToSend = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Task t = GetTicket();
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
            {
                Task<UserAccount> GetRequestor = GetUserAccountInfo(Ticket.RequestorId);
                Task<UserAccount> GetAssignee = GetUserAccountInfo(Convert.ToInt64(Ticket.AssigneeId));
                Task u = Task.WhenAll(LoadComment(), LoadSignalR(), GetRequestor, GetAssignee);
                await u;
                if (u.Status == TaskStatus.RanToCompletion)
                {
                    currentTimeDifference = await Extensions.DetermineTimeDifference(Ticket);
                    MapFilesAttached();
                    requestorAccount = GetRequestor.Result;
                    assigneeAccount = GetAssignee.Result;
                    CompletedFetch();
                }
            }
        }

        protected void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }
        protected void Cancel() => MudDialog.Close(DialogResult.Ok(true));

        protected async Task GetTicket()
        {
            var response = await TicketService.GetTicketById(Ticket.Id, GlobalClass.Token);
            if (response != null)
                Ticket = response;
        }

        protected async Task LoadComment()
        {
            ticketMessage.Token = GlobalClass.Token;
            ticketMessage.TicketId = Ticket.Id;
            var result = await TicketCommentService.LoadTicketComments(ticketMessage);
            if (result != null)
            {
                ticketMessages = result; 
                // ApplicationState.TicketMessages = result; 
                commentCount = CountComments();
            }
            
        }

        protected async Task LoadSignalR()
        {
            hubConnection = new HubConnectionBuilder()
            .WithUrl(Config["SignalRConnection"] + "ticketcommenthub")
            .WithAutomaticReconnect()
            .Build();
            await hubConnection.StartAsync();

            hubConnection.On<TicketComment>("ReceiveTicketComment", (response) =>
            {
                ticketMessages.Add(response);
                ApplicationState.TicketMessages.Add(response);
                messageToSend = string.Empty;
                StateHasChanged();
                Task.Delay(1);
            });
        }

        private async Task<UserAccount> GetUserAccountInfo(long userId)
        {
            if (userId != 0)
            {
                var result = await EmployeeAccountService.GetEmployee(GlobalClass.Token, userId);
                if (result != null)
                    return result;
                else 
                    return new();
            }
            else
                return new();

        }

        protected async Task CopyTextToClipboard(string text)
        {
            await JSRuntime.InvokeAsync<object>("copyToClipboard", text);
            Extensions.ShowAlert("Copied to clipboard.", Variant.Filled, SnackbarService, Severity.Normal, Icons.Material.Filled.ContentCopy);
        }

        protected int CountComments()
        {
            return ticketMessages.Where(x => x.TicketId == Ticket.Id).ToList().Count();
        }

        protected async void SendComment(string message)
        {
            isSending = true;
            TicketComment ticketComment = new TicketComment()
            {
                Token = GlobalClass.Token,
                TicketId = Ticket.Id,
                DateTimeLog = DateTime.Now,
                UserId = GlobalClass.CurrentUserAccount.EmployeeId,
                IsActive = true,
                Comment = message
            };
            if (!string.IsNullOrWhiteSpace(ticketComment.Comment))
            {
                Console.WriteLine("invoke hub");
                await hubConnection.InvokeAsync("ReceiveTicketComment", ticketComment);
                isSending = false;
                await ScrollMessageContent("msg-content-container-id");
            }
            else
            {
                isSending = false;
                Console.WriteLine("hub not working");
            }
            StateHasChanged();
        }

        protected async Task ScrollToBottom(string elementId) => await JSRuntime.InvokeVoidAsync("ScrollToBottomComment", elementId);
        private void MapFilesAttached()
        {
            string filesToHash = "";
            foreach (TicketFileAttachmentModel items in Ticket.TicketFileAttachmentModels)
            {
                filesToHash += items.FileName;
                if (items.IsActive)
                {
                    FilesAttachedList.Add(items);
                }
                using (SHA512 sha512Hash = SHA512.Create())
                {
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(filesToHash);
                    byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                    filesHashStr = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                }
            }
        }

        protected async Task ScrollMessageContent(string elementId)
        {
            await Task.Delay(200);
            Task<bool> ScrollBarCheck = CheckScrollBar(elementId);
            await ScrollBarCheck;
            if (ScrollBarCheck.Status == TaskStatus.RanToCompletion)
            {
                if (ScrollBarCheck.Result)
                    await ScrollToBottom(elementId);
            }
        }

        protected async Task<bool> CheckScrollBar(string elementId)
        {
            var CheckScrollBar = JSRuntime.InvokeAsync<bool>("CheckScrollbar", elementId);
            await CheckScrollBar;
            return CheckScrollBar.Result;
        }
    }
}