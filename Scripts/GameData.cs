using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
namespace RMX {




	public class GameData : ASingleton<GameData>  {
		/// <summary>
		/// Usually true after the game was turned off and on
		/// </summary>
		/// <value><c>true</c> if new session; otherwise, <c>false</c>.</value>
	



		public float totalTime {
			get {
				return SavedData.Get(UserData.TotalTime).Float;
			}
		}
		
		
		public float currentProcrastination {
			get {
				return SavedData.Get(UserData.CurrentProcrastination).Float;
			}
		}
		
		public float currentSessionTime {
			get {
				return SavedData.Get(UserData.CurrentSession).Float;
			}
		}
		
		public float longestProcrastination {
			get {
				return SavedData.Get(UserData.LongestProctrastination).Float;
			}
		}






//		public bool GetSavedData()

//		private long GetLong(UserData data) {
//			switch (data) {
//			case UserData.CurrentSession:
//				return (long) currentSessionTime;
//			case UserData.CurrentProcrastination:
//				return (long) currentProcrastination;
//			case UserData.TotalTime:
//				return (long) totalTime;
//			case UserData.OfDevTime:
//				return (long) (100 * totalTime / settings.TotalDevTimeWasted);
//			}
//			return -1;
//		}

		public long PercentageOfDevTimeWastedX10000 {
			get {
				return (long) (PercentageOfDevTimeWasted * 10000);
			}
		}

		public double PercentageOfDevTimeWasted {
			get {
				return GameData.current.totalTime / settings.TotalDevTimeWasted;
			}
		}





		/*
		public static UserData GetEnum(string key) {
			switch (key) {
			case Key.LastSession:
				return UserData.CurrentSession;
			case Key.LastProcrastination:
				return UserData.CurrentProcrastination;
			case Key.Total:
				return UserData.Total;
			default:
				throw new System.ArgumentNullException("Key was not recognised in GameData.GetEnum(string key)");
			} 

		}
*/
		public void TestData ()
		{
			float[] testTimes = { 10f, 20f, 30f, 45f, 60f, 120f, 3000f, 6000f, 80000f };
			foreach (float time in testTimes) {
				Debug.Log("\n\n--------- Testing: " + time + "---------");
				var list = DataReader.current.GetActivityList (time);
				foreach (string thing in list) {
					Debug.Log (thing);
				}
			}
			
		}

		public Wychd WhatYouCouldHaveDone(float time) {
			Wychd result = DataReader.current.GetActivityList (time);
			var log = Bugger.StartNewLog (Testing.GameDataLists);
			log.message += "List accessed with time: " + time + ", and " + result.Count + " sentences.";
			if (result.Count > 0) {
				log.message += "\n - Adding from database...";
			} else {
				log.message += "\n - Found none in Database...";
				float timeInMinutes = time / 60;
				if (timeInMinutes < 0.5) {
					result.Add("approved this app for distribution through the app store!");
					result.Add("done very little else. And I'm glad you did not");
				} else if (timeInMinutes < 1) {
					result.Add("trolled someone on Twitter");
				} else if (timeInMinutes < 1.5) {
					result.Add ("briefly checked for new emails");
				} else if (timeInMinutes < 2) {
					result.Add ("re-heated that cold cup of coffee using the microvave.");
				} else if (timeInMinutes < 4) {
					result.Add ("soft-boiled an egg.");
				} else if (timeInMinutes < 10) {
					result.Add ("hard boilded an egg.");
				} else if (timeInMinutes < 12) {
					result.Add ("had a short bath");
				} else {
					result.Add ("had a good hard think about the meaning of it all.");
					result.Add ("taken a good long look at yourself.");
					result.Add ("learned a new Spanish phrase on Duolingo");
					result.Add ("helped a blind person to see with \"Be My Eyes\"");
				}
			}
			if (log.isActive) {
				foreach (string s in result) {
					log.message += "\n => " + s;
				}
				Debug.Log(log);
			}
			return result;
		}


	}



}