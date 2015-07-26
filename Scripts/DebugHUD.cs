using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using RMX;  namespace Procrastinate {
	public class DebugHUD : RMX.Singletons.ASingleton<DebugHUD> {

		public GameObject showButton;
		public GameObject hideButton;
	

		// Use this for initialization
		void Start () {
			Hide ();
			if (!GameController.current.DebugHUD) {
				showButton.SetActive(false);
			}
		}

		bool _show = false;
		public void Show() {
			_show = true;
			showButton.SetActive (false);
			hideButton.SetActive (true);
//			debugPanel.transform.position = new Vector3 (-slideX, 0, 0);
//			info.text = "Width: " + Camera.main.pixelWidth.ToString();
		}

		public void Hide() {
			_show = false;
			showButton.SetActive (true);
			hideButton.SetActive (false);
//			debugPanel.transform.position = new Vector3 (slideX, 0, 0);
//			info.text = "Width: " + Camera.main.pixelWidth.ToString();
		}

		string GetTime(object key) {
			var time = SavedData.Get<float> (key);
			var timeString = time > 0 ? TextFormatter.TimeDescription (time) : time.ToString ();
			return "\n – " + key.ToString () + ": <color=yellow>" + timeString + "</color>";
		}
		// Update is called once per frame
		void OnGUI() {
			if (_show) {
				string info = "DEBUG =>";
				foreach (KeyValuePair<UserData,string> data in GameCenter.UniqueID) {
					var val = data.Key == UserData.sc_longest_procrastination ? GetTime(data.Key)
						: SavedData.Get<string>(data.Key).Length == 0 ? "False" 
						: SavedData.Get<string>(data.Key);
					info += "\n – " + data.Key.ToString() + ": " + val;
				}
				info += GetTime(UserData.gd_current_procrastination);
				info += GetTime(UserData.gd_current_session);
				info += GetTime(UserData.gd_total_time_Wasted);
				info += "\nClockSpawnMode: " + GameController.current.ClockSpawnMode;
				info += "\n" + DragRigidbody.current;

				GUIStyle style = new GUIStyle ();
//				
				style.richText = true;
				style.wordWrap = true;
				style.alignment = TextAnchor.UpperRight;
				style.padding.left = style.padding.right = style.padding.top = style.padding.bottom = 20;
				//				style.border
				GUI.Label (new Rect (0, 0, Screen.width, Screen.height), TextFormatter.Format(info,Time.timeScale == 0 ? "black" : "white"), style);

			}
		}
	}
}