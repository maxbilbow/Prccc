using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using RMX;  

namespace Procrastinate.Depricated {

	public static class Timer { // : RMX.Singletons.ASingleton<Timer> {



		public static string GetTimeDescription(float timeInSeconds) {
			var seconds = timeInSeconds;//PlayerPrefs.GetFloat(GameData.GetKey(key));
			int minutes = (int) seconds / 60;
			int hours = minutes / 60;
			seconds = Mathf.Round(seconds % 60);
			string result = "";
			if (hours > 0) {
				result += hours + " hours, ";
			}
			if (minutes > 0) {
				result += minutes + " minutes and ";
			}
			if (seconds > 1) {
				result += seconds + " seconds.";
			} else {
				result += "not a second longer.";
			}
			return result;
		}

	
	 	





	}
}