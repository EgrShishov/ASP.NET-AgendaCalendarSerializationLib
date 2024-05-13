using AgendaCalendar.Domain.Entities;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Calendar = AgendaCalendar.Domain.Entities.Calendar;
using IcalCalendar = Ical.Net.Calendar;

namespace _De_SerializationLib
{
    public static class IcalConverter
    {
        public static string Serialize(object obj)
        {
            if (obj is not Calendar) { throw new ArgumentException("Object is not of type AgendaCalendar", nameof(obj)); }

            Calendar calendar = (Calendar)obj;
            IcalCalendar ical = new IcalCalendar();

            foreach (var calendarEvent in calendar.Events)
            {
                var recurrency_pattern = new RecurrencePattern();
                if (calendarEvent.ReccurenceRules.freq != "none")
                {
                    recurrency_pattern = new RecurrencePattern()
                    {
                        Frequency = ConvertToFrequencyType(calendarEvent.ReccurenceRules.freq),
                        Interval = calendarEvent.ReccurenceRules.interval,
                        ByDay = ConvertToWeekDay(calendarEvent.ReccurenceRules.byweekday),
                        Until = DateTime.Parse(calendarEvent.ReccurenceRules.until, null)
                    };
                }

                var rules = new List<RecurrencePattern>() { recurrency_pattern };

                var @event = new CalendarEvent()
                {
                    Summary = calendarEvent.Title,
                    Start = new CalDateTime(calendarEvent.StartTime),
                    End = new CalDateTime(calendarEvent.EndTime),
                    Description = calendarEvent.Description,
                    Uid = calendarEvent.Id.ToString(),
                    Organizer = new Organizer("email@yandex.ru"),
                    Location = calendarEvent.Location,
                };

                if (calendarEvent.ReccurenceRules is not null)
                {
                    @event.RecurrenceRules = rules;
                }
                ical.Events.Add(@event);
            }

            ical.Properties.Add(new CalendarProperty("X-WR-CALNAME", calendar.Title));
            ical.Properties.Add(new CalendarProperty("X-WR-CALDESC", calendar.CalendarDescription));
            ical.Properties.Add(new CalendarProperty("X-WR-COLOR", calendar.CalendarColor));
            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(ical);
            return serializedCalendar;
        }

        public static Calendar Deserialize(string ical_format)
        {
            IcalCalendar ical_calendar = IcalCalendar.Load(new StringReader(ical_format));
            if (ical_calendar is null) return new Calendar();
            var title = string.Empty;
            var description = string.Empty;
            var calendarColor = string.Empty;
            if (ical_calendar.Properties.Any())
            {
                title = description = ical_calendar.Properties.ContainsKey("X-WR-CALNAME") ? 
                    ical_calendar.Properties.First(x => x.Name == "X-WR-CALNAME").Value.ToString() : "";
                description = ical_calendar.Properties.ContainsKey("X-WR-CALDESC") ? 
                    ical_calendar.Properties.First(x => x.Name == "X-WR-CALDESC").Value.ToString() : "";
                calendarColor = ical_calendar.Properties.ContainsKey("X-WR-COLOR") ? 
                    ical_calendar.Properties.First(x => x.Name == "X-WR-COLOR").Value.ToString() : "";
            }
            var events = new List<Event>();

            if (ical_calendar.Events.Any())
            {
                foreach (var IcalEvent in ical_calendar.Events)
                {
                    var @event = new Event()
                    {
                        Title = IcalEvent.Summary ?? "NonameEvent",
                        StartTime = IcalEvent.Start?.Value ?? DateTime.MinValue,
                        EndTime = IcalEvent.End?.Value ?? DateTime.MinValue,
                        Description = IcalEvent.Description ?? "",
                        EventParticipants = new List<EventParticipant>(),
                        Location = IcalEvent.Location ?? "",
                    };
                    if (IcalEvent.RecurrenceRules is not null)
                    {
                        foreach (var rec_rule in IcalEvent.RecurrenceRules)
                        {
                            if (rec_rule.Frequency == FrequencyType.None) continue;
                            var rec_rules = IcalEvent.RecurrenceRules[0];
                            var reccurencyRule = new RecurrenceRule()
                            {
                                freq = ConvertFromFrequencyType(rec_rules.Frequency),
                                interval = rec_rules.Interval,
                                byweekday = ConvertFromWeekDay(rec_rules.ByDay),
                                dtstart = Convert.ToString(IcalEvent.Start?.Value ?? DateTime.MinValue),
                                until = Convert.ToString(rec_rules.Until)
                            };
                            @event.ReccurenceRules = reccurencyRule;
                        }
                    }

                    events.Add(@event);
                }
            }

            var calendar = new Calendar()
            {
                Events = events,
                Reminders = new List<Reminder>(),
                CalendarDescription = description,
                Title = title,
                CalendarColor = calendarColor
            };
            return calendar;
        }

        private static FrequencyType ConvertToFrequencyType(string frequency)
        {
            switch (frequency)
            {
                case "daily":
                    return FrequencyType.Daily;
                case "monthly":
                    return FrequencyType.Monthly;
                case "weekly":
                    return FrequencyType.Weekly;
                case "yearly":
                    return FrequencyType.Yearly;
                default:
                    return FrequencyType.None;
            }
        }

        private static string ConvertFromFrequencyType(FrequencyType frequencyType)
        {
            switch (frequencyType)
            {
                case FrequencyType.Daily:
                    return "daily";
                case FrequencyType.Monthly:
                    return "monthly";
                case FrequencyType.Weekly:
                    return "weekly";
                case FrequencyType.Yearly:
                    return "yearly";
                default:
                    return "none";
            }
        }

        private static List<WeekDay> ConvertToWeekDay(List<string> days)
        {
            var WeekDays = new List<WeekDay>();
            if (days is null) return WeekDays;
            foreach (var day in days)
            {
                switch (day)
                {
                    case "mo":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Monday });
                        break;
                    case "tu":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Tuesday });
                        break;
                    case "we":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Wednesday });
                        break;
                    case "th":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Thursday });
                        break;
                    case "fr":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Friday });
                        break;
                    case "sa":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Saturday });
                        break;
                    case "su":
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Sunday });
                        break;
                    default:
                        WeekDays.Add(new WeekDay() { });
                        break;
                }
            }
            return WeekDays;
        }

        private static List<string> ConvertFromWeekDay(List<WeekDay> days)
        {
            var WeekDays = new List<string>();
            foreach (var day in days)
            {
                switch (day.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        WeekDays.Add("mo");
                        break;
                    case DayOfWeek.Tuesday:
                        WeekDays.Add("tu");
                        break;
                    case DayOfWeek.Wednesday:
                        WeekDays.Add("we");
                        break;
                    case DayOfWeek.Thursday:
                        WeekDays.Add("th");
                        break;
                    case DayOfWeek.Friday:
                        WeekDays.Add("fr");
                        break;
                    case DayOfWeek.Saturday:
                        WeekDays.Add("sa");
                        break;
                    case DayOfWeek.Sunday:
                        WeekDays.Add("su");
                        break;
                    default:
                        break;
                }
            }
            return WeekDays;
        }

    }
}
