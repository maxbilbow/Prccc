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

using RMX;  

namespace Procrastinate {
	public class GameCenter : RMX.Singletons.ASingleton<GameCenter> {


		void Start() {
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner (true);
			Authenticate ();
//			TimeBasedAchievementsShouldUpdate ();


		

		
			UpdateGameCenterAchievements ();
			GameController.current.willPauseOnLoad = SavedData.Get<float>(UserData.gd_current_session) > 0;
			if (GameController.current.willPauseOnLoad) {
				GameController.current.PauseGame(true, null);
			}

		}


		void UpdateGameCenterAchievements () { 
			var log = "";
			foreach (KeyValuePair<UserData,string> id in UniqueID) {
				if (id.Key.ToString ().StartsWith ("ach_")) {
					var successInSD = SavedData.Get<bool> (id.Key);
					try {
						var successInGC = CheckAchievementsWithGameCenter (id.Key);
						if (successInSD != successInGC) 
							DidCauseEvent (Events.GC_AchievementGained, id.Key);
						log += " => " + id.Key + " SD: " + successInSD + ", GC: " + successInGC + "\n";
					
					} catch {
						log += " => " + id.Key + " Should be in GC: " + successInSD + "\n";
					}
				}
				if (Bugger.WillLog (Tests.Misc, log))
					Debug.Log (Bugger.Last);
			}
//			foreach (KeyValuePair<UserData,string> id in UniqueID) {
//				if (!CheckAchievementsWithGameCenter (id.Key) && SavedData.Get<bool> (id.Key)) 
//					ReportProgress (id.Key);
//
//			}
		}

