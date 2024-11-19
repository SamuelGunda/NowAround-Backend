namespace NowAround.Api.Utilities.Interface;

public interface IDateHelper
{
    List<string> GetMonthsInYear(string year);
    (DateTime StartDate, DateTime EndDate) GetMonthStartAndEndDate(string date);
}