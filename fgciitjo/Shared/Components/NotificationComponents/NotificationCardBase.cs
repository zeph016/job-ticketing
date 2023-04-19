namespace fgciitjo.Shared.Components.NotificationComponents
{
    public class NotificationCardBase : ComponentBase
    {
        #region Inject Service

        #endregion
        #region Properties
        [Parameter] public TicketModel Ticket { get; set; } = new();
        [Parameter] public UserAccount Employee { get; set; } = new();
        #endregion
    }
}