
namespace _De_SerializationLib.Entities
{
    public class RRule
    {
        public string freq { get; set; }
        public int interval { get; set; }
        public List<string> byweekday { get; set; }
        public DateTime dtstart { get; set; }
        public DateTime untill { get; set; }
        public List<int> daysOfWeek { get; set; }

    }
}