		public override void OnEventDidStart(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.PauseSession))
				Authenticate ();
		}

		public override void OnEventDidEnd(IEvent theEvent, object info) {
			if (theEvent.IsType(Events.PauseSession)) {
				UpdateGameCenterAchievements ();
				ReportScore(SavedData.Get<long>(UserData.gd_current_procrastination), UserData.sc_longest_procrastination);
			} 
		}

		public override void OnEvent(IEvent theEvent, object info) {
			if (theEvent.IsType (Events.GC_AchievementGained))
				if (info is UserData) {
					var key = (UserData)info;
					SavedData.Set (key, true);
					ReportProgress (key);
					if (Bugger.WillLog (Testing.EventCenter, "Achievement Gained: " + info.ToString ()))
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
						userInfo += "\nUsername: " + Social.localUser.userName + 
							"\nUser ID: " + Social.localUser.id + 
							"\nIsUnderage: " + Social.localUser.underage;
					} else {
						DidFinishEvent (Events.GC_UserAuthentication, EventStatus.Failure);
						userInfo += "Authentication failed";
					}
				});


			} else {
				userInfo += "Authentication already completed";
			}
			if (Bugger.WillLog(Testing.GameCenter, userInfo))
				Debug.Log (Bugger.Last);
		}



		public void ReportScore (long score, UserData data) {
			if (UserAuthenticated && score > 0) {
				string leaderboardID = UniqueID [data];
				var log = ""; var feature = Testing.GameCenter;
				log += "Reporting score " + score + " on leaderboard " + leaderboardID;
				try {
					Social.ReportScore (score, leaderboardID, success => {
						log += success ? ": Reported score successfully" : ": Failed to report score";	
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






		// Update is called once per frame
		float _checkTime = 0;
		void Update () {

			if (Time.fixedTime > _checkTime) {
				var log = "";
				foreach (UserData key in System.Enum.GetValues(typeof(UserData)))
				{
					try {
						log += " => "+ key + ": " + SavedData.Get<bool> (key) + ", First Time: " +  HasMetTimeCriteria (key) + "\n";

					} catch {
//						log += " => "+ key + ": is NOT a time-based achievement\n";
					}
				}
				if (Bugger.WillLog(Tests.Achievements, log))
					Debug.Log(Bugger.Last);
				_checkTime = Time.fixedTime + GameController.current.updateScoresEvery;
			}
		}


		public static bool HasPlayerAlreadyAchieved(UserData key) {
			return SavedData.Get<bool>(key);
		}

		const float MINUTES = 60f, HOURS = 60 * 60f;

		/// <summary>
		/// Determines if has met time criteria the specified key.
		/// </summary>
		/// <returns><c>true</c> iff the criteria is met for the first time otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		public static bool HasMetTimeCriteria(UserData key) {
			var totalTime = SavedData.Get<float>(UserData.gd_total_time_Wasted);
			if (SavedData.Get<bool> (key))
				return false;
			var result = false;
			switch (key) {
			case UserData.ach_ameteur_crastinator:
				result = totalTime > 20;
				break;
			case UserData.ach_time_waster:
				result = totalTime > (10 * MINUTES);
				break;
			case UserData.ach_semi_pro:
				result = totalTime > (GameController.current.TotalDevTimeWasted / 4);
				break;
			case UserData.ach_apathetic:
				result = totalTime > (GameController.current.TotalDevTimeWasted / 2);
				break;
			case UserData.ach_pro_crastinator:
				result = totalTime > GameController.current.TotalDevTimeWasted ;//gameData.PercentageOfDevTimeWasted;	
				break;
			default:
				throw new Exception(key + " Has not ben accounded for in HasMetTimeCriteria(UserData key)");
			}
	
			if (result) {// && result != SavedData.Get (key).Bool) { 
				NotificationCenter.EventDidOccur (Events.GC_AchievementGained, key);
				SavedData.Set(key, true);
				return true;
			} else {
				return false; 
			}

		}



		static bool UserAuthenticated {
			get {
				return NotificationCenter.StatusOf(Events.GC_UserAuthentication) == EventStatus.Success;
			}
		}

		public void ReportProgress(UserData data) {
			var achieved = SavedData.Get<bool> (data);
			var log = data.ToString() + " => Reporting Progress: " + achieved; var feature = Testing.Achievements;

			float progress = achieved ? 100 : 0;

			if (progress < 0) {
				Debug.LogError(data.ToString() + " Tried to log as time based achievement but failed with " + progress);
				return;
			}
			
			if (UserAuthenticated) {
				#if UNITY_IOS || UNITY_STANDALONE_OSX
				try {
					GKAchievementReporter.ReportAchievement(UniqueID[data], progress, true);
					log += " => SUCCESS";
				} catch (Exception e){
					if (Bugger.WillLog(Testing.Exceptions,e.Message) || Bugger.WillLog(Testing.Achievements,e.Message) )
						Debug.Log(Bugger.Last);
				}
				#else
				try {
					Social.ReportProgress (UniqueID[data], progress, result => {
						log += ", result: " + result;
						if (result) {
							log += "\n => SUCCESS";
						} else {
							log += "\n => Achievement Failed to report";
						}
					});
				} catch (Exception e){
					if (Bugger.WillLog(Testing.Exceptions,e.Message) || Bugger.WillLog(Testing.Achievements,e.Message) )
						Debug.Log(Bugger.Last);
				}
				#endif
				log += " - New status isCompleted: " + progress;
				
			}
			if (Bugger.WillLog(feature,log) )
				Debug.Log(Bugger.Last);

		}

		bool CheckAchievementsWithGameCenter(UserData key) {
			var achievementID = UniqueID [key];
			var isComplete = false;
			var log = "";
			if (UserAuthenticated) { //TODO: Check this works
				try {
					Social.LoadAchievements (achievements => {
						if (achievements.Length > 0) {
							log += "Got " + achievements.Length + " achievement instances: ";
							foreach (IAchievement achievement in achievements) {
								if ( achievement.id == achievementID) {
									isComplete = achievement.completed || achievement.percentCompleted == 100;
									log += "\nAchievement " + achievement.id + ", progresss: " + achievement.percentCompleted + ", complete: " + achievement.completed;
									break;
								}
							}
						} else {
							if (Bugger.WillLog(Testing.Achievements,log) )
								Debug.Log(Bugger.Last);
							throw new System.ArgumentException ("No achievements returned");
						}
					});
				} catch (System.ArgumentException e) {
					if (Bugger.WillLog(Testing.Exceptions,e.Message) || Bugger.WillLog(Testing.Achievements,e.Message) )
						Debug.Log(Bugger.Last);
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
			{ UserData.sc_longest_procrastination, _grp + "CgkI2PKS_coeEAIQAw" },//"55415446";
			{ UserData.sc_total_as_percent_of_dev, _grp + "CgkI2PKS_coeEAIQCA" },
			{ UserData.ach_ameteur_crastinator, _grp + "CgkI2PKS_coeEAIQAQ" },
			{ UserData.ach_time_waster, _grp + "CgkI2PKS_coeEAIQBA" },
			{ UserData.ach_apathetic, _grp + "CgkI2PKS_coeEAIQBw" },
			{ UserData.ach_semi_pro, _grp + "CgkI2PKS_coeEAIQBQ" },
			{ UserData.ach_pro_crastinator, _grp + "CgkI2PKS_coeEAIQBg" },
			{ UserData.ach_making_time, _grp + "CgkI2PKS_coeEAIQCQ" },
			{ UserData.ach_big_time, _grp + "CgkI2PKS_coeEAIQDA" },
			{ UserData.ach_overtime, _grp + "CgkI2PKS_coeEAIQDQ" }
		};


		void OnApplicationQuit() {
			ReportScore(GameData.PercentageOfDevTimeWastedX10000, UserData.sc_total_as_percent_of_dev);

		}


	}
}