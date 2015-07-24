using System;
using System.Collections;
using UnityEngine;

namespace RMX
{
    public class DragRigidbody : ASingleton<DragRigidbody>
    {
		private const float PI_OVER_180 = Mathf.PI/180;
		const float k_Spring = 50.0f;
		const float k_Damper = 5.0f;
		const float k_Drag = 10.0f;
		const float k_AngularDrag = 5.0f;
		const float k_Distance = 0.2f;
        const bool k_AttachToCenterOfMass = false;
		float fingerWidth { 
			get {
				return settings.FingerSize;
			}
		}

		private GameObject finger;

        private SpringJoint2D m_SpringJoint;

        private void Update()
        {
            // Make sure the user pressed the mouse down
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
			var mainCamera = Camera.main;
            // We need to actually hit an object
			RaycastHit2D hit = Physics2D.Raycast (mainCamera.ScreenPointToRay (Input.mousePosition).origin,
			                                     mainCamera.ScreenPointToRay (Input.mousePosition).direction);
			if (!m_SpringJoint)
			{
				var go = new GameObject("Rigidbody dragger");
				Rigidbody2D body = go.AddComponent<Rigidbody2D>();
				m_SpringJoint = go.AddComponent<SpringJoint2D>();
				body.isKinematic = true;
			}


			// We need to hit a rigidbody that is not kinematic
			if (hit && hit.rigidbody.isKinematic)
			{
				//				print ("We need to hit a rigidbody that is not kinematic! ");
				return;
			}

            if (!hit || m_SpringJoint.connectedBody != null) //Stops clothsline effect for now.
            {
				if (!finger) {
					finger = new GameObject("Finger");
//					Rigidbody2D body = finger.AddComponent<Rigidbody2D>();
					CircleCollider2D collider = finger.AddComponent<CircleCollider2D>();
//					body.isKinematic = true;
					collider.radius = fingerWidth;
					collider.sharedMaterial = new PhysicsMaterial2D();
					collider.sharedMaterial.bounciness = 0.5f;
					finger.SetActive (false);
				}
//				print ("We need to actually hit an object");
				finger.SetActive (true);
				finger.transform.position = mainCamera.ScreenPointToRay (Input.mousePosition).origin;
				StartCoroutine("MoveFinger", 0);// mainCamera.ScreenPointToRay (Input.mousePosition).direction);
                return;
            }
          

            
			//Todo: create a new joint for additional objects in scene. This will be fun.

            m_SpringJoint.transform.position = hit.point;
            m_SpringJoint.anchor = Vector2.zero;

            m_SpringJoint.frequency = k_Spring;
            m_SpringJoint.dampingRatio = k_Damper * hit.transform.localScale.magnitude;
            m_SpringJoint.distance = k_Distance;
			m_SpringJoint.connectedBody = hit.rigidbody;

			var theta = m_SpringJoint.connectedBody.rotation * -PI_OVER_180;
			var anchor = hit.point - m_SpringJoint.connectedBody.position;

			var cosø = Mathf.Cos (theta);
			var sinø = Mathf.Sin (theta);
			var x = cosø * anchor.x - sinø * anchor.y;
			var y = sinø * anchor.x + cosø * anchor.y;

			m_SpringJoint.connectedAnchor = new Vector2 (x, y);//(hit.point - m_SpringJoint.connectedBody.position);
	

            StartCoroutine("DragObject", hit.distance);
//			print (this);
        }

		void AttachBody(Rigidbody2D body, Touch touch, float distance) {
//			StopAllCoroutines ();
			var point2D = touch.position;
			var point3D = new Vector3 (point2D.x, point2D.y, distance);
			var point3DWorld = Camera.main.ScreenToWorldPoint (point3D);
			var point = new Vector2 (point3DWorld.x, point3DWorld.y);// Physics2D.Raycast (Camera.main.ScreenPointToRay (touch.position),
//			                               Camera.main.ScreenPointToRay (touch.deltaPosition));
			m_SpringJoint.transform.position = point;
			m_SpringJoint.anchor = Vector2.zero;
			
			m_SpringJoint.frequency = k_Spring;
			m_SpringJoint.dampingRatio = k_Damper * body.transform.localScale.magnitude;
			m_SpringJoint.distance = k_Distance;
			m_SpringJoint.connectedBody = body;
			
			var theta = m_SpringJoint.connectedBody.rotation * -PI_OVER_180;
			var anchor = point - m_SpringJoint.connectedBody.position;
			
			var cosø = Mathf.Cos (theta);
			var sinø = Mathf.Sin (theta);
			var x = cosø * anchor.x - sinø * anchor.y;
			var y = sinø * anchor.x + cosø * anchor.y;
			
			m_SpringJoint.connectedAnchor = new Vector2 (x, y);//(hit.point - m_SpringJoint.connectedBody.position);
			
			
			StartCoroutine("DragObject", distance);
		}

		public override string ToString ()
		{
			return  "\njoint anchor: " + m_SpringJoint.connectedAnchor +
//			        "\nHit Position: " + hit.point +
					"\nObj Position: " + m_SpringJoint.connectedBody.position +
			        "\n  Hit Anchor: " + m_SpringJoint.anchor ;
		}

        private IEnumerator DragObject(float distance)
        {
            var oldDrag = m_SpringJoint.connectedBody.drag;
            var oldAngularDrag = m_SpringJoint.connectedBody.angularDrag;
            m_SpringJoint.connectedBody.drag = k_Drag;
            m_SpringJoint.connectedBody.angularDrag = k_AngularDrag;
			var mainCamera = Camera.main;

            while (Input.GetMouseButton(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                m_SpringJoint.transform.position = ray.GetPoint(distance);
                yield return null;
            }
            if (m_SpringJoint.connectedBody)
            {
                m_SpringJoint.connectedBody.drag = oldDrag;
                m_SpringJoint.connectedBody.angularDrag = oldAngularDrag;
                m_SpringJoint.connectedBody = null;
            }
        }


		private IEnumerator MoveFinger(float distance)
		{
			var mainCamera = Camera.main;

			while (Input.GetMouseButton(0))
			{
				var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
				finger.transform.position = ray.GetPoint(distance);
				yield return null;
			}
			if (finger.activeSelf)
			{
				GameController.CheckForAnomalies();
				finger.SetActive (false);
			}
		}

		public override void OnEventDidEnd (Event theEvent, object args)
		{
			switch (theEvent) {
			case Event.SpawnInflatableClock:
				if (args is ClockBehaviour) {
					AttachBody((args as ClockBehaviour).body, Input.GetTouch(Input.touchCount - 1),0);
				}
				break;
			}
		}
    }
}
