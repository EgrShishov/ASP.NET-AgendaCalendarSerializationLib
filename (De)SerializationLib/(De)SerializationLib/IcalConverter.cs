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
                if (calendarEvent.ReccurenceRules is not null)
                {
                    recurrency_pattern = new RecurrencePattern()
                    {
                        Interval = calendarEvent.ReccurenceRules.Interval,
                        Frequency = ConvertTo(calendarEvent.ReccurenceRules.Frequency),
                        ByDay = ConvertToWeekDay(calendarEvent.ReccurenceRules.DaysOfWeek),
                        ByWeekNo = calendarEvent.ReccurenceRules.WeeksOfMonth,
                        ByMonth = calendarEvent.ReccurenceRules.MonthsOfYear,
                        ByMonthDay = calendarEvent.ReccurenceRules.DaysOfMonth
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
                    Location = calendarEvent.Location
                };

                if(calendarEvent.ReccurenceRules is not null)
                {
                    @event.RecurrenceRules = rules;
                    if(calendarEvent.ReccurenceRules.RecurrenceDates is not null)
                    {
                        @event.RecurrenceDates = ConvertToPeriodList(calendarEvent.ReccurenceRules.RecurrenceDates);
                    }
                }
                ical.Events.Add(@event);
            }

            ical.Name = calendar.Title;
            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(ical);
            return serializedCalendar;
        }

        public static Calendar Deserialize(string ical_format)
        {
            IcalCalendar ical_calendar = IcalCalendar.Load(ical_format); 
            var title = ical_calendar.Name;
            var events = new List<Event>();

            foreach (var IcalEvent in ical_calendar.Events)
            {

                var @event = new Event()
                {
                    Title = IcalEvent.Summary,
                    StartTime = IcalEvent.Start.Value,
                    EndTime = IcalEvent.End.Value,
                    Description = IcalEvent.Description,
                    EventParticipants = new List<EventParticipant>(),
                    Location = IcalEvent.Location,
                };
                if (IcalEvent.RecurrenceRules is not null)
                {
                    foreach(var rec_rule in IcalEvent.RecurrenceRules)
                    {
                        if (rec_rule is null) continue;
                        var rec_rules = IcalEvent.RecurrenceRules[0];
                        var reccurencyRule = new RecurrenceRule()
                        {
                            Interval = rec_rules.Interval,
                            Frequency = ConvertToRecurrenceFrequency(rec_rules.Frequency),
                            DaysOfWeek = ConvertFromWeekDay(rec_rules.ByDay),
                            WeeksOfMonth = rec_rules.ByWeekNo,
                            MonthsOfYear = rec_rules.ByMonth,
                            DaysOfMonth = rec_rules.ByMonthDay
                        };
                        @event.ReccurenceRules = reccurencyRule;
                    }
                }
                events.Add(@event);
            }

            var calendar = new Calendar() 
            {
                Events = events,
                Reminders = new List<Reminder>(),
                Title = ical_calendar.Name,
            };
            return calendar;
        }

        private static FrequencyType ConvertTo(RecurrenceFrequency frequency)
        {
            switch (frequency)
            {
                case RecurrenceFrequency.Daily:
                    return FrequencyType.Daily;
                case RecurrenceFrequency.Monthly:
                    return FrequencyType.Monthly;
                case RecurrenceFrequency.Weekly:
                    return FrequencyType.Weekly;
                case RecurrenceFrequency.Yearly:
                    return FrequencyType.Yearly;
                default:
                    return FrequencyType.None;
            }
        }

        public static RecurrenceFrequency ConvertToRecurrenceFrequency(FrequencyType frequencyType)
        {
            switch (frequencyType)
            {
                case FrequencyType.Daily:
                    return RecurrenceFrequency.Daily;
                case FrequencyType.Monthly:
                    return RecurrenceFrequency.Monthly;
                case FrequencyType.Weekly:
                    return RecurrenceFrequency.Weekly;
                case FrequencyType.Yearly:
                    return RecurrenceFrequency.Yearly;
                default:
                    return RecurrenceFrequency.None;
            }
        }

        private static List<WeekDay> ConvertToWeekDay(List<RecurrenceDayOfWeek> days)
        {
            var WeekDays = new List<WeekDay>();
            if (days is null) return WeekDays;
            foreach (var day in days)
            {
                switch (day)
                {
                    case RecurrenceDayOfWeek.Monday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Monday });
                        break;
                    case RecurrenceDayOfWeek.Tuesday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Tuesday });
                        break;
                    case RecurrenceDayOfWeek.Wednesday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Wednesday });
                        break;
                    case RecurrenceDayOfWeek.Thursday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Thursday });
                        break;
                    case RecurrenceDayOfWeek.Friday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Friday });
                        break;
                    case RecurrenceDayOfWeek.Saturday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Saturday });
                        break;
                    case RecurrenceDayOfWeek.Sunday:
                        WeekDays.Add(new WeekDay() { DayOfWeek = DayOfWeek.Sunday });
                        break;
                    default:
                        WeekDays.Add(new WeekDay() { });
                        break;
                }
            }
            return WeekDays;
        }

        private static List<RecurrenceDayOfWeek> ConvertFromWeekDay(List<WeekDay> days)
        {
            var WeekDays = new List<RecurrenceDayOfWeek>();
            foreach (var day in days)
            {
                switch (day.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        WeekDays.Add(RecurrenceDayOfWeek.Monday);
                        break;
                    case DayOfWeek.Tuesday:
                        WeekDays.Add(RecurrenceDayOfWeek.Tuesday);
                        break;
                    case DayOfWeek.Wednesday:
                        WeekDays.Add(RecurrenceDayOfWeek.Wednesday);
                        break;
                    case DayOfWeek.Thursday:
                        WeekDays.Add(RecurrenceDayOfWeek.Thursday);
                        break;
                    case DayOfWeek.Friday:
                        WeekDays.Add(RecurrenceDayOfWeek.Friday);
                        break;
                    case DayOfWeek.Saturday:
                        WeekDays.Add(RecurrenceDayOfWeek.Saturday);
                        break;
                    case DayOfWeek.Sunday:
                        WeekDays.Add(RecurrenceDayOfWeek.Sunday);
                        break;
                    default:
                        break;
                }
            }
            return WeekDays;
        }

        public static IList<PeriodList> ConvertToPeriodList(List<TimePeriod> periodDates)
        {
            var periodList = new List<PeriodList>();

            foreach (var date in periodDates)
            {
                /*periodList.Add(new Period()
                {
                    //StartTime = date.StartTime,
                    //EndTime = date.EndTime,
                });*/
            }
            return periodList;
        }
    }
}
