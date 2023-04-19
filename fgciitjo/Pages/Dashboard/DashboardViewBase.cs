namespace fgciitjo.Pages.Dashboard
{
    public class DashboardBase : ComponentBase
    {
        #region Inject Service
        [Inject] protected ITicketListService TicketListService { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;
        [Inject] protected ISnackbar SnackbarService { get; set; } = default!;
        #endregion
        #region Properties
        [CascadingParameter] public AppStoreState ApplicationState { get; set; } = new();
        protected bool dataFetched;
        protected TicketStatusModel selectedTicketStatus = new TicketStatusModel();
        protected Enums.PriorityLevel selectedPriorityLevel = Enums.PriorityLevel.Low;
        protected List<TicketModel> ticketList { get; set; } = new List<TicketModel>();
        protected List<TicketStatusModel> ticketStatusList = new List<TicketStatusModel>();
        private List<TicketCategoryModel> ticketCategoryList = new List<TicketCategoryModel>();
        protected List<TicketStatusCBModel> ticketStatusListCBList = new List<TicketStatusCBModel>();
        protected List<TicketCategoryModelCB> ticketCategoryModelCBList = new List<TicketCategoryModelCB>();
        protected List<Employee> itDeptEmpList = new List<Employee>();
        protected List<EmployeeCB> itDeptEmpListCB = new List<EmployeeCB>();
        private List<string> pieLabels = new List<string>();
        private List<double> pieData = new List<double>();
        protected List<ChartSeries> barChartSeries = new List<ChartSeries>();
        protected string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }, xAxisPieLabels = {};
        protected double[] pieDataX = {};
        protected long totalTickets, ticketStatusCount, ticketPriorityCount, ticketsForTheDayCount, totalHardwareCount, totalSoftwareCount, totalAdminCount;
        protected int index = -1;
        protected string totalTicketsTitle = string.Empty, styleStringBorderPriority = string.Empty, styleStringBackGPriority = string.Empty, ticketDateString = string.Empty;
        protected DateTime? selectedDate = DateTime.Today;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            GlobalContentTitle.contentTitle = "Dashboard";
            if (GlobalClass.CurrentUserAccount.AccessLevel != Enums.AccessLevel.Administrator)
                totalTicketsTitle = "Your Stats";
            else
                totalTicketsTitle = "Employee Stats";
        
            Task t = InitializeDashboardData();
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
                CompletedFetch();
        }

        private async Task InitializeDashboardData()
        {
            Task t = Task.WhenAll(LoadAllTickets(), LoadTicketStatus(), LoadTicketCategories(), AddPriorityLevelsToList(), GetITDept());
            await t;
            if (t.Status == TaskStatus.RanToCompletion)
            {
                CountTickets();
                CountTicketStatus(selectedTicketStatus);
                CountTicketPriority(selectedPriorityLevel);
                CountTicketsForTheDay(Convert.ToDateTime(selectedDate));
                CountTotalCategoryType();
                CountTicketsByPriority();
                Task u = Task.WhenAll(CountTicketsByMonths(), CountTicketsPerITEmployee());
                await u;
            }
        }
        private void CompletedFetch()
        {
            dataFetched = true;
            StateHasChanged();
        }

        private async Task LoadAllTickets()
        {
            FilterParameter filterParameter = new FilterParameter();
            filterParameter.TicketAccessLevel = GlobalClass.CurrentUserAccount.AccessLevel;
            filterParameter.TicketUserAccountId = GlobalClass.CurrentUserAccount.EmployeeId;
            filterParameter.isActive = true;
            ticketList = await TicketListService.LoadTicketv2(filterParameter, GlobalClass.Token);
            // ticketList = await TicketListService.LoadTicket(filterParameter, GlobalClass.Token);
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.IT)
                ticketList = ticketList.Where(x => x.AssigneeId == GlobalClass.CurrentUserAccount.EmployeeId).ToList();
            if (GlobalClass.CurrentUserAccount.AccessLevel == Enums.AccessLevel.Client)
                ticketList = ticketList.Where(x => x.RequestorId == GlobalClass.CurrentUserAccount.EmployeeId).ToList();

        }
        private async Task GetITDept()
        {
            while (GlobalList.ITDept == null)
                await Task.Delay(1);
            itDeptEmpList = GlobalList.ITDept;
        }
        private async Task LoadTicketStatus()
        {
            while (GlobalList.TicketStatusList == null)
                await Task.Delay(1);
            ticketStatusList = GlobalList.TicketStatusList;
            foreach (var items in ticketStatusList)
            {
                if (items.StatusName == "Done")
                {
                    selectedTicketStatus = items;
                    break;
                }
            }
            PopulateTicketStatusCB();
        }
        private async Task LoadTicketCategories()
        {
            while (GlobalList.ticketCategoryList == null)
                await Task.Delay(1);
            ticketCategoryList = GlobalList.ticketCategoryList;
        }
        private async Task AddPriorityLevelsToList()
        {
            await Task.Run(() =>
            {
                foreach (Enums.PriorityLevel items in Enum.GetValues(typeof(Enums.PriorityLevel)))
                {
                    pieLabels.Add(items.ToString());
                }
            });
            xAxisPieLabels = pieLabels.ToArray();
        }
        private void PopulateTicketStatusCB()
        {
            foreach (var item in ticketStatusList)
            {
                TicketStatusCBModel cbValue = new TicketStatusCBModel();
                cbValue.Id = item.Id;
                if (item.Id == 1 || item.Id == 2 || item.Id == 6 || item.Id == 8 || item.Id == 7)
                    cbValue.IsChecked = true;
                else
                    cbValue.IsChecked = false;
                cbValue.selectedTicketStatus = item;
                ticketStatusListCBList.Add(cbValue);
            }
        }
        private void CountTotalCategoryType()
        {
            foreach (var item in ticketCategoryList)
            {
                TicketCategoryModelCB tcatCB = new TicketCategoryModelCB();
                tcatCB.Id = item.Id;
                tcatCB.CategoryName = item.CategoryName;
                tcatCB.Count = ticketList.Where(x => x.TicketCategoryId == item.Id).ToList().Count();
                tcatCB.CategoryTypeId = item.CategoryTypeId;
                tcatCB.TicketCategory = item;
                ticketCategoryModelCBList.Add(tcatCB);

                if (item.CategoryTypeId == Enums.TicketCategoryType.Hardware)
                    totalHardwareCount += tcatCB.Count;
                if (item.CategoryTypeId == Enums.TicketCategoryType.Software)
                    totalSoftwareCount += tcatCB.Count;
                if (item.CategoryTypeId == Enums.TicketCategoryType.Admin)
                    totalAdminCount += tcatCB.Count;
            }
            ticketCategoryModelCBList = ticketCategoryModelCBList.OrderByDescending(x => x.Count).ToList();
        }
        private void CountTickets()
        {
            totalTickets = ticketList.Count();
            Console.WriteLine("Total Tickets: " + totalTickets);
        }
        protected void CountTicketStatus(TicketStatusModel ticketStatus)
        {
            selectedTicketStatus = ticketStatus;
            ticketStatusCount = ticketList.Where(x => x.TicketStatusName == selectedTicketStatus.StatusName).ToList().Count();
        }
        protected void CountTicketPriority(Enums.PriorityLevel ticketPriority)
        {
            selectedPriorityLevel = ticketPriority;
            ticketPriorityCount = ticketList.Where(x => x.PriorityLevelId == selectedPriorityLevel).ToList().Count();
            SelectTicketPriority(ticketPriority);
        }
        private void SelectTicketPriority(Enums.PriorityLevel ticketPriority)
        {
            if (ticketPriority == Enums.PriorityLevel.High)
            {
                styleStringBorderPriority = "border-color: #F44336;";
                styleStringBackGPriority = "background-color: #F44336;";
            }
            if (ticketPriority == Enums.PriorityLevel.Medium)
            {
                styleStringBorderPriority = "border-color: #FF9800;";
                styleStringBackGPriority = "background-color: #FF9800;";
            }

            if (ticketPriority == Enums.PriorityLevel.Low)
            {
                styleStringBorderPriority = "border-color: #2196F3";
                styleStringBackGPriority = "background-color: #2196F3;";
            }
        }
        protected void CountTicketsForTheDay(DateTime selectedDate)
        {
            DateTime startDay = selectedDate;
            DateTime endDay = startDay.AddDays(1).AddTicks(-1);
            ticketsForTheDayCount = ticketList.Where(x => x.TicketDate >= startDay && x.TicketDate <= endDay).ToList().Count();
            ticketDateString = startDay.ToString("MM/dd/yyyy");
        }
        private async Task CountTicketsByMonths()
        {
            barChartSeries = new List<ChartSeries>();
            foreach (var item in ticketStatusListCBList)
            {
                if (item.IsChecked)
                {
                    ChartSeries addSeries = new ChartSeries();
                    addSeries.Name = item.selectedTicketStatus.StatusName;
                    addSeries.Data = new double[]
                    {
                        await CountMonth(item.Id, 1), await CountMonth(item.Id, 4), await CountMonth(item.Id, 7), await CountMonth(item.Id, 10),
                        await CountMonth(item.Id, 2), await CountMonth(item.Id, 5), await CountMonth(item.Id, 8), await CountMonth(item.Id, 11),
                        await CountMonth(item.Id, 3), await CountMonth(item.Id, 6), await CountMonth(item.Id, 9), await CountMonth(item.Id, 12)
                    };
                    barChartSeries.Add(addSeries);
                }
            }
        }
        private void CountTicketsByPriority()
        {
            foreach (Enums.PriorityLevel items in Enum.GetValues(typeof(Enums.PriorityLevel)))
            {
                double count = ticketList.Where(x => x.PriorityLevelId == items).ToList().Count();
                pieData.Add(count);
            }
            pieDataX = pieData.ToArray();
        }
        private async Task CountTicketsPerITEmployee()
        {
            await Task.Run(() =>
            {
                foreach (var item in itDeptEmpList)
                {
                    EmployeeCB employee = new EmployeeCB();
                    employee.Id = item.EmployeeId;
                    employee.TicketCount = ticketList.Where(x => x.AssigneeId == item.EmployeeId).ToList().Count();
                    employee.EmployeeDetails = item;
                    itDeptEmpListCB.Add(employee);
                }
            });
            itDeptEmpListCB = itDeptEmpListCB.OrderByDescending(x => x.TicketCount).ToList();
        }
        protected async Task ViewBarChart()
        {
            var parameters = new DialogParameters();
            parameters.Add("buttonText", "Return");
            parameters.Add("ticketStatusList", ticketStatusList);
            parameters.Add("ticketList", ticketList);
            parameters.Add("ticketStatusListCBList", ticketStatusListCBList);
            parameters.Add("xAxisLabels", xAxisLabels);
            parameters.Add("barChartSeries", barChartSeries);
            parameters.Add("color", Color.Primary);

            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraExtraLarge,
                FullWidth = false,
                NoHeader = true
            };

            var resultDialog = await DialogService.Show<Shared.Dialogs.DashboardDialogs.BarChartDialog>("Ticket Summary Bar Chart",
            parameters, options).Result;
        }
        protected async Task ViewPieChart()
        {
            var parameters = new DialogParameters();
            parameters.Add("ButtonText", "Return");
            parameters.Add("xAxisPieLabels", xAxisPieLabels);
            parameters.Add("pieDataX", pieDataX);
            parameters.Add("Color", Color.Primary);

            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraExtraLarge,
                FullWidth = false,
                NoHeader = true
            };

            var resultDialog = await DialogService.Show<Shared.Dialogs.DashboardDialogs.PieChartDialog>("Ticket Priority Pie Chart",
            parameters, options).Result;
        }
        protected void RefreshBarChart()
        {
            var count = ticketStatusListCBList.Where(x => x.IsChecked == true).ToList().Count();
            if (count > 5)
                ShowAlert("You can only select up to 5 Ticket Status", Severity.Warning);
            else if (count == 0)
                ShowAlert("You must select a Ticket Status", Severity.Warning);
            else if (count <= 5 && count != 0)
                CountTicketsByMonths();
        }
        private void ShowAlert(string message, Severity severity)
        {
            SnackbarService.Clear();
            SnackbarService.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            SnackbarService.Configuration.SnackbarVariant = Variant.Filled;
            SnackbarService.Configuration.MaxDisplayedSnackbars = 10;
            SnackbarService.Add($"{message}", severity);
        }

        private async Task<double> CountMonth(long statusId, int monthNumber)
        {
            double count = 0;
            count = 
            await Task.Run(() => 
                ticketList.Where(x => Convert.ToDateTime(x.TicketDate).Month == monthNumber && x.TicketStatusId == statusId).ToList().Count()
            );
            return count;
        }
    }
}