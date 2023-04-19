public static class NotificationMethods
{
    public static async Task<bool> CheckSilentSetting(ILocalStorageService localStorageService)
    {
        return await localStorageService.GetItemAsync<bool>("notif-silent");
    }
    public static async Task SetNotificationLocalStorage(ILocalStorageService localStorageService, List<NotificationTrailModel> notificationList)
    {
        await localStorageService.SetItemAsync("notifications", notificationList);
    }
    public static async Task<List<NotificationTrailModel>> GetNotificationsFromLocalStorage(ILocalStorageService localStorageService)
    {
        return await localStorageService.GetItemAsync<List<NotificationTrailModel>>("notifications");
    }
    public static async Task ClearNotifications(ILocalStorageService localStorageService) =>  await localStorageService.RemoveItemAsync("notifications");
    public static async Task SetSilentNotification(ILocalStorageService localStorageService, bool value)
    {
        await localStorageService.SetItemAsync("notif-silent", value);
    }

    public static void ShowNotificationSnackbar(ISnackbar snackbar, TicketModel ticket, 
        NavigationManager navigationManager, UserAccount employee,
        ILocalStorageService localStorageService)
    {
        snackbar.Configuration.VisibleStateDuration = 15000;
        snackbar.Configuration.ShowCloseIcon = true;
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomStart;
        snackbar.Configuration.SnackbarVariant = Variant.Filled;
        snackbar.Add<NotificationCard>(new Dictionary<string, object>() {
            { "Ticket", ticket },
            { "Employee", employee }
        }, Severity.Normal, config => {
            config.HideIcon = true;
            config.Onclick = snackbar =>
            {
                // notification.IsSeen = true;
                // Task.Run( async ()=> await NavigateToDocument(notification,navigationManager,localStorageService));
                navigationManager.NavigateTo($"/ticket/{ticket.Id}");
                return Task.CompletedTask;
            };
        });
    }
}