using AgendaCalendar.Domain.Entities;

namespace _De_SerializationLib.Entities
{
    public class FullCalendarEvent
    {
        public string id { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string resourceId { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string calendarId { get; set; }
        public string backgroundColor { get; set; }
        public bool editable { get; set; }
        public RecurrenceRule rrule { get; set; } = new();
    }
}
