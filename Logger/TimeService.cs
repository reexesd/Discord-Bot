namespace Discord_Bot.Logger
{
    public class TimeService : ITimeService
    { 
        public DateTime HomeTime 
        {
            get 
            {
                return GetRussiaCurrentTime();
            }
        }

        public DateTime LocalTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        private DateTime GetRussiaCurrentTime()
        {
            var targetLocation = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");

            var time = DateTime.Now;

            if (targetLocation != TimeZoneInfo.Local)
                TimeZoneInfo.ConvertTime(time, targetLocation);

            return time;
        }
    }
}
