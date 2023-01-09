namespace YomLog.BlazorShared.Extensions;

public static class DateTimeExtension
{
    public static string FormatDateTime(this DateTime utcDateTime)
    {
        var localDateTime = utcDateTime.ToLocalTime();
        var now = DateTime.Now;

        if (localDateTime.Date == now.Date)
            return $"Today {localDateTime:HH:mm}";
        else if (now.Date - localDateTime.Date == TimeSpan.FromDays(1))
            return $"Yesterday {localDateTime:HH:MM}";
        else if (now.Date - localDateTime.Date < TimeSpan.FromDays(7))
            return $"{localDateTime:dddd HH:MM}";
        else if (now.Year == localDateTime.Year)
            return $"{localDateTime:MM/dd HH:mm}";
        else
            return $"{localDateTime:yyyy/MM/dd HH:mm}";
    }
}
