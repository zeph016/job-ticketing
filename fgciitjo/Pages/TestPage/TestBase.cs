namespace fgciitjo.Pages.TestPage
{
    public class TestBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        #endregion
        #region Properties
        protected List<NotificationTrailModel> NotificationList = new();
        #endregion

        protected async Task Test()
        {
            await NotificationMethods.SetNotificationLocalStorage(LocalStorageService, new());
        }

        protected async Task GetList()
        {
            var List = await NotificationMethods.GetNotificationsFromLocalStorage(LocalStorageService);
            if (List != null)
                NotificationList = List;
            else
                NotificationList = new();
            StateHasChanged();
        }
    }
}