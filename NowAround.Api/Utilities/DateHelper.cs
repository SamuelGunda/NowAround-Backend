namespace NowAround.Api.Utilities;

public static class DateHelper
{
    public static void ValidateDate(string date)
    {
        if (!DateTime.TryParse(date, out _))
        {
            throw new ArgumentException("Invalid date format");
        }
        
        if (DateTime.Parse(date) > DateTime.Now)
        {
            throw new ArgumentException("Date cannot be in the future");
        }
    }
    
    public static List<string> GetMonthsInYear(string year)
    {
        var start = DateTime.Parse(year + "-01");
        var end = DateTime.Parse(year + "-12");
        var months = new List<string>();

        while (start <= end)
        {
            months.Add(start.ToString("yyyy-MM"));
            start = start.AddMonths(1);
        }

        return months;
    }
    
    public static (DateTime StartDate, DateTime EndDate) GetMonthStartAndEndDate(string date)
    {
        var year = date.Split('-').Select(int.Parse).ToArray()[0];
        var month = date.Split('-').Select(int.Parse).ToArray()[1];
        
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return (startDate, endDate);
    }
}