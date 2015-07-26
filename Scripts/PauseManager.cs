using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

using RMX;  

namespace Procrastinate.Depricated {
	public class PauseManager : MonoBehaviour {

//		Text text1, text2;



//		void Awake() {
//			if (Timer.pauseManager == null) {
//				DontDestroyOnLoad (gameObject);
//				Timer.pauseManager = this;
//			} else if (Timer.pauseManager != this) {
//				Destroy (gameObject);
//			}
//		}
//

		public void Quit()
		{
			#if UNITY_EDITOR 
			EditorApplication.isPlaying = false;
			#else 
			Application.Quit();
			#endif
		}
		

	}
}