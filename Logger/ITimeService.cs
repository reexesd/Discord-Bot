namespace Discord_Bot.Logger
{
    public interface ITimeService
    {
        public DateTime LocalTime { get; }

        public DateTime HomeTime { get; }
    }
}
