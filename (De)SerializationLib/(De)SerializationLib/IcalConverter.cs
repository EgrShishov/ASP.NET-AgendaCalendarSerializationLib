using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using AgendaCalendar.Domain.Entities;
using Calendar = AgendaCalendar.Domain.Entities.Calendar;
using IcalCalendar = Ical.Net.Calendar;
using Ical.Net.CalendarComponents;
using AgendaCalendar.Domain.Abstractions;
using System.Text;

namespace _De_SerializationLib
{
    public static class IcalConverter
    {
        public static string Serialize(object obj) 
        {
            if (obj is Calendar)
            {
                Calendar calendar = ((Calendar)obj);
                IcalCalendar ical = new IcalCalendar();
                foreach (var calendarEvent in calendar.Events)
                {
                    ical.Events.Add(new CalendarEvent()
                    {
                        Summary = calendarEvent.Title,
                        Start = new CalDateTime(calendarEvent.StartTime),
                        End = new CalDateTime(calendarEvent.EndTime),
                        Description = calendarEvent.Description,
                        Uid = calendarEvent.Id.ToString(),
                        Organizer = new Organizer("email@yandex.ru")
                    });
                }
                ical.Name = calendar.Title;
                var serializer = new CalendarSerializer();
                var serializedCalendar = serializer.SerializeToString(ical);
                return serializedCalendar;
            }
            else
            {
                throw new ArgumentException("Object is not of type AgendaCalendar", nameof(obj));
            }
        }
        public static Calendar Deserialize(string ical_format)
        {
            var deserializer = new CalendarSerializer();
            Console.WriteLine(ical_format);
            using Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(ical_format ?? ""));
            IcalCalendar ical = (IcalCalendar)deserializer.Deserialize(stream, System.Text.Encoding.UTF8);

            if (ical == null) throw new ArgumentException("Error parsing ical format. Calendar should not be empty!"); //doesnt work

            var title = ical.Name;
            var events = new List<IEvent>();
            var reminders = new List<Reminder>();
            foreach(var IcalEvent in ical.Events)
            {
                var rec_id = IcalEvent.RecurrenceId;
                var rec_rules = IcalEvent.RecurrenceRules;
                var rec_dates = IcalEvent.RecurrenceDates;
                events.Add(new Event(
                    IcalEvent.Name, Convert.ToDateTime(IcalEvent.Start), Convert.ToDateTime(IcalEvent.End), IcalEvent.Description, -1));
            }
            var parsed_calendar = new Calendar(title, string.Empty, -1, events, reminders);
            return parsed_calendar;
        }
    }
}
