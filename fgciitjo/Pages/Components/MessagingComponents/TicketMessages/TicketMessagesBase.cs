namespace fgciitjo.Pages.Components.MessagingComponents.TicketMessages
{
    public class TicketMessagesBase : ComponentBase
    {
        #region Inject Services
        [Inject] protected IJSRuntime JSRuntime { get; set;} = default!;
        #endregion
        #region Properties
        [CascadingParameter] protected AppStoreState ApplicationState { get; set; } = new();
        [Parameter] public EventCallback<string> SendCommentClick { get; set; }
        [Parameter] public long SenderId { get; set; }
        [Parameter] public bool IsSendingMessage { get; set; }
        [Parameter] public List<TicketComment> TicketMessages { get; set; } = new List<TicketComment>();
        [Parameter] public string MessageToSend { get; set; } = string.Empty;
        protected bool dataFetched;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await ScrollMessageContent("msg-content-container-id");
        }

        protected async Task SendComment() => await SendCommentClick.InvokeAsync(MessageToSend);

        protected void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
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
         protected async Task ScrollToBottom(string elementId) => await JSRuntime.InvokeVoidAsync("ScrollToBottomComment", elementId);
    }
}