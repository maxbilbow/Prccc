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



using RMX;  namespace Procrastinate {
	public class GameCenter : RMX.Singletons.ASingleton<GameCenter> {


		void Start() {
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
			Authenticate ();
//			TimeBasedAchievementsShouldUpdate ();

			Settings.current.willPauseOnLoad = SavedData.Get(UserData.CurrentSession).Float > 0;
			if (Settings.current.willPauseOnLoad) {
				PauseCanvas.current.Pause(true);
			}
		

		
			UpdateGameCenterAchievements ();

		}


		void UpdateGameCenterAchievements () { //TODO
			foreach (KeyValuePair<UserData,string> id in UniqueID)
				if (!CheckAchievementsWithGameCenter (id.Key) && SavedData.Get (id.Key).Bool) 
					ReportProgress (id.Key);
		}

		public override void OnEventDidStart(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.PauseSession))
				Authenticate ();
		}

		public override void OnEventDidEnd(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.PauseSession)) {
				UpdateGameCenterAchievements ();
				ReportScore(SavedData.Get(UserData.CurrentProcrastination).Long, UserData.LongestProctrastination);
			} 
		}

		public override void OnEvent(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.GC_AchievementGained))
				if (info is UserData) {
					var key = (UserData) info;
					ReportProgress (key);
					if (Bugger.WillLog (Testing.EventCenter, info.ToString ()))
						Debug.Log (Bugger.Last);
			}
		}

		void Authenticate() {
			string userInfo = "";
			if (!UserAuthenticated) {
				WillBeginEvent(Events.GC_UserAuthentication);
				Social.localUser.Authenticate (success => {
					if (success) {
						DidFinishEvent (Events.GC_UserAuthentication, EventStatus.Success);
						userInfo += "Authentication successful";
						userInfo += "Username: " + Social.localUser.userName + 
							"\nUser ID: " + Social.localUser.id + 
							"\nIsUnderage: " + Social.localUser.underage;
					} else {
						DidFinishEvent (Events.GC_UserAuthentication, EventStatus.Failure);
						userInfo += "Authentication failed";
					}
				});


			} else {
				userInfo += "Authentication already completed\n";
			}
			if (Bugger.WillLog(Testing.GameCenter, userInfo))
				Debug.Log (Bugger.Last);
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
				var log = ""; var feature = Testing.GameCenter;
				log += "Reporting score " + score + " on leaderboard " + leaderboardID + "\n";
				try {
					Social.ReportScore (score, leaderboardID, success => {
						log += success ? "Reported score successfully" : "Failed to report score";	
					});
				} catch (System.Exception e) {
					log += e;
					feature = Testing.Exceptions;
				} finally {
					if (Bugger.WillLog(feature, log))
						Debug.Log (Bugger.Last);
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
				_checkTime = Time.fixedTime + Settings.current.updateScoresEvery;
			}
		}


		public static bool HasPlayerAlreadyAchieved(UserData key) {
			return SavedData.Get(key).Bool;
		}

		const float MINUTES = 60f, HOURS = 60 * 60f;
		public static bool HasMetTimeCriteria(UserData key) {
			var totalTime = SavedData.Get(UserData.TotalTime).Float;
			var result = false;
			switch (key) {
			case UserData.AmeteurCrastinator:
				result = SavedData.Get(key).Bool || totalTime > 20;
				break;
			case UserData.TimeWaster:
				result = SavedData.Get(key).Bool || totalTime > (10 * MINUTES);
				break;
			case UserData.SemiPro:
				result = SavedData.Get(key).Bool || totalTime > (Settings.current.TotalDevTimeWasted / 4);
				break;
			case UserData.Apathetic:
				result = SavedData.Get(key).Bool || totalTime > (Settings.current.TotalDevTimeWasted / 2);
				break;
			case UserData.Pro:
				result = SavedData.Get(key).Bool || totalTime > Settings.current.TotalDevTimeWasted ;//gameData.PercentageOfDevTimeWasted;	
				break;
			default:
				throw new Exception(key + " Has not ben accounded for in HasMetTimeCriteria(UserData key)");
			}
	
			if (result) {// && result != SavedData.Get (key).Bool) { 
				Notifications.EventDidOccur (Events.GC_AchievementGained, key);
//				SavedData.Get(key).Bool = true;
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
				return Notifications.StatusOf(Events.GC_UserAuthentication) == EventStatus.Success;
			}
		}
		const double EVENT_BASED_ACHIEVEMENT = -1;
		public void ReportProgress(UserData data) {
			var achieved = SavedData.Get (data).Bool;
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


		void OnApplicationQuit() {
			ReportScore(GameData.current.PercentageOfDevTimeWastedX10000, UserData.PercentageOfDevTime);

		}


	}
}