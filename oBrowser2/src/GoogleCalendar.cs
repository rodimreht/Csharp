using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace oBrowser2
{
	public class GoogleCalendar
	{
		public static bool AddEvent(DateTime theTime, string email, string calID, string uni, string desc)
		{
			try
			{
				UserCredential credential;
				string resourceName = "oBrowser2.client_secret_207848219249.json";
				//string resourceName = "oBrowser2.client_secret_60278543472.json";
				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
						GoogleClientSecrets.Load(stream).Secrets,
						new[] { CalendarService.Scope.Calendar },
						"user", CancellationToken.None).Result;
				}
				/*
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					new ClientSecrets()
					{
						ClientId = "207848219249-miu24d6rcubiaa0pr75c22mle3lajsmd.apps.googleusercontent.com",
						ClientSecret = "43WPsL4zTSrVCrbVNAr2MntL"
					},
					new [] { CalendarService.Scope.Calendar },
					"user", CancellationToken.None).Result;
				*/

				// Create the service.
				CalendarService service = new CalendarService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credential,
					ApplicationName = "oBrowserRD2+",
				});

				Event ev = new Event()
				{
					Summary = "O-Game " + uni + " 공격 탐지",
					Description = desc,
					Start = new EventDateTime() {DateTime = theTime},
					End = new EventDateTime() {DateTime = theTime},
					/*Attendees = new List<EventAttendee>()
					{
						new EventAttendee() {Email = email}
					}*/
				};
				Event ret = service.Events.Insert(ev, string.IsNullOrEmpty(calID) ? "primary" : calID).Execute();
				Logger.Log("O-Game " + uni + "공격 탐지: Google Calendar 이벤트 송신 완료: " + ret.Id);
				return true;
			}
			catch (Exception ex)
			{
				Logger.Log("O-Game 공격 탐지: Google Calendar 이벤트 송신 실패: " + ex.Message);
				return false;
			}
		}
	}
}
