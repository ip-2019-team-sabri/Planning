using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IP_Planning_GoogleAPI
{
    public class GoogleAPI
    {
        public object createEvent(CreateEvent createEvent)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";

                UserCredential credential;

                using (var stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }

                // Create Google Calendar API service.
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

            Event newEvent = new Event()
            {
                Summary = createEvent.Summary,
                Location = createEvent.Location,
                Description = createEvent.Description,
                Start = new EventDateTime()
                {
                    DateTime = createEvent.StartDatum,
                    TimeZone = createEvent.TimeZone,
                },
                End = new EventDateTime()
                {
                    DateTime = createEvent.EindDatum,
                    TimeZone = createEvent.TimeZone,
                },
                Recurrence = new String[] { "RRULE:FREQ=DAILY;COUNT=2" },
                Attendees = new EventAttendee[]
                {
                    new EventAttendee() { Email = createEvent.SprekerEmail, DisplayName= createEvent.SprekerNaam + " (SPREKER)" }
                },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[]
                    {
                        new EventReminder() { Method = "email", Minutes = 24 * 60 }
                    }
                }
                
            };

            EventsResource.InsertRequest request = service.Events.Insert(newEvent, createEvent.calendarID);
            Event createdEvent = request.Execute();
            Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
            Console.Read();

            return createdEvent;
        }

        public Event updateEvent(UpdateEvent eventToUpdate)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            EventsResource.GetRequest getRequest = service.Events.Get(eventToUpdate.calendarID, eventToUpdate.eventID);
            Event updateEvent = getRequest.Execute();
            updateEvent.Summary = eventToUpdate.Summary;
            updateEvent.Location = eventToUpdate.Location;
            updateEvent.Description = eventToUpdate.Description;
            updateEvent.Start = new EventDateTime()
            {
                DateTime = eventToUpdate.StartDatum,
                TimeZone = eventToUpdate.TimeZone,
            };
            updateEvent.End = new EventDateTime()
            {
                DateTime = eventToUpdate.EindDatum,
                TimeZone = eventToUpdate.TimeZone,
            };
            updateEvent.Attendees = new EventAttendee[]
            {
                new EventAttendee() { Email = eventToUpdate.SprekerEmail, DisplayName= eventToUpdate.SprekerNaam + " (SPREKER)" }
            };

            EventsResource.UpdateRequest updateRequest = service.Events.Update(updateEvent, eventToUpdate.calendarID, eventToUpdate.eventID);
            Event bijgewerkt = updateRequest.Execute();

            return bijgewerkt;
        }

        public Calendar createCalendar(CreateCalendar createCalendar)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Calendar calendar = new Calendar()
            {
                Summary = createCalendar.Summary,
                Description = createCalendar.Description,
                TimeZone = createCalendar.TimeZone
            };

            CalendarsResource.InsertRequest nieuwKalender = service.Calendars.Insert(calendar);
            Calendar createdCal = nieuwKalender.Execute();
            Console.WriteLine("Calendar created: {0}" + createdCal.Id, createdCal.Summary);
            Console.Read();

            return createdCal;
        }

        public Calendar updateCalendar(UpdateCalendar updateCalendar)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            CalendarsResource.GetRequest getRequest = service.Calendars.Get(updateCalendar.calendarID);
            Calendar toUpdateCalendar = getRequest.Execute();

            toUpdateCalendar.Summary = updateCalendar.Summary;
            toUpdateCalendar.Description = updateCalendar.Description;
            toUpdateCalendar.TimeZone = updateCalendar.TimeZone;

            CalendarsResource.UpdateRequest updateRequest = service.Calendars.Update(toUpdateCalendar, updateCalendar.calendarID);
            Calendar cal = updateRequest.Execute();

            return cal;
            
        }

        public void deleteEvent(string eventId)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // link om eventid te verkrijgen: https://calendar.google.com/calendar/r?tab=mc&eventdeb=1

            EventsResource.DeleteRequest request = service.Events.Delete("primary", eventId);
            request.Execute();
        }

        public void deleteCalendar(string calendarId)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
          
            CalendarsResource.ClearRequest clearRequest = service.Calendars.Clear(calendarId);//alle events uit primary calendar weg
            clearRequest.Execute();

            /*
            CalendarsResource.DeleteRequest delete = service.Calendars.Delete("calendarId");
            delete.Execute();
            */
        }

        public List<Event> getEvents(string calendarId)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            EventsResource.ListRequest listRequest = service.Events.List(calendarId);
            Events events = listRequest.Execute();
            List<Event> evenementen = new List<Event>();

            foreach (var myEvent in events.Items)
            {
                //Console.WriteLine(string.Format("EventSummary: {0} EventID: {1}", myEvent.Summary, myEvent.Id));
                evenementen.Add(myEvent);
            }

            return evenementen;
        }

        public List<Calendar> getCalendars(string calendarId)
        {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/calendar-dotnet-quickstart.json
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Google Calendar API .NET Quickstart";


            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            /*
            CalendarListResource.ListRequest list = service.CalendarList.List();
            CalendarList calendarList =  list.Execute();
            List<Calendar> kalenders = new List<Calendar>();

            foreach (var calendar in calendarList.Items)
            {
                //Console.WriteLine(string.Format("CalendarSummary: {0} CalendarID: {1}", calendar.Summary, calendar.Id));
            }
            */

            CalendarsResource.GetRequest getRequest = service.Calendars.Get(calendarId);
            Calendar cal = getRequest.Execute();
            List<Calendar> kalenders = new List<Calendar>();
            kalenders.Add(cal);

            return kalenders;
        }
    }
}
