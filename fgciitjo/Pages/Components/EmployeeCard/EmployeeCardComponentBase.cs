namespace fgciitjo.Pages.Components.EmployeeCard
{
    public class EmployeeCardComponentBase : ComponentBase
    {
        #region Inject Services
        [Inject] protected IGlobalService GlobalService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        [Parameter] public bool isITEmployee { get; set; }
        [Parameter] public string employeeName { get; set; } = string.Empty;
        [Parameter] public long employeeId { get; set; }
        [Parameter] public long? employeeITId { get; set; }
        protected bool dataFetched;
        protected UserAccount employeeDetails = new UserAccount();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (GlobalList.ITDept == null)
                await Task.Delay(1);

            Task t = (isITEmployee ? GetFromITDept() : GetEmployeeDetails());
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
                CompeletedFetch();
        }

        private void CompeletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }

        private async Task GetEmployeeDetails()
        {
            var response = await Task.Run(() => GlobalList.TemporaryEmployeeList.Where(x=>x.EmployeeId == employeeId).FirstOrDefault());
            if (response != null)
                employeeDetails = response;
            else
                response = await GlobalService.GetEmployeeV2(GlobalClass.Token, employeeId);
                if (response != null)
                {
                    GlobalList.TemporaryEmployeeList.Add(response);
                    employeeDetails = response;
                }
        }

        private async Task GetFromITDept()
        {
            Console.WriteLine(GlobalList.ITDept.Count());
            var result = await Task.Run(() =>  GlobalList.ITDept.Where(x=>x.EmployeeId == employeeITId).SingleOrDefault());
            if (result != null)
            {
                employeeDetails.EmployeeId = result.EmployeeId;
                employeeDetails.FirstName = result.FirstName;
                employeeDetails.LastName = result.LastName;
                employeeDetails.NameExtension = result.NameExtension;
                employeeDetails.Designation = result.Designation;
                employeeDetails.Picture = result.Picture;
            }
            else
            {
                employeeDetails.FirstName = "Not yet assigned";
                employeeDetails.LastName = string.Empty;
                employeeDetails.Designation = string.Empty;
                employeeDetails.Picture = new byte[]{};
            }
        }
    }
}