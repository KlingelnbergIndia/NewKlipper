namespace UseCaseBoundary.Model
{
    public class Time
    {
        public int Hour;
        public int _minute;
        public Time(int hours, int minutes)
        {
            Hour = hours;
            _minute = minutes;
        }
    }
}
