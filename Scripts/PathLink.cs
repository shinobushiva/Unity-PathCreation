using UnityEngine;
using System.Collections;

namespace PathCreation{
	public class PathLink : MonoBehaviour {

		Camera currentCamera;

		public PathNode pn1;
		public PathNode pn2;


		// Use this for initialization
		void Start () {
			GetComponent<Renderer> ().material.color = Color.green;

			currentCamera = Camera.main;
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void Adjust(){
			transform.position = (pn1.transform.position + pn2.transform.position)/2;
			transform.rotation = Quaternion.LookRotation((pn2.transform.position - pn1.transform.position).normalized, Vector3.forward);
			transform.Rotate(Vector3.right * 90f, Space.Self);
			transform.localScale = new Vector3(.2f, (pn2.transform.position - pn1.transform.position ).magnitude/2, .2f);
		}
	}
}