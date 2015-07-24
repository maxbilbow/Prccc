using UnityEngine;
using UnityStandardAssets;
using System.Collections;
using UnityEngine.UI;
namespace RMX {
	public class GoAwayText : MonoBehaviour {


		Text label;
		public string text = "Stop wasting time!";
		public FontStyle fontStyle = FontStyle.Normal;
		public bool bestFit = false;
		public Color color = Color.white;
	

		void Start () {
			label = GetComponentInChildren<Text> ();
			if (label == null) {
				label = gameObject.AddComponent<Text> ();
				label.font = LookAndFeel.current.mainFont;
				label.fontSize = 62;
				label.fontStyle = fontStyle;
				label.lineSpacing = 1;
				label.supportRichText = true;
				label.alignment = TextAnchor.MiddleCenter;
				label.horizontalOverflow = HorizontalWrapMode.Wrap;
				label.verticalOverflow = VerticalWrapMode.Truncate;
				label.resizeTextForBestFit = bestFit;
				label.color = color;
//				label.material = null;
				label.text = text;
			}
			Hide ();
		}
//		// Use this for initialization
//		void Start () {
//
//		}
		
		// Update is called once per frame
		void Update () {
			if (ClockBehaviour.VisibleClockCount == 0) {
				Show ();
			} else {
				Hide ();
			}
		}

		public void Show() {
			label.enabled = true;
		}
		public void Hide() {
			label.enabled = false;
		}

	}
}