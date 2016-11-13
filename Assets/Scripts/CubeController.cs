using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour {
	public float speed = 500;
	private Rigidbody body;

	// Use this for initialization
	void Start () {
		body = GetComponent <Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		body.AddForce (new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * speed);
	}

	void OnFound() {
		Debug.Log ("found!");
		gameObject.transform.position = new Vector3 (0, 100, 0);
	}
}
