using AgendaCalendar.Domain.Entities;
using _De_SerializationLib.Entities;

namespace _De_SerializationLib
{
    public static class JsonConverter
    {
        public static string GetJsonEventList(string calendarColor, IReadOnlyList<Event> events)
        {
            var eventList = new List<FullCalendarEvent>();
            foreach(var @event in events)
            {
                var fcEvent = new FullCalendarEvent()
                {
                    id = @event.Id.ToString(),
                    title = @event.Title,
                    description = @event.Description,
                    start = @event.StartTime,
                    end = @event.EndTime,
                    backgroundColor = calendarColor,
                    calendarId = @event.CalendarId.ToString(),
                    editable = true,
                };

                if (@event.ReccurenceRules.freq != "none")
                {
                    fcEvent.rrule = new RecurrenceRule()
                    {
                        freq = @event.ReccurenceRules.freq,
                        interval = @event.ReccurenceRules.interval,
                        byweekday = @event.ReccurenceRules.byweekday,
                        dtstart = @event.ReccurenceRules.dtstart,
                        until = @event.ReccurenceRules.until
                    };
                }
                else
                {
                    fcEvent.rrule = null;
                }
                Console.WriteLine(fcEvent);

                eventList.Add(fcEvent);
            }

            return System.Text.Json.JsonSerializer.Serialize(eventList);
        }

        public static List<Event> GetEventListFromJson(string json_input)
        {
            var jsonList = new List<FullCalendarEvent>();
            jsonList = System.Text.Json.JsonSerializer.Deserialize<List<FullCalendarEvent>>(json_input);

            List<Event> events = new List<Event>();
            foreach(var icalEvent in jsonList)
            {
                Event @event = new Event()
                {
                    Description = icalEvent.description,
                    Title = icalEvent.title,
                    StartTime = icalEvent.start,
                    EndTime = icalEvent.end,
                    Location = icalEvent.resourceId,
                };

                if(icalEvent.rrule is not null)
                {
                    @event.ReccurenceRules = new RecurrenceRule
                    {
                        freq = icalEvent.rrule.freq,
                        interval = icalEvent.rrule.interval,
                        byweekday = icalEvent.rrule.byweekday,
                        dtstart = icalEvent.rrule.dtstart,
                        until = icalEvent.rrule.until
                    };
                }

                events.Add(@event);
            }

            return events;
        }
    }
}
