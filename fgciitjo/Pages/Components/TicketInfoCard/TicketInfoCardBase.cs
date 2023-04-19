namespace fgciitjo.Pages.Components.TicketInfoCard
{
    public class TicketInfoCardBase : ComponentBase
    {
        #region Inject Services
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        [Parameter] public TicketModel Ticket { get; set; } = new();
        #endregion

        protected async Task CopyTextToClipboard(string stringToCopy)
        {
            await JSRuntime.InvokeAsync<object>("copyToClipboard", stringToCopy);
            Extensions.ShowAlert("Copied to clipboard.", Variant.Filled, SnackbarService, Severity.Normal, Icons.Material.Filled.ContentCopy);
        }
    }
}