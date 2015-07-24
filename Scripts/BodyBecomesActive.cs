using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RMX {


	public class BodyBecomesActive : ABonus<Rigidbody2D> {



	

		protected override void OnApplicationFocus(bool focus) {
			if (key == UserData.CurrentProcrastination) {
				base.OnApplicationFocus(focus);
			}
		}

		public override void Activate() {
			base.Activate ();
			component.isKinematic = false;
		}

		public override void Deactivate() {
			try {
				component.isKinematic = true;
				base.Deactivate();
			} catch {
				print ("Warning: component could not be deactivated.");
			}
		}

		public override bool isBonusActive {
			get{
				return !component.isKinematic;
			}
		}

	}


}