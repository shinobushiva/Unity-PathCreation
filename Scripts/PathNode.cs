using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathCreation{
	public class PathNode : MonoBehaviour {


		public List<PathLink> connectedLinks;

		public PathCreationMaster.Point point;
		public PathCreationMaster master;

		public void AddPathLink(PathLink pl ){
			if (!connectedLinks.Contains (pl)) {
				connectedLinks.Add(pl);
			}
		}


		// Use this for initialization
		void Awake () {
			GetComponent<Renderer> ().material.color = Color.gray;

			connectedLinks = new List<PathLink>();
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void MouseDragged(){
			foreach (PathLink pl in connectedLinks) {
				pl.Adjust();
			}
			Sync ();
		}

		void Sync(){
			point.position = transform.position;
		}



		public void MousePress(){
			master.PathNodeSelected (this);
		}

		public void Delete(){
			if (connectedLinks.Count == 0) {
				master.DeletePoint(this.point);
				Destroy (gameObject);
				return;
			}

			PathLink pl1 = null;
			PathLink pl2 = null;
			PathNode pn1 = null;
			PathNode pn2 = null;
			if (connectedLinks.Count >= 2) {
				pl1 = connectedLinks [0];
				pl2 = connectedLinks [1];

				if(pl1.pn1 == this){
					pn2 = pl1.pn2;
				}
				if(pl1.pn2 == this){
					pn1 = pl1.pn1;
				}
				if(pl2.pn1 == this){
					pn2 = pl2.pn2;
				}
				if(pl2.pn2 == this){
					pn1 = pl2.pn1;
				}

				pn1.connectedLinks.Remove (pl1);
				pn1.connectedLinks.Remove (pl2);
				pn2.connectedLinks.Remove (pl1);
				pn2.connectedLinks.Remove (pl2);

				pn1.AddPathLink(pl1);
				pn2.AddPathLink(pl1);
				pl1.pn1 = pn1;
				pl1.pn2 = pn2;
				pl1.Adjust();

				Destroy(pl2.gameObject);
				master.DeletePoint(this.point);
				Destroy (gameObject);
				return;
			}
			
			if (connectedLinks.Count == 1) {

				pl1 = connectedLinks [0];
				
				if(pl1.pn1 == this){
					pl1.pn2.connectedLinks.Remove(pl1);
				}
				if(pl1.pn2 == this){
					pl1.pn1.connectedLinks.Remove(pl1);
				}
				
				Destroy(pl1.gameObject);
				master.DeletePoint(this.point);
				Destroy (gameObject);
				return;
			}

		}

		public void AddForward(){
			if (connectedLinks.Count == 0) {
				
				Vector3 pos = transform.position + Vector3.one;
				Quaternion rot = transform.rotation;
				master.AddPathPoint(pos, rot);
				return;
			}

//			master.CreateNode(
			
			PathLink pl1 = null;
			PathLink pl2 = null;
			if (connectedLinks.Count >= 2) {
				pl1 = connectedLinks [0];
				pl2 = connectedLinks [1];

				if(pl2.pn1 == this){

					Vector3 pos = (pl2.pn2.transform.position + transform.position)/2;
					Quaternion rot = pl2.pn1.transform.rotation;

					PathCreationMaster.Point p = new PathCreationMaster.Point();
					master.InsertPointAfter(this.point, p);
					PathNode pn = master.CreateNode(pos, rot, p);

					pl2.pn1 = pn;
					pl1.pn2 = this;
					this.connectedLinks.Remove (pl2);
					pn.AddPathLink(pl2);

					master.CreateLink(this.point,pn.point);

					pl2.Adjust();
					pl1.Adjust();

				}
			}
			if (connectedLinks.Count == 1) {
				pl1 = connectedLinks [0];

				if(pl1.pn1 == this){
					
					Vector3 pos = (pl1.pn2.transform.position + transform.position)/2;
					Quaternion rot = pl1.pn1.transform.rotation;
					
					PathCreationMaster.Point p = new PathCreationMaster.Point();
					master.InsertPointAfter(this.point, p);
					PathNode pn = master.CreateNode(pos, rot, p);
					master.CreateLink(this.point,pn.point);
					
					pl1.pn1 = pn;
					pn.AddPathLink(pl1);
					this.connectedLinks.Remove (pl1);

					pl1.Adjust();
					
				}
				if(pl1.pn2 == this){
					
					Vector3 pos = pl1.pn2.transform.position + (pl1.pn2.transform.position - pl1.pn1.transform.position).normalized;
					Quaternion rot = pl1.pn2.transform.rotation;
					master.AddPathPoint(pos, rot);

				}
			}
		}
	}
}