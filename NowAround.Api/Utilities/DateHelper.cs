using NowAround.Api.Utilities.Interface;

namespace NowAround.Api.Utilities;

public class DateHelper : IDateHelper
{
    public List<string> GetMonthsInYear(string year)
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
    
    public (DateTime StartDate, DateTime EndDate) GetMonthStartAndEndDate(string date)
    {
        var year = date.Split('-').Select(int.Parse).ToArray()[0];
        var month = date.Split('-').Select(int.Parse).ToArray()[1];
        
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return (startDate, endDate);
    }
}