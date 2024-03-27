// See https://aka.ms/new-console-template for more information
using _De_SerializationLib;
using AgendaCalendar.Domain.Entities;

Console.WriteLine("Hello, World!");

var events = new List<Event>() { new Event() 
{ 
    Description = "New event!", 
    AuthorId = 1, 
    StartTime = DateTime.Now, 
    EndTime = DateTime.Now.AddDays(2),
    EventParticipants = new List<EventParticipant>()
    {
        new EventParticipant()
        {
            Email = "e.shishov98@yandex.ru",
            Name = "Egorka",
        },
        new EventParticipant()
        {
            Email = "e.shishov98@yandex.ru",
            Name = "Shishov",
        },
        new EventParticipant()
        {
            Email = "mr.nysha93@mail.com",
            Name = "Windows",
        },
        new EventParticipant()
        {
            Email = "aboba@gmail.com",
            Name = "Height",
        }
    },
    Location = "NY city, Brooklyn ave, st. 23",
    Title = "Mama papa"
},
    new Event()
    {
        Title = "Pizdec",
        Description = "Reccuring event!",
        AuthorId = 1,
        StartTime = DateTime.Now.AddDays(2),
        EndTime = DateTime.Now.AddDays(5),
        Location = "Mam's house",
/*        ReccurenceRules = new RecurrenceRule()
        {
            *//*DaysOfMonth = new List<int>(){1, 4, 6, 20},
            Interval = 1,
            Frequency = RecurrenceFrequency.Daily,
            RecurrenceDates = new List<TimePeriod>(){ new TimePeriod() {StartTime = DateTime.Now.AddMonths(1), EndTime = DateTime.Now.AddMonths(2) } }*//*
        }*/
    }
};

var calendar = new Calendar()
{
    AuthorId = 1,
    CalendarDescription = "Test",
    Title = "Test",
    Events = events,
    Reminders = new List<Reminder>(),
    Subscribers = new List<int>(),
};

var serialized = IcalConverter.Serialize(calendar);
Console.WriteLine(serialized);

var text = "BEGIN:VCALENDAR\r\nVERSION:2.0\r\nCALSCALE:GREGORIAN\r\nBEGIN:VEVENT\r\nSUMMARY:Access-A-Ride Pickup\r\nDTSTART;TZID=America/New_York:20130802T103400\r\nDTEND;TZID=America/New_York:20130802T110400\r\nLOCATION:1000 Broadway Ave.\\, Brooklyn\r\nDESCRIPTION: Access-A-Ride trip to 900 Jay St.\\, Brooklyn\r\nSTATUS:CONFIRMED\r\nSEQUENCE:3\r\nBEGIN:VALARM\r\nTRIGGER:-PT10M\r\nDESCRIPTION:Pickup Reminder\r\nACTION:DISPLAY\r\nEND:VALARM\r\nEND:VEVENT\r\nBEGIN:VEVENT\r\nSUMMARY:Access-A-Ride Pickup\r\nDTSTART;TZID=America/New_York:20130802T200000\r\nDTEND;TZID=America/New_York:20130802T203000\r\nLOCATION:900 Jay St.\\, Brooklyn\r\nDESCRIPTION: Access-A-Ride trip to 1000 Broadway Ave.\\, Brooklyn\r\nSTATUS:CONFIRMED\r\nSEQUENCE:3\r\nBEGIN:VALARM\r\nTRIGGER:-PT10M\r\nDESCRIPTION:Pickup Reminder\r\nACTION:DISPLAY\r\nEND:VALARM\r\nEND:VEVENT\r\nEND:VCALENDAR";
var calendar_deserialized = IcalConverter.Deserialize(serialized);
Console.WriteLine(calendar_deserialized.Title);