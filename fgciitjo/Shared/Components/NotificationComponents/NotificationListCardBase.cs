namespace fgciitjo.Shared.Components.NotificationComponents
{
    public class NotificationListCardBase : ComponentBase
    {
        #region Inject Services
        [Inject] protected ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        #endregion
        #region  Properties
        [CascadingParameter] public AppStoreState ApplicationState { get; set; } = new();
        [Parameter] public EventCallback<bool> SetNotificationSetting { get; set; }
        [Parameter] public EventCallback CloseNotificationList { get; set; }
        private List<TicketComment> ticketComments = new List<TicketComment>();
        private Enums.NotificationViewType viewType = new();
        protected List<NotificationTrailModel> notificationList { get; set; } = new();
        protected bool dataFetched;
        
        #endregion

        protected override async Task OnInitializedAsync() 
        {
            await FilterNotificationList(Enums.NotificationViewType.All);
        }
        
        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }

        protected async Task FilterNotificationList(Enums.NotificationViewType notifViewType)
        {
            viewType = notifViewType;
            var list = await GetNotifications();
            if (list != null)
            {
                ApplicationState.Notifications = list;
                await ApplicationState.UpdateStoreState(); 
                await ReloadTableState();
                if (notifViewType == Enums.NotificationViewType.All)
                    notificationList = list.OrderByDescending(x => x.LogDateTime).ToList();
                else if (notifViewType == Enums.NotificationViewType.Read)
                    notificationList = list.Where(x => x.isRead).OrderByDescending(x => x.LogDateTime).ToList();
                else if (notifViewType == Enums.NotificationViewType.Unread)
                    notificationList = list.Where(x => x.isRead == false).OrderByDescending(x => x.LogDateTime).ToList();
            }
        }

        protected async Task SetNotificationConfig()
        {
            ApplicationState.IsNotificationSilent = !ApplicationState.IsNotificationSilent;
            await NotificationMethods.SetSilentNotification(LocalStorageService, ApplicationState.IsNotificationSilent);
            await ApplicationState.UpdateStoreState();
        }

        protected async Task MarkAllRead()
        {
            var list = await GetNotifications();
            if (list != null)
            {
                list.Select(x => { x.isRead = true; return x; }).ToList();
                ApplicationState.Notifications = list;
                await NotificationMethods.SetNotificationLocalStorage(LocalStorageService, list);
            }
            await CloseNotificationList.InvokeAsync();
        }

        private async Task UpdateSelectedNotification(long ticketId)
        {
            var list = notificationList;
            if (list != null)
            {
                var item = list.Where(x => x.TicketId == ticketId).FirstOrDefault();
                if (item != null && !item.isRead)
                {
                    item.isRead = true;
                    int index = list.FindIndex(x => x.TicketId == item.TicketId);
                    if (index != -1)
                        list[index] = item;
                    await NotificationMethods.SetNotificationLocalStorage(LocalStorageService, list);
                    list = ReloadTableWithoutState(list);
                    notificationList = list.OrderByDescending(x => x.LogDateTime).ToList();
                    await ApplicationState.UpdateStoreState();
                    StateHasChanged();
                }
            }
        }

        

        protected async Task ClearAllNotifications() 
        {
            ApplicationState.Notifications.Clear();
            notificationList.Clear();
            await NotificationMethods.ClearNotifications(LocalStorageService);
            await Refresh();
        }

        protected async Task ViewTicket(long ticketId) 
        {
            NavigationManager.NavigateTo($"/ticket/{ticketId}");
            await CloseNotificationList.InvokeAsync();
        }

        protected async Task PreviewTicket(long ticketId)
        {
            ViewedComments(ticketId);
            await UpdateSelectedNotification(ticketId);
            TicketModel ticket = new()
            {
                Id = ticketId
            };
            var parameters = new DialogParameters()
            {
                { "DialogTitle", "Job Description" },
                { "Ticket", ticket},
                { "CancelText", "Close" },
                { "IconString", @Icons.Material.Filled.BugReport }
            };
            var options = new DialogOptions()
            {
                CloseButton = false,
                MaxWidth = MaxWidth.Large,
                FullWidth = false,
                NoHeader = true,
                DisableBackdropClick = false
            };
            var resultDialog = await DialogService.Show<Shared.Dialogs.TicketList.PreviewTicketDialog>(string.Empty, parameters, options).Result;
            if (!resultDialog.Canceled)
                StateHasChanged();
            else
                StateHasChanged();
        }

        private void ViewedComments(long ticketId)
        {
            ticketComments.RemoveAll(x => x.TicketId == ticketId);
            GlobalList.TicketComments.RemoveAll(x => x.TicketId == ticketId);
        }

        protected async Task ReloadTableState()
        {
            dataFetched = true;
            notificationList.Clear();
            await Task.Delay(250);
            CompletedFetch();
        }
        private List<NotificationTrailModel> ReloadTableWithoutState(List<NotificationTrailModel> list)
        {
            if (viewType == Enums.NotificationViewType.Read)
                return list.Where(x => x.isRead).ToList();
            else if (viewType == Enums.NotificationViewType.Unread)
                return list.Where(x => x.isRead == false).ToList();
            else
                return list;
        }
        private async Task<List<NotificationTrailModel>> GetNotifications()
        {
            var list = await NotificationMethods.GetNotificationsFromLocalStorage(LocalStorageService);
            return list;
        }

        protected async Task Refresh()
        {
            dataFetched = false;
            await FilterNotificationList(viewType);
            CompletedFetch();
        }
    }
}