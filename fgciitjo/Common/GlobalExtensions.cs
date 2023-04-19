using System.ComponentModel;
using System.Reflection;
using System.IdentityModel.Tokens.Jwt;
using fgciitjo.service.UserAccountServices;


public static class Extensions
{
    public static string GetEnumDisplayName(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DisplayAttribute[] attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false); 
        if (attributes != null && attributes.Length > 0)  
            return attributes[0].Name;  
        else  
            return value.ToString();  
    }
    public static string GetEnumDescription(Enum value)  
    {  
        var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();  
        var descriptionAttribute =  
            enumMember == null  
                ? default(DescriptionAttribute)  
                : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;  
        return  
            descriptionAttribute == null  
                ? value.ToString()  
                : descriptionAttribute.Description;  
    }

    public static async Task<bool> CheckGlobalTokenV2(ILocalStorageService localStorageService)
    {
        GlobalClass.Token = await localStorageService.GetItemAsync<string>("token");
        if (!string.IsNullOrWhiteSpace(GlobalClass.Token))
            return true;
        return false;
    }

    public static async Task<bool> ValidateToken(IUserAccountService userAccountService, ILocalStorageService localStorageService, IJSRuntime jSRuntimeService)
    {
        bool isTokenExist = await CheckGlobalTokenV2(localStorageService) ? true : await GetSetToken(localStorageService, jSRuntimeService);
        bool isAuthExpired;
        if (isTokenExist)
        {
            isAuthExpired = IsTokenExpired(await localStorageService.GetItemAsync<string>("token"));
            if (!isAuthExpired)
            {
                return await AuthenticateToken(userAccountService, GlobalClass.Token);
            }
            else if (isAuthExpired)
            {
                await GetSetToken(localStorageService, jSRuntimeService);
                return await AuthenticateToken(userAccountService, GlobalClass.Token);
            }
        }
        return false;
    }

    public static async Task<bool> GetSetToken(ILocalStorageService localStorageService, IJSRuntime jSRuntimeService)
    {
        var cookieToken = await jSRuntimeService.InvokeAsync<string>("GetCookie", "token");
        if (!string.IsNullOrWhiteSpace(cookieToken))
        {
            await localStorageService.SetItemAsync<string>("token", cookieToken);
            GlobalClass.Token = await localStorageService.GetItemAsync<string>("token");
            await jSRuntimeService.InvokeAsync<string>("DeleteCookie");
            return true;
        }
        else if (string.IsNullOrWhiteSpace(cookieToken))
        {
            await ClearRemainingCookie(jSRuntimeService, localStorageService);
            return false;
        }
        return false;
    }

    public static bool IsTokenExpired(string token)
    {
        const int second = 1;
        const int minute = 60 * second;
        const int hour = 60 * minute;

        var handler = new JwtSecurityTokenHandler(); 
        var jwt = handler.ReadJwtToken(token);
        var name = jwt.Claims.First(claim => claim.Type == "unique_name").Value;
        var userId = jwt.Claims.First(claim => claim.Type == "EmployeeId").Value;
        var tokenDate = jwt.Claims.First(claim => claim.Type == "exp").Value;
        DateTime expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenDate)).DateTime;
        
        // if (DateTimeOffset.Now > expirationTime)
        // {
        //     Console.WriteLine("Token expired");
        //     return true;
        // }
        var ts = new TimeSpan(DateTime.Now.Ticks - expirationTime.Ticks);
        double delta = Math.Abs(ts.TotalSeconds);
        if (delta > 8 * hour)
            return true;
        return false;
    }

    public static async Task ClearRemainingCookie(IJSRuntime jsRuntimeService, ILocalStorageService localStorageService)
    {
        await jsRuntimeService.InvokeAsync<string>("DeleteCookie");
        await localStorageService.RemoveItemAsync("token");
    }

    public static async Task<bool> AuthenticateToken(IUserAccountService userAccountService, string token)
    {
        UserAccount userAccount = await userAccountService.AuthenticateToken(new UserAccount(), token);
        GlobalClass.CurrentUserAccount = userAccount ?? new UserAccount();
        Console.WriteLine(GlobalClass.CurrentUserAccount.httpResponse);
        if (GlobalClass.CurrentUserAccount.httpResponse == System.Net.HttpStatusCode.OK 
        && GlobalClass.CurrentUserAccount.httpResponse != System.Net.HttpStatusCode.Unauthorized)
            return true;
        return false;
    }

    public static void ShowAlert(string message, Variant variant, ISnackbar snackbarService, Severity severityType, string iconString)
    {
        snackbarService.Clear();
        snackbarService.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
        snackbarService.Configuration.SnackbarVariant = variant;
        snackbarService.Configuration.MaxDisplayedSnackbars = 10;
        snackbarService.Configuration.VisibleStateDuration = 2000;
        snackbarService.Add($"{message}", severityType, config => 
        {
            if (!string.IsNullOrWhiteSpace(iconString)) config.Icon = iconString;
        });
    }
    public static void ShowAlertV2(string message, Variant variant, ISnackbar snackbarService, Severity severityType, string iconString, string position)
    {
        snackbarService.Clear();
        snackbarService.Configuration.PositionClass = position;
        snackbarService.Configuration.SnackbarVariant = variant;
        snackbarService.Configuration.MaxDisplayedSnackbars = 10;
        snackbarService.Configuration.VisibleStateDuration = 2000;
        snackbarService.Add($"{message}", severityType, config => 
        {
            if (!string.IsNullOrWhiteSpace(iconString)) config.Icon = iconString;
        });
    }

    public static string GetColor(long statusId)
    {
        var ticketStatus = GlobalList.TicketStatusList.Where(x =>x.Id == statusId).FirstOrDefault();
        if (ticketStatus != null)
            return "background-color:" + ticketStatus.StatusColor + ";color:white";
        return string.Empty;
    }

    public static string DetermineFileIcon(string fileName)
    {
        if(fileName.Contains(".png") || fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
            return "fa-solid fa-file-image";
        else if(fileName.Contains(".doc") || fileName.Contains(".docx"))
            return "fa-solid fa-file-word";
        else if(fileName.Contains(".xls") || fileName.Contains(".xlsx"))
            return "fa-solid fa-file-excel";
        else if(fileName.Contains(".csv"))
            return "fa-solid fa-file-csv";
        else if(fileName.Contains(".pdf"))
            return "fa-solid fa-file-pdf";
        return "fa-solid fa-file";
    }

    public static string DetermineFileColor(string fileName)
    {
        if(fileName.Contains(".png") || fileName.Contains(".jpg") || fileName.Contains(".jpeg"))
            return "color:#D73D6F";
        else if(fileName.Contains(".doc") || fileName.Contains(".docx"))
            return "color:#112D7C";
        else if(fileName.Contains(".xls") || fileName.Contains(".xlsx"))
            return "color:#037341";
        else if(fileName.Contains(".csv"))
            return "color:#307F5A";
        else if(fileName.Contains(".pdf"))
            return "color:#CE1C26";
        return "";
    }

    public static async Task<Enums.TicketCategoryType> GetCategoryTypeId(long tCatID)
    {
        while (GlobalList.ticketCategoryList == null)
            await Task.Delay(1);
        var result = await Task.Run(() => GlobalList.ticketCategoryList.Where(x=>x.Id == tCatID).FirstOrDefault());
        if (result != null)
            return result.CategoryTypeId;
        else
            return 0;
    }

    public static double CalculateFileSize(string fileString)
    {
        double stringLength = fileString.Length;
        double sizeInBytes = 4 * Math.Ceiling((stringLength / 3))*0.5624896334383812;
        var finalSize = sizeInBytes/1000;
        double sizeInKb = Convert.ToDouble(finalSize);
        return sizeInKb;
    }

    public static string ConvertToUpperCase(string stringChar)
    {
        return stringChar = (!string.IsNullOrWhiteSpace(stringChar) ? Convert.ToString(char.ToUpper(stringChar[0]) + stringChar.Substring(1)) : string.Empty);
    }

    public static string GetPriorityEnumColor(Enums.PriorityLevel priorityLevel)
    {
        if(priorityLevel == Enums.PriorityLevel.Low)
            return "bg-material-info color-white";
        else if(priorityLevel == Enums.PriorityLevel.Medium)
            return "bg-material-warning color-white";
        else if (priorityLevel == Enums.PriorityLevel.High)
            return "bg-material-error color-white";
        else
            return "bg-material-normal";
    }

    public static async Task<string> DetermineTimeDifference(TicketModel ticket)
    {
        string timeDiff = string.Empty;
        DateTime dateNow = GlobalContentTitle.contentServerTime;
        TimeSpan diff = dateNow - Convert.ToDateTime(ticket.TicketDate);
        TimeSpan seconds = new TimeSpan(0, 0, 59);
        TimeSpan minuteOne = new TimeSpan(0, 01, 0);
        TimeSpan minuteTwo = new TimeSpan(0, 02, 0);
        TimeSpan minutes = new TimeSpan(0, 59, 0);
        TimeSpan hourOne = new TimeSpan(1, 0, 0);
        TimeSpan hourTwo = new TimeSpan(2, 0, 0);
        TimeSpan dayOne = new TimeSpan(24, 0, 0);
        TimeSpan dayTwo = new TimeSpan(48, 0, 0);
        TimeSpan weekOne = new TimeSpan(168, 0, 0);
        TimeSpan weekTwo = new TimeSpan(336, 0, 0);
        TimeSpan monthOne = new TimeSpan(730, 0, 0);
        TimeSpan monthOneTwo = new TimeSpan(1095, 0, 0);
        TimeSpan monthTwo = new TimeSpan(1460, 0, 0);
        TimeSpan monthYearOne = new TimeSpan(8760, 0, 0);
        TimeSpan monthYearTwo = new TimeSpan(17520, 0, 0);

        return await Task.Run(string () =>
        {
            if (diff <= seconds)
                return "a moment ago";
            else if (diff >= minuteOne && diff <= minuteTwo)
                return "a minute ago";
            else if (diff > minuteTwo && diff < minutes)
                return $"{diff.Minutes} minutes ago";
            else if (diff <= hourOne)
                return $"{diff.Hours} an hour ago";
            else if (diff >= hourOne && diff <= hourTwo)
                return $"{diff.Hours} hour ago";
            else if (diff > hourTwo && diff < dayOne)
                return $"{diff.Hours} hours ago";
            else if (diff >= dayOne && diff < dayTwo)
                return $"{diff.Days} day ago";
            else if (diff > dayTwo && diff < weekOne)
                return $"{diff.Days} days ago";
            else if (diff <= weekOne)
            {
                var week = diff / weekOne;
                return $"{Math.Round(week)} week ago";
            }
            else if (diff <= weekTwo)
            {
                var week = diff / weekOne;
                return $"{Math.Round(week)} weeks ago";
            }
            else if (diff > weekTwo && diff < monthOne)
            {
                var weeksPassed = diff / weekOne;
                return $"{Math.Round(weeksPassed)} weeks ago";
            }
            else if (diff >= monthOne && diff < monthOneTwo)
            {
                var month = diff / monthOne;
                return $"{Math.Round(month)} month ago";
            }
            else if (diff > monthOneTwo && diff < monthYearOne)
            {
                var months = diff / monthOne;
                return $"{Math.Round(months)} months ago";
            }
            else if (diff >= monthYearOne && diff < monthYearTwo)
            {
                var year = diff / monthYearOne;
                return $"{Math.Round(year)} year ago";
            }
            else if (diff > monthYearTwo)
            {
                var years = diff / monthYearOne;
                return $"{Math.Round(years)} years ago";
            }
            else
                return string.Empty;
        });
    }

    public static string GetStatusExplanation(string statusName)
    {
        if (statusName.Contains("For IT Approval", StringComparison.InvariantCultureIgnoreCase))
            return "Issue not yet checked, approved for work or IT personnel is still busy. Please kindly wait.";
        else if (statusName.Contains("In Progress", StringComparison.InvariantCultureIgnoreCase))
            return "Issue is currently being worked on. Kindly monitor your Spark Messenger for updates from assigned IT.";
        else if (statusName.Contains("Pending", StringComparison.InvariantCultureIgnoreCase))
            return "There is a pending or required issue. Check Spark Messenger for inquiries from assigned IT.";
        else if (statusName.Contains("Assigned", StringComparison.InvariantCultureIgnoreCase))
            return "Issue is assigned but not yet being worked on. Please kindly wait for assigned IT to follow-up.";
        else if (statusName.Contains("Done", StringComparison.InvariantCultureIgnoreCase))
            return "Issue has been completed.";
        else if (statusName.Contains("Cancelled", StringComparison.InvariantCultureIgnoreCase))
            return "Issue has been cancelled. Kindly check Spark Messenger for updates or contact IT.";
        else if (statusName.Contains("For Assignment", StringComparison.InvariantCultureIgnoreCase))
            return "Issue has been approved for work and is pending for IT assignment.";
        else if (statusName.Contains("For QA", StringComparison.InvariantCultureIgnoreCase))
            return "Issue has been completed and is now being checked by Quality Assurance.";
        return string.Empty;
    }

    public static async Task SetDarkMode(ILocalStorageService localStorageService, bool value) => await localStorageService.SetItemAsync("darkmode", value);
    public static async Task<bool> GetDarkSetting(ILocalStorageService localStorageService)
    {
        var result = await localStorageService.GetItemAsync<bool>("darkmode");
        if (result != false)
            return result;
        else
        {
            await SetDarkMode(localStorageService, false);
            return false;
        }
        
    }
}