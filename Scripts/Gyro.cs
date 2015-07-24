using UnityEngine;
using System.Collections;

namespace RMX {
	public class Gyro : ASingleton<Gyro> {

	 	void LateUpdate () 
		{
			Physics2D.gravity = new Vector2 (Input.acceleration.x, Input.acceleration.y) * 9.81f;
		}
	
	}
}
