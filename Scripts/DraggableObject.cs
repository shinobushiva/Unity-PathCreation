using UnityEngine;
using System.Collections;

public class DraggableObject : MonoBehaviour {

	Camera currentCamera;

	public GameObject target;

	// Use this for initialization
	void Start () {
		currentCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private Vector3 screenPoint;
	private Vector3 offset;
	
	void OnMouseDown() {
		this.screenPoint = currentCamera.WorldToScreenPoint(transform.position);
		this.offset = transform.position - currentCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		gameObject.SendMessage ("MousePress", SendMessageOptions.DontRequireReceiver);
	}
	
	void OnMouseDrag() {
		Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 currentPosition = currentCamera.ScreenToWorldPoint(currentScreenPoint) + this.offset;
		transform.position = currentPosition;
		gameObject.SendMessage ("MouseDragged", SendMessageOptions.DontRequireReceiver);
	}

	void OnMouseUp(){
		gameObject.SendMessage ("MousePressed", SendMessageOptions.DontRequireReceiver);
	}
}
