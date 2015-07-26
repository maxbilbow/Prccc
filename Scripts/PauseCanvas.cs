using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using RMX; 

namespace Procrastinate {
	public class PauseCanvas : Singletons.ASingleton<PauseCanvas>  {

		Canvas canvas;
		Button infoButton;
		// Canvas Variables
		public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
		public bool pixelPerfect = false;

		// CanvasScala vars
		public CanvasScaler.ScaleMode uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
		public CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points;
		public float fallbackScreenDPI = 96f;
		public float defaultSpriteDPI = 96f;
		public float referencePixelsPerUnit = 100f;

		// Raycaster Variables
		public bool ignoreReversedGraphics = true;
		public GraphicRaycaster.BlockingObjects blockingObjects = GraphicRaycaster.BlockingObjects.None;
//		public GraphicRaycaster.BlockingObjects blockingObjects = GraphicRaycaster.BlockingObjects.None;

		// Image Variables
		public Sprite sourceImage;
		public Color color = Color.white;
		public Material material;
		public bool preserveAspect = false;

		// Triggers
		public List<EventTrigger.Entry> triggers = new List<EventTrigger.Entry>() {
			{ new Trigger() }
		};

		static void UnPauseGame(BaseEventData data) {
			GameController.current.PauseGame (false, null);
		}

		class Trigger : EventTrigger.Entry {

			public Trigger() {
				eventID = EventTriggerType.PointerDown;
				callback = new Callback();
				callback.AddListener(UnPauseGame);
			}
			               
		}

		class Callback : EventTrigger.TriggerEvent {

		}


		// Use this for initialization
		 void Start () {
		 	canvas = gameObject.GetComponent<Canvas> ();
			if (!canvas) {
				canvas = gameObject.AddComponent<Canvas> ();
				canvas.renderMode = renderMode;
				canvas.pixelPerfect = pixelPerfect;
				canvas.enabled = false;
			}


			if (!gameObject.GetComponent<CanvasScaler> ()) {
				var scalar = gameObject.AddComponent<CanvasScaler> ();
				scalar.uiScaleMode = uiScaleMode;
				scalar.physicalUnit = physicalUnit;
				scalar.fallbackScreenDPI = fallbackScreenDPI;
				scalar.defaultSpriteDPI = defaultSpriteDPI;
				scalar.referencePixelsPerUnit = referencePixelsPerUnit;
			}

			if (!gameObject.GetComponent<GraphicRaycaster> ()) {
				var raycaster = gameObject.AddComponent<GraphicRaycaster> ();
				raycaster.ignoreReversedGraphics = ignoreReversedGraphics;
				raycaster.blockingObjects = blockingObjects;
//				raycaster.blockingMask
			}

			if (!gameObject.GetComponent<CanvasRenderer> ()) {
//				var renderer = 
				gameObject.AddComponent<CanvasRenderer> ();
			}

			if (!gameObject.GetComponent<Image> ()) {
				var image = gameObject.AddComponent<Image> ();
				image.sprite = sourceImage;
				image.color = color;
				image.material = material;
				image.preserveAspect = preserveAspect;

			}

			if (!gameObject.GetComponent<EventTrigger> ()) {
				var trigger = gameObject.AddComponent<EventTrigger> ();
				trigger.triggers = triggers;
			}


			infoButton = GetComponentInChildren<Button> ();
			infoButton.onClick.AddListener (toggleInfo);
//			if (GameController.current.willPauseOnLoad) {
//				BuildWychd();
//			}

			if (SavedData.Get<bool>(UserData.gd_has_played_before) && SavedData.Get<float>(UserData.gd_current_session) > 0){
				GameController.current.PauseGame (true, Event.FirstPause);
			}	

//			_canvasReady = true;
		}

		static void toggleInfo() {
			current.information = !current.information;
		}

		bool information = false;	

		public void ShowInfo() {
			information = !information;
		}


		string _wychd;

//		void OnApplicationFocus(bool focusStatus) {
//			if (!focusStatus) {
//				PauseGame (true, null);
//			}
//		}

		void Update () {
			if (canvas != null)
				canvas.enabled = GameController.current.isPaused;
		}
		//		bool newSession = true;
		void OnGUI(){
			if (canvas.enabled) {
				if (_wychd == null) {// && _timeText == null) {
						BuildWychd(Event.FirstPause);
//						GameController.current.PauseGame (false, null);
						Debug.LogWarning("timeText not initialized - game unpaused");

				}

				string text = !information ? _wychd : "In total you've only managed to waste " + string.Format("{0:N2}%",GameData.PercentageOfDevTimeWasted) + 
							"\n of the time I've lost developing this game." +
							"\n\n Try again?";
				GUIStyle style = new GUIStyle ();
				style.fontSize = 50;
				style.richText = true;
				style.wordWrap = true;
				style.alignment = TextAnchor.MiddleCenter;
				style.padding.left = style.padding.right = style.padding.top = style.padding.bottom = 50;
				
				GUI.Label (new Rect (0, 0, Screen.width, Screen.height), text, style);
				
			} else if (information) {
				information = false;
			}
		}
		
		void BuildWychd (Event args = Event.NULL) {
			float time = 0; 
			var activities = GameData.WhatYouCouldHaveDone (time);
			
			if (args.Equals(Event.FirstPause)) {
				_wychd = "Congratulations. During your last session, you wasted ";
				time = SavedData.Get<float> (UserData.gd_current_session);
			} else {
				_wychd = "Congratulations. You have wasted ";
				time = SavedData.Get<float> (UserData.gd_current_procrastination);
			}
			
			_wychd += TextFormatter.TimeDescription (time);
			
			if (GameController.current.newPersonalBest) {
				_wychd += "\nA NEW PERSONAL BEST!";
				GameController.current.newPersonalBest = false;
			}
			
			_wychd += "\n\nDuring that time you could have " + activities [Random.Range (0, activities.Count)];
		}

		public override void OnEventDidStart(System.Enum theEvent, object info) {
			if (theEvent.Equals (RMX.Event.PauseSession)) {
				canvas.enabled = true;
				BuildWychd(info is Event ? (Event) info : Event.NULL);

			} else if (theEvent.Equals(RMX.Event.ResumeSession)) {
				canvas.enabled = false;
			}
		}
	
	}

}