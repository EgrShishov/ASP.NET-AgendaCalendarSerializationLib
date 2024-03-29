﻿using AgendaCalendar.Domain.Entities;
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
                        Frequency = ConvertToFrequencyType(calendarEvent.ReccurenceRules.Frequency),
                        ByDay = ConvertToWeekDay(calendarEvent.ReccurenceRules.DaysOfWeek),
                        ByWeekNo = calendarEvent.ReccurenceRules.WeeksOfMonth ?? new List<int>(),
                        ByMonth = calendarEvent.ReccurenceRules.MonthsOfYear ?? new List<int>(),
                        ByMonthDay = calendarEvent.ReccurenceRules.DaysOfMonth ?? new List<int>(),
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

                if (calendarEvent.ReccurenceRules is not null)
                {
                    @event.RecurrenceRules = rules;
                    if (calendarEvent.ReccurenceRules.RecurrenceDates is not null)
                    {
                        @event.RecurrenceDates = ConvertToPeriodList(calendarEvent.ReccurenceRules.RecurrenceDates);
                    }
                }
                ical.Events.Add(@event);
            }

            ical.Properties.Add(new CalendarProperty("X-WR-CALNAME", calendar.Title));
            ical.Properties.Add(new CalendarProperty("X-WR-CALDESC", calendar.CalendarDescription));
            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(ical);
            return serializedCalendar;
        }

        public static Calendar Deserialize(string ical_format)
        {
            IcalCalendar ical_calendar = IcalCalendar.Load(new StringReader(ical_format));
            if (ical_calendar is null) return new Calendar();
            var title = ical_calendar.Properties.First(x => x.Name == "X-WR-CALNAME").Value.ToString() ?? "";
            var description = ical_calendar.Properties.First(x => x.Name == "X-WR-CALDESC").Value.ToString() ?? "";
            var events = new List<Event>();

            foreach (var IcalEvent in ical_calendar.Events)
            {
                var @event = new Event()
                {
                    Title = IcalEvent.Summary ?? "NonameEvent",
                    StartTime = IcalEvent.Start.Value,
                    EndTime = IcalEvent.End.Value,
                    Description = IcalEvent.Description ?? "",
                    EventParticipants = new List<EventParticipant>(),
                    Location = IcalEvent.Location ?? "",
                };
                if (IcalEvent.RecurrenceRules is not null)
                {
                    foreach (var rec_rule in IcalEvent.RecurrenceRules)
                    {
                        if (rec_rule is null) continue;
                        var rec_rules = IcalEvent.RecurrenceRules[0];
                        var reccurencyRule = new RecurrenceRule()
                        {
                            Interval = rec_rules.Interval,
                            Frequency = ConvertFromFrequencyType(rec_rules.Frequency),
                            DaysOfWeek = ConvertFromWeekDay(rec_rules.ByDay),
                            WeeksOfMonth = rec_rules.ByWeekNo ?? new List<int>(),
                            MonthsOfYear = rec_rules.ByMonth ?? new List<int>(),
                            DaysOfMonth = rec_rules.ByMonthDay ?? new List<int>(),
                        };
                        @event.ReccurenceRules = reccurencyRule;
                        if (IcalEvent.RecurrenceDates is not null)
                        {
                            @event.ReccurenceRules.RecurrenceDates = ConvertFromPeriodList(IcalEvent.RecurrenceDates);
                        }
                    }
                }

                events.Add(@event);
            }

            var calendar = new Calendar()
            {
                Events = events,
                Reminders = new List<Reminder>(),
                CalendarDescription = description,
                Title = title
            };
            return calendar;
        }

        private static FrequencyType ConvertToFrequencyType(RecurrenceFrequency frequency)
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

        private static RecurrenceFrequency ConvertFromFrequencyType(FrequencyType frequencyType)
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

        private static IList<PeriodList> ConvertToPeriodList(List<TimePeriod> periodDates)
        {
            var periodList = new List<PeriodList>();
            foreach (var date in periodDates)
            {
                var list = new PeriodList() { new Ical.Net.DataTypes.Period(new CalDateTime(date.StartTime), new CalDateTime(date.EndTime)) };
                periodList.Add(list);
            }
            return periodList;
        }

        private static List<TimePeriod> ConvertFromPeriodList(IList<PeriodList> periodList)
        {
            var timePeriods = new List<TimePeriod>();
            foreach (var period in periodList)
            {
                var timePeriod = new TimePeriod()
                {
                    StartTime = Convert.ToDateTime(period[0].StartTime),
                    EndTime = Convert.ToDateTime(period[0].EndTime)
                };
            }
            return timePeriods;
        }
    }
}
