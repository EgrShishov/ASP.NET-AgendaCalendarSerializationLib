using _De_SerializationLib;
using AgendaCalendar.Domain.Entities;

var events = new List<Event>() { new Event()
{
    Id = 79,
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
        Id = 3212,
        Title = "Pizdec",
        Description = "Reccuring event!",
        AuthorId = 1,
        StartTime = DateTime.Now.AddDays(2),
        EndTime = DateTime.Now.AddDays(5),
        Location = "Mam's house",
        ReccurenceRules = new RecurrenceRule
        {
            freq = "none",
            interval = 0,
            byweekday = [],
            dtstart = "",
            until = ""
        }
    }
};

var calendar = new Calendar()
{
    AuthorId = 1,
    CalendarDescription = "Test",
    Title = "Test",
    Events = events,
    Reminders = new List<Reminder>(),
    SubscribersId = new List<int>(),
};

var json = JsonConverter.GetJsonEventList(calendar.CalendarColor, calendar.Events);
Console.WriteLine(json);

var serialized = IcalConverter.Serialize(calendar);
Console.WriteLine(serialized);

var text = "BEGIN:VCALENDAR\r\nPRODID:-//Google Inc//Google Calendar 70.9054//EN\r\nVERSION:2.0\r\nCALSCALE:GREGORIAN\r\nMETHOD:PUBLISH\r\nX-WR-CALNAME:e.shishov98@yandex.ru\r\nX-WR-TIMEZONE:Europe/Minsk\r\nBEGIN:VEVENT\r\nDTSTART:20220629T210000Z\r\nDTSTAMP:20240328T075307Z\r\nUID:68qjaopickp64b9g6sr3cb9k6crmab9ockojabb36ko38cj56spj6oj164@google.com\r\nCREATED:20220701T122234Z\r\nLAST-MODIFIED:20220701T122234Z\r\nSEQUENCE:0\r\nSTATUS:CONFIRMED\r\nTRANSP:OPAQUE\r\nEND:VEVENT\r\nEND:VCALENDAR";
var second_text = "BEGIN:VCALENDAR\r\nPRODID:-//Google Inc//Google Calendar 70.9054//EN\r\nVERSION:2.0\r\nCALSCALE:GREGORIAN\r\nMETHOD:PUBLISH\r\nX-WR-CALNAME:Дни рождения\r\nX-WR-TIMEZONE:UTC\r\nX-WR-CALDESC:Показывает дни рождения\\, годовщины и другие значимые события \r\n для людей в Google Контактах.\r\nBEGIN:VEVENT\r\nDTSTART;VALUE=DATE:20231201\r\nDTEND;VALUE=DATE:20231202\r\nDTSTAMP:20240328T075307Z\r\nUID:2023_BIRTHDAY_self@google.com\r\nX-GOOGLE-CALENDAR-CONTENT-DISPLAY:CHIP\r\nX-GOOGLE-CALENDAR-CONTENT-ICON:https://calendar.google.com/googlecalendar/i\r\n mages/cake.gif\r\nCLASS:PUBLIC\r\nCREATED:20240324T154517Z\r\nDESCRIPTION:С днем рождения!\r\nLAST-MODIFIED:20240324T154517Z\r\nSEQUENCE:0\r\nSTATUS:CONFIRMED\r\nSUMMARY:С днем рождения!\r\nTRANSP:OPAQUE\r\nEND:VEVENT\r\nBEGIN:VEVENT\r\nDTSTART;VALUE=DATE:20241201\r\nDTEND;VALUE=DATE:20241202\r\nDTSTAMP:20240328T075307Z\r\nUID:2024_BIRTHDAY_self@google.com\r\nX-GOOGLE-CALENDAR-CONTENT-DISPLAY:CHIP\r\nX-GOOGLE-CALENDAR-CONTENT-ICON:https://calendar.google.com/googlecalendar/i\r\n mages/cake.gif\r\nCLASS:PUBLIC\r\nCREATED:20240324T154517Z\r\nDESCRIPTION:С днем рождения!\r\nLAST-MODIFIED:20240324T154517Z\r\nSEQUENCE:0\r\nSTATUS:CONFIRMED\r\nSUMMARY:С днем рождения!\r\nTRANSP:OPAQUE\r\nEND:VEVENT\r\nBEGIN:VEVENT\r\nDTSTART;VALUE=DATE:20251201\r\nDTEND;VALUE=DATE:20251202\r\nDTSTAMP:20240328T075307Z\r\nUID:2025_BIRTHDAY_self@google.com\r\nX-GOOGLE-CALENDAR-CONTENT-DISPLAY:CHIP\r\nX-GOOGLE-CALENDAR-CONTENT-ICON:https://calendar.google.com/googlecalendar/i\r\n mages/cake.gif\r\nCLASS:PUBLIC\r\nCREATED:20240324T154517Z\r\nDESCRIPTION:С днем рождения!\r\nLAST-MODIFIED:20240324T154517Z\r\nSEQUENCE:0\r\nSTATUS:CONFIRMED\r\nSUMMARY:С днем рождения!\r\nTRANSP:OPAQUE\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n";
var calendar_deserialized = IcalConverter.Deserialize(second_text);

Console.WriteLine(calendar_deserialized.CalendarDescription);
foreach (var eve in calendar_deserialized.Events)
{
    Console.WriteLine(eve);
    if (eve.ReccurenceRules is not null)
    {
        Console.WriteLine(eve.ReccurenceRules.freq);
    }
}

var mama = IcalConverter.Deserialize(second_text);
Console.WriteLine(mama.Title, mama.CalendarDescription);

var deserialized = JsonConverter.GetJsonEventList("blue", mama.Events);
Console.WriteLine(deserialized);
var papa = JsonConverter.GetEventListFromJson(deserialized);
foreach (var item in papa)
{
    Console.WriteLine(papa);
}