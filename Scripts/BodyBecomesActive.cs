using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RMX;  namespace Procrastinate {


	public class BodyBecomesActive : ABonus<Rigidbody2D,float> {



	

		protected override void OnApplicationFocus(bool focus) {
			if (key.ToString() == UserData.gd_current_procrastination.ToString()) {
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