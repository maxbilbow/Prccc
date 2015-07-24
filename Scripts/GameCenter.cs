using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Runtime.InteropServices;
#if !UNITY_IOS && !UNITY_STANDALONE_OSX
//using GooglePlayGames;
#endif



namespace RMX {
	public class GameCenter : ASingleton<GameCenter> , EventListener {


		void Start() {
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
			Authenticate ();
//			TimeBasedAchievementsShouldUpdate ();

			settings.willPauseOnLoad = SavedData.Get(UserData.CurrentSession).Float > 0;
			if (settings.willPauseOnLoad) {
				PauseCanvas.current.Pause(true);
			}
		

		
			UpdateGameCenterAchievements ();

		}


		void UpdateGameCenterAchievements () {
			foreach (KeyValuePair<UserData,string> id in UniqueID)
				if (!CheckAchievementsWithGameCenter (id.Key) && SavedData.Get (id.Key).Bool) 
					ReportProgress (id.Key, true);
		}

		public override void OnEventDidStart(Event theEvent, object info) {
			switch (theEvent) {
			case Event.PauseSession:
				Authenticate ();
				break;

			}
		}

		public override void OnEventDidEnd(Event theEvent, object info) {
			switch (theEvent) {
			case Event.PauseSession:
				UpdateGameCenterAchievements ();
				ReportScore(SavedData.Get(UserData.CurrentProcrastination).Long, UserData.LongestProctrastination);
				break;
			}
		}

		public override void OnEvent(Event theEvent, object info) {
			switch (theEvent) {
			case Event.GC_AchievementGained:
				if (info is UserData) {
					var key = (UserData) info;
					ReportProgress (key, true);
					if (Bugger.WillLog (Testing.EventCenter, info.ToString ()))
						Debug.Log (Bugger.Last);
				}
				break;
			}
		}

		void Authenticate() {
			var userInfo = Bugger.StartNewLog (Testing.GameCenter);
			if (!UserAuthenticated) {
				WillBeginEvent(Event.GC_UserAuthentication);
				Social.localUser.Authenticate (success => {
					if (success) {
						DidFinishEvent (Event.GC_UserAuthentication, EventStatus.Success);
						userInfo.message += "Authentication successful";
						userInfo.message += "Username: " + Social.localUser.userName + 
							"\nUser ID: " + Social.localUser.id + 
							"\nIsUnderage: " + Social.localUser.underage;
					} else {
						DidFinishEvent (Event.GC_UserAuthentication, EventStatus.Failure);
						userInfo.message += "Authentication failed";
					}
				});


			} else {
				userInfo.message += "Authentication already completed\n";
			}
			if (userInfo.isActive)
				Debug.Log (userInfo);
		}

//		public bool HasAchieved(UserData key) {
//			try {
//				return SavedData.Get (key).Bool;
//			} catch (Exception e) {
//				SavedData.Get (key).Bool = false;
//				var log = Bugger.StartNewLog(Testing.Exceptions, "HasAchieved(" + key + ") threw an error!\n" + e.Message);
//				if (log.isActive)
//				    Debug.Log(log);
//				return false;
//			}
//		}

		public void ReportScore (long score, UserData data) {
			if (UserAuthenticated && score > 0) {
				string leaderboardID = UniqueID [data];
				var log = Bugger.StartNewLog (Testing.GameCenter);
				log.message += "Reporting score " + score + " on leaderboard " + leaderboardID + "\n";
				try {
					Social.ReportScore (score, leaderboardID, success => {
						log.message += success ? "Reported score successfully" : "Failed to report score";	
					});
				} catch (System.Exception e) {
					log.message += e;
					log.feature = Testing.Exceptions;
				} finally {
					if (log.isActive)
						Debug.Log(log);
				}
			}
		}


		private UserData[] timeBasedAchievements = {
			UserData.AmeteurCrastinator, 
			UserData.Apathetic, UserData.SemiPro, UserData.Pro 
		};



		// Update is called once per frame
		float _checkTime = 0;
		void Update () {
			if (Time.fixedTime > _checkTime) {
				foreach (UserData key in timeBasedAchievements)
					HasMetTimeCriteria(key);
				_checkTime = Time.fixedTime + settings.updateScoresEvery;
			}
		}


		public static bool HasPlayerAlreadyAchieved(UserData key) {
			return SavedData.Get(key).Bool;
		}

