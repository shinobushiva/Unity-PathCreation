using UnityEngine;
using System.Collections;
using PathCreation;

public class DraggableObject : MonoBehaviour {

	public PathCreationMaster master;


	public GameObject target;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private Vector3 screenPoint;
	private Vector3 offset;
	
	void OnMouseDown() {
		this.screenPoint = master.cameraSwitcher.CurrentActive.c.WorldToScreenPoint(transform.position);
		this.offset = transform.position - master.cameraSwitcher.CurrentActive.c.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		gameObject.SendMessage ("MousePress", SendMessageOptions.DontRequireReceiver);
	}
	
	void OnMouseDrag() {
		Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 currentPosition = master.cameraSwitcher.CurrentActive.c.ScreenToWorldPoint(currentScreenPoint) + this.offset;
		transform.position = currentPosition;
		gameObject.SendMessage ("MouseDragged", SendMessageOptions.DontRequireReceiver);
	}

	void OnMouseUp(){
		gameObject.SendMessage ("MousePressed", SendMessageOptions.DontRequireReceiver);
	}
}
