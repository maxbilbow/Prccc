using UnityEngine;
using System.Collections;

namespace RMX {
	public abstract class ABonus<TComponent> : MonoBehaviour
	where TComponent : Object {

//		public enum UserData {
//			CurrentSession, CurrentProcrastination, Total
		//		}
		private bool isActive = false;
		public bool deactivateOnPause = true;
		public UserData key;
		protected float threshold = 0;
		public float min = 30;
		public float max = 45;
		protected TComponent component;
		public SavedData data {
			get {
				return SavedData.Get(key);
			}
		}
		public float probability = 0.5f;
		
		// Use this for initialization
		protected virtual void SetComponent(TComponent component) {
			this.component = component;
		}
		void Start () {
			threshold = min > max ? min : Random.Range (min, max);
//			key = GameData.GetKey (data);
			try {
				SetComponent(GetComponent<TComponent> ());
			} catch {
				SetComponent(null);
			} finally {
				if (component == null) {
					print ("Warning: component could not be set at start! Consider overriding SetComponent method.");
				}
			}
			Deactivate ();

		}

		protected virtual void OnApplicationFocus(bool focus) {
			if (!focus && deactivateOnPause) {
				Deactivate();
			}
		}


		// Update is called once per frame
		void Update () {
			if (!isBonusActive && data.Float > threshold) {
				Activate ();
			}
		}
		
		public virtual void Activate () {
			isActive = true;
		}
		
		public virtual void Deactivate () {
			isActive = false;
		}
		
		public virtual bool isBonusActive {
			get {
				return isActiveAndEnabled && isActive;
			}
		}
			

	}
}