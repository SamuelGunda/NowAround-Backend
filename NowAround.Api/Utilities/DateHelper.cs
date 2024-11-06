namespace NowAround.Api.Utilities;

public static class DateHelper
{
    public static (DateTime StartDate, DateTime EndDate) GetMonthStartAndEndDate(string date)
        {
            var year = date.Split('-').Select(int.Parse).ToArray()[0];
            var month = date.Split('-').Select(int.Parse).ToArray()[1];
            
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
        }
}