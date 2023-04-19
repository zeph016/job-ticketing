namespace fgciitjo.domain.clsAppstate
{
    public class ApplicationState
    {
        public bool IsDirty { get; set; }
        public bool _checkListCompState { get; set; }
        public string AppsStateStr = string.Empty;
        public bool isTListFilterOpen { get; set; }
        public event Action? OnChange;
        public async Task UpdateMainLayoutComponent(bool state)
        {
            IsDirty = state;
            await NotifyStateChangedAsync();
        }
        public void CLCompState(bool state)
        {
            _checkListCompState = state;
            NotifyStateChanged();
        }
        public async Task SetTListFilterState(bool value)
        {
            isTListFilterOpen = value;
            await NotifyStateChangedAsync();
        }
        private async Task NotifyStateChangedAsync() => await Task.Run(() => OnChange?.Invoke());
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}