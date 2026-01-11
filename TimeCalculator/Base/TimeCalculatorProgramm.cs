using TimeCalculator.Base.Types;

namespace TimeCalculator.Base;

public class TimeCalculatorProgramm
{
    public List<TimeEntry> TimeEntries = [];

    public TimeData TotalTime = new();
    public TimeData TotalWorkTime = new();
    public TimeData TotalTimeLeftToWork => GetTimeLeftToWork();
    public TimeType SelectedTimeType = default;
    public TimeData CurrentTimeEntry = new();

    public int DailyWorkHours = 0;

    public void RemoveTimeEntry(TimeEntry timeEntry)
    {
        TimeEntries.Remove(timeEntry);
        CalculateTotalTime();
    }

    public void SetHours(TimeData timeEntry, int hours)
    {
        timeEntry.Hours = hours;
    }

    public void SetMinutes(TimeData timeEntry, int minutes)
    {
        timeEntry.Minutes = minutes;
    }

    public void SetSeconds(TimeData timeEntry, int seconds)
    {
        timeEntry.Seconds = seconds;
    }

    public void AddTimeEntry()
    {
        TimeEntries.Add(
            new()
            {
                Time = new()
                {
                    Hours = CurrentTimeEntry.Hours,
                    Minutes = CurrentTimeEntry.Minutes,
                    Seconds = CurrentTimeEntry.Seconds,
                },
                Type = SelectedTimeType,
            }
        );

        SelectedTimeType = default;
        CalculateTotalTime();
    }

    public void CalculateTotalTime()
    {
        var workTime = TimeEntries.Where(timeEntry => timeEntry.Type == TimeType.Work).ToList();

        TotalTime = SumTimeEntries(TimeEntries);
        TotalWorkTime = SumTimeEntries(workTime);

        CurrentTimeEntry =
            SelectedTimeType == TimeType.Work
                ? TotalTimeLeftToWork.NegativeClone()
                : new TimeData();
    }

    private TimeData GetTimeLeftToWork()
    {
        var timeLeft = TotalWorkTime.Clone();
        timeLeft -= new TimeData { Hours = DailyWorkHours };
        return timeLeft;
    }

    private TimeData SumTimeEntries(List<TimeEntry> entries)
    {
        int totalSeconds = 0;

        foreach (var entry in entries)
        {
            totalSeconds += entry.Time.Hours * 3600 + entry.Time.Minutes * 60 + entry.Time.Seconds;
        }

        return GetTimeDataFromSeconds(totalSeconds);
    }

    private TimeData GetTimeDataFromSeconds(int totalSeconds)
    {
        return new TimeData
        {
            Hours = totalSeconds / 3600,
            Minutes = totalSeconds % 3600 / 60,
            Seconds = totalSeconds % 60,
        };
    }
}