		public static bool HasMetTimeCriteria(UserData key) {
			var totalTime = SavedData.Get(UserData.TotalTime).Float;
			var result = false;
			switch (key) {
			case UserData.AmeteurCrastinator:
				result = SavedData.Get(key).Bool ? true : totalTime > 20;
				break;
			case UserData.TimeWaster:
				result = SavedData.Get(key).Bool ? true : totalTime > (10 * 60);
				break;
			case UserData.SemiPro:
				result = SavedData.Get(key).Bool ? true : totalTime > (Settings.current.TotalDevTimeWasted / 4);
				break;
			case UserData.Apathetic:
				result = SavedData.Get(key).Bool ? true : totalTime > (Settings.current.TotalDevTimeWasted / 2);
				break;
			case UserData.Pro:
				result = SavedData.Get(key).Bool ? true : totalTime > Settings.current.TotalDevTimeWasted ;//gameData.PercentageOfDevTimeWasted;	
				break;
			default:
				throw new Exception(key + " Has not ben accounded for in HasMetTimeCriteria(UserData key)");
			}
	
			if (result) {// && result != SavedData.Get (key).Bool) { 
				Notifications.EventDidOccur (Event.GC_AchievementGained, key);
				return true;
			} else {
				return false; 
			}

		}

		/*
		public static bool HasPlayerAchieved(UserData key, bool result) {
			var totalTime = SavedData.Get(UserData.TotalTime).Double;
			switch (key) {
			case UserData.AmeteurCrastinator:
				result = SavedData.Get(key).Bool ? true : totalTime > 20;
				break;
			case UserData.TimeWaster:
				result = SavedData.Get(key).Bool ? true : totalTime > (10 * 60);
				break;
			case UserData.SemiPro:
				result = SavedData.Get(key).Bool ? true : totalTime > (Settings.current.TotalDevTimeWasted / 4);
				break;
			case UserData.Apathetic:
				result = SavedData.Get(key).Bool ? true : totalTime > (Settings.current.TotalDevTimeWasted / 2);
				break;
			case UserData.Pro:
				result = SavedData.Get(key).Bool ? true : totalTime > Settings.current.TotalDevTimeWasted ;//gameData.PercentageOfDevTimeWasted;	
				break;
			case UserData.MakingTime:
			case UserData.BigTime:
			case UserData.OverTime:
				break;
			}
			if (result && !SavedData.Get (key).Bool) { 
				SavedData.Get (key).Bool = true;
				Notifications.EventDidOccur (Event.GC_AchievementGained, key);
				try {
					current.ReportProgress(key, true);
				} catch (Exception e){
					Debug.LogWarning(e.Message);
				}
				return true;
			} else {
				return result; 
			}

		}
		*/
		static bool UserAuthenticated {
			get {
				return Notifications.StatusOf(Event.GC_UserAuthentication) == EventStatus.Success;
			}
		}
		const double EVENT_BASED_ACHIEVEMENT = -1;
		public void ReportProgress(UserData data, bool achieved) {
			SavedData.Get(data).Bool = achieved;
			var log = Bugger.StartNewLog (Testing.Achievements, "Reporting Progress: " + achieved + "\n");

			float progress = achieved ? 100 : 0;

			if (progress < 0) {
				Debug.LogError(data.ToString() + " Tried to log as time based achievement but failed with " + progress);
				return;
			}
			
			if (UserAuthenticated) {
				#if UNITY_IOS || UNITY_STANDALONE_OSX
				try {
					GKAchievementReporter.ReportAchievement(UniqueID[data], progress, true);
					log.message += "\n => SUCCESS";
				} catch (Exception e){
					if (log.isActive) 
						Debug.Log(log);
					log.feature = Testing.Exceptions;
					log.message += e.Message;
					if (log.isActive) 
						Debug.Log(log);
					log.feature = Testing.Achievements;
				}
				#else
				try {
					Social.ReportProgress (UniqueID[data], progress, result => {
						log.message += ", result: " + result;
						if (result) {
							log.message += "\n => SUCCESS";
						} else {
							log.message += "\n => Achievement Failed to report";
						}
					});
				} catch (Exception e){
					if (log.isActive) 
						Debug.Log(log);
					log.feature = Testing.Exceptions;
					log.message += e.Message;
					if (log.isActive) 
						Debug.Log(log);
					log.feature = Testing.Achievements;
				}
				#endif
				log.message += "\n New status isCompleted: " + progress;
				
			}
			if (log.isActive)
				Debug.Log(log);

		}

