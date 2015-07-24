using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace RMX {
	public class DebugHUD : ASingleton<DebugHUD> {

		public GameObject showButton;
		public GameObject hideButton;
	

		// Use this for initialization
		void Start () {
			Hide ();
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

		string GetTime(UserData key) {
			return "\n – " + key.ToString () + ": <color=yellow>" + Timer.GetTimeDescription(SavedData.Get (key).Float) + "</color>";
		}
		// Update is called once per frame
		void OnGUI() {
			if (_show) {
				string info = "DEBUG =>";
				foreach (KeyValuePair<UserData,string> data in GameCenter.UniqueID) {
					var val = data.Key == UserData.LongestProctrastination ? GetTime(data.Key)
						: SavedData.Get(data.Key).String.Length == 0 ? "False" 
						: SavedData.Get(data.Key).String;
					info += "\n – " + data.Key.ToString() + ": " + val;
				}
				info += GetTime(UserData.CurrentProcrastination);
				info += GetTime(UserData.CurrentSession);
				info += GetTime(UserData.TotalTime);

				GUIStyle style = new GUIStyle ();
//				style.fontSize = 50;
				style.richText = true;
				style.wordWrap = true;
				style.alignment = TextAnchor.UpperRight;
				style.padding.left = style.padding.right = style.padding.top = style.padding.bottom = 20;
				//				style.border
				GUI.Label (new Rect (0, 0, Screen.width, Screen.height), TextFormatter.Format(info), style);

			}
		}
	}
}