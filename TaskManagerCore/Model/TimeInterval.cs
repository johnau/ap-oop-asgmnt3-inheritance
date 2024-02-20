namespace TaskManagerCore.Model
{
    public enum TimeInterval
    {
        None = 0,
        Hourly = 1,
        Daily = 24,
        Weekly = 24*7,
        Fortnightly = 24*7*2,
        Monthly = 24*7*30,
        Yearly = 24*7*365,
    }
}