		bool CheckAchievementsWithGameCenter(UserData key) {
			var achievementID = UniqueID [key];
			var isComplete = false;
			var log = Bugger.StartNewLog(Testing.Achievements);
			if (UserAuthenticated) { //TODO: Check this works
				try {
					Social.LoadAchievements (achievements => {
						if (achievements.Length > 0) {
							log.message += "Got " + achievements.Length + " achievement instances:\n";
							foreach (IAchievement achievement in achievements) {
								if ( achievement.id == achievementID) {
									isComplete = achievement.completed || achievement.percentCompleted == 100;
									log.message += "Achievement " + achievement.id + ", progresss: " + achievement.percentCompleted + ", complete: " + achievement.completed + "\n";
									break;
								}
							}
						} else {
							if (log.isActive) 
								Debug.Log (log);
							throw new System.ArgumentException ("No achievements returned");
						}
					});
				} catch (System.ArgumentException exception) {
					log.message += exception.Message;
					log.feature = Testing.Exceptions;
					if (log.isActive)
						Debug.Log (log);
					log.feature = Testing.Achievements;
				}
			}
			return isComplete;
		}


		private void InitializeKeys() {
			#if UNITY_IOS || UNITY_STANDALONE_OSX
			foreach (KeyValuePair<UserData,string> pair in UniqueID) {
				UniqueID[pair.Key] = "grp." + pair.Value;
			}
			#endif
		}

		#if UNITY_IOS || UNITY_STANDALONE_OSX
		const string _grp = "grp.";
		#else
		const string _grp = "";
		#endif

		public static Dictionary<UserData,string> UniqueID = new Dictionary<UserData,string> () {
			{ UserData.LongestProctrastination, _grp + "CgkI2PKS_coeEAIQAw" },//"55415446";
			{ UserData.PercentageOfDevTime, _grp + "CgkI2PKS_coeEAIQCA" },
			{ UserData.AmeteurCrastinator, _grp + "CgkI2PKS_coeEAIQAQ" },
			{ UserData.TimeWaster, _grp + "CgkI2PKS_coeEAIQBA" },
			{ UserData.Apathetic, _grp + "CgkI2PKS_coeEAIQBw" },
			{ UserData.SemiPro, _grp + "CgkI2PKS_coeEAIQBQ" },
			{ UserData.Pro, _grp + "CgkI2PKS_coeEAIQBg" },
			{ UserData.MakingTime, _grp + "CgkI2PKS_coeEAIQCQ" },
			{ UserData.BigTime, _grp + "CgkI2PKS_coeEAIQDA" },
			{ UserData.OverTime, _grp + "CgkI2PKS_coeEAIQDQ" }
		};

//		public string GetID(UserData key) {
//			#if UNITY_IOS || UNITY_STANDALONE_OSX
//			string id = "grp.";
//			#else
//			string id = "";
//			#endif
//
//			return id + UniqueID [key];
//
//			switch (key) {
//			// Leadership Boards
//			case UserData.LongestProctrastination:
//				id += "CgkI2PKS_coeEAIQAw";//"55415446";
//				break;
//			case UserData.PercentageOfDevTime:
//				id += "CgkI2PKS_coeEAIQCA";//"55415445";
//				break;
//			// Time Based Achievements
//			case UserData.AmeteurCrastinator:
//				id += "CgkI2PKS_coeEAIQAQ";
//				break;
//			case UserData.TimeWaster:
//				id += "CgkI2PKS_coeEAIQBA";
//				break;
//			case UserData.Apathetic:
//				id += "CgkI2PKS_coeEAIQBw";
//				break;
//			case UserData.SemiPro:
//				id += "CgkI2PKS_coeEAIQBQ";
//				break;
//			case UserData.Pro:
//				id += "CgkI2PKS_coeEAIQBg";
//				break;
//			// Event Based Achievements
//			case UserData.MakingTime:
//				id += "CgkI2PKS_coeEAIQCQ";
//				break;
//			case UserData.BigTime:
//				id += "CgkI2PKS_coeEAIQDA";
//				break;
//			case UserData.OverTime:
//				id += "CgkI2PKS_coeEAIQDQ";
//				break;
//			}
//			return id;
//		}


	}
}