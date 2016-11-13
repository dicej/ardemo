/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using System.Collections;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class DefaultTrackableEventHandler : MonoBehaviour,
                                                ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES
 
        private TrackableBehaviour mTrackableBehaviour;

		private float elapsedSeconds = 0;
		const float scaleFactor = 30;

		struct Inclination {
			public float x;
			public float y;
			public float z;
			public float gravity;
		}

        #endregion // PRIVATE_MEMBER_VARIABLES

		void FixedUpdate () {
			elapsedSeconds += Time.deltaTime;
			if (elapsedSeconds > 0.5f) {
				elapsedSeconds = 0.0f;
				WWW www = new WWW("http://192.168.43.198:1337/inclination");
				StartCoroutine (WaitForRequest (www));
			}
		}

		IEnumerator WaitForRequest(WWW www) {
			yield return www;
			Inclination inclination = JsonUtility.FromJson<Inclination>(www.text);
			// adjust for sensor bias:
			inclination.y += 1;
			//inclination.x -= 1;
			Debug.Log ("inclination is " + inclination.x + ", " + inclination.y);
			gameObject.transform.rotation = Quaternion.Euler(-inclination.x, 0, -inclination.y);
			Physics.gravity = new Vector3(0, (float) (-inclination.gravity * scaleFactor), 0);
		}

        #region UNTIY_MONOBEHAVIOUR_METHODS
    
        void Start()
        {
			Physics.gravity = new Vector3(0, (float) (-9.8 * scaleFactor), 0);
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }

			//Debug.Log ("gravity is " + gravity);
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS



        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

			gameObject.BroadcastMessage ("OnFound");

			//Physics.gravity = gravity;

            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        }


        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable colliders:
			if (false) {
				foreach (Collider component in colliderComponents) {	
					component.enabled = false;
				}
			}

			//Physics.gravity = Vector3.zero;

            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        }

        #endregion // PRIVATE_METHODS
    }
}
