using System;

public static class TimeSpanExtensions
{
	public static string FormatRaceTime(this TimeSpan TS)
	{
		if (TS.TotalMilliseconds <= 0.0)
		{
			return "--:--.--";
		}
		return string.Format("{0:00}:{1:00}.{2:00}", TS.Minutes, TS.Seconds, TS.Milliseconds / 10);
	}
}
