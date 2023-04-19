namespace fgciitjo.Pages.Components.TicketList
{
    public class TListSubTaskCheckCompBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ISubTaskService SubTaskService { get; set; } = default!;
        [Inject] protected ITicketService TicketService { get; set; } = default!;
        #endregion
        #region Properties
        [Parameter] public long TicketId { get; set; }
        [Parameter] public string TicketNumber { get; set; } = string.Empty;
        protected TicketModel parentTicket = new();
        private long parentTicketId;
        protected bool dataFetched;
        protected string taskType = string.Empty, parentTicketNumber = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Task t = Task.WhenAll(LoadSubTickets());
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
            {
                Task<bool> CheckSubTicketTask = IsSubTask();
                await CheckSubTicketTask;
                if (CheckSubTicketTask.Status == TaskStatus.RanToCompletion)
                {
                    if(CheckSubTicketTask.Result)
                    {
                        taskType = "SUB TICKET";
                        Task parentTicketTask = GetParentTicketId();
                        await parentTicketTask;
                        if (parentTicketTask.Status == TaskStatus.RanToCompletion)
                            CompletedFetch();
                    }
                    else
                        CompletedFetch();
                }
            }
        }

        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }

        private async Task LoadSubTickets()
        {
            IEnumerable<TicketModel> data = await SubTaskService.SubTaskList(TicketId, GlobalClass.Token);
            foreach (var item in data)
                GlobalList.SubTaskList.Add(item);
            GlobalList.SubTaskList = await Task.Run(() =>  GlobalList.SubTaskList.Distinct().ToList());
        }

        private async Task<bool> IsSubTask()
        {
            var response = await Task.Run(() => GlobalList.SubTaskList.Where(x=>x.Id == TicketId).FirstOrDefault());
            if (response != null)
            {
                parentTicketId = response.TicketParentId;
                return true;
            }
            else
                return false;
        }

        private async Task GetParentTicketId()
        {
            var result = await TicketService.GetTicketById(parentTicketId, GlobalClass.Token);
            if (result != null)
                parentTicket = result;
            else
                parentTicket = new();
        }
    }
}