namespace fgciitjo.Store
{
    public class AppStoreState
    {
        public event Action? OnChange;
        public bool IsNotificationSilent;
        public List<TicketComment> TicketMessages = new();
        public List<NotificationTrailModel> Notifications = new();
        public List<UserAccount> UserAccounts = new();
        private async Task NotifyStateChangedAsync() => await Task.Run(() => OnChange?.Invoke());
        private void NotifyStateChanged() => OnChange?.Invoke();
        public async Task UpdateStoreState() => await NotifyStateChangedAsync();
        
    }
}