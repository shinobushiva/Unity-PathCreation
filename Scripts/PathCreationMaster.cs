using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataSaveLoad;

namespace PathCreation
{
	public class PathCreationMaster : MonoBehaviour
	{

		public Transform target;
		public Camera moveAlongCamera;
		private float moveAlongSpeed = 1;

		public float MoveAlongSpeed {
			set {
				moveAlongSpeed = value;
			}
			get { 
				return moveAlongSpeed;
			}
		}

		public DataSaveLoadMaster dataSaveLoad;


		// Use this for initialization
		void Start ()
		{
			dataSaveLoad.loadDataUI.dataLoadHandler += DataLoadCallback;
		}

		public void DataLoadCallback (object o)
		{
			StartPathCreation ();

			List<Point> list = o as List<Point>;
			foreach (Point p in list) {
				AddPathPoint (p.position, p.rotation);
			}
		}

		public void ShowSaveUI ()
		{
			
			dataSaveLoad.ShowSaveDialog (currentPathCreating);
		}
		
		public void ShowLoadUI ()
		{
			dataSaveLoad.ShowLoadDialog (typeof(List<Point>));
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		[System.Serializable]
		public class Point
		{
			public Vector3 position;
			public Quaternion rotation;
			[System.Xml.Serialization.XmlIgnore]
			public PathNode
				pn;
		}

		private List<Point> currentPathCreating;
		private GameObject creatingPathObj;

		public void StartPathCreation ()
		{
			if (creatingPathObj) {
				Destroy (creatingPathObj);
			}

			currentPathCreating = new List<Point> ();
			creatingPathObj = new GameObject ("Created Path");
		}

		public void AddPathPoint ()
		{
			if (currentPathCreating == null)
				StartPathCreation ();

			AddPathPoint (target.position, target.rotation);
		}

		public PathNode CreateNode (Vector3 pos, Quaternion rot, Point p)
		{
			p.position = pos;
			p.rotation = rot;

			GameObject po =  GameObject.CreatePrimitive (PrimitiveType.Sphere);
			po.transform.position = pos;
			po.transform.rotation = rot;
			po.transform.localScale = Vector3.one * 0.5f;
			po.transform.SetParent (creatingPathObj.transform, false);
			po.GetComponent<Collider> ().isTrigger = true;
			PathNode pn = po.AddComponent<PathNode> ();
			p.pn = pn;
			pn.point = p;
			po.AddComponent<DraggableObject> ();
			pn.master = this;

			return pn;
		}

		public void DeletePoint(Point p){
			currentPathCreating.Remove (p);
		}

		public void InsertPointAfter(Point i, Point p){
			int idx = currentPathCreating.IndexOf (i);
			currentPathCreating.Insert (idx+1, p);
		}

		public PathLink CreateLink (Point p1, Point p2)
		{
			GameObject li = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
			Destroy (li.GetComponent<Collider> ());
			li.transform.SetParent (creatingPathObj.transform, false);
			PathLink pl = li.AddComponent<PathLink> ();
			pl.pn1 = p1.pn;
			pl.pn2 = p2.pn;
			p1.pn.AddPathLink (pl);
			p2.pn.AddPathLink (pl);
			pl.Adjust ();

			return pl;
		}

		public void AddPathPoint (Vector3 pos, Quaternion rot)
		{
			Point p = new Point ();
			currentPathCreating.Add (p);

			CreateNode (pos, rot, p);

			if (currentPathCreating.Count >= 2) {
				int len = currentPathCreating.Count;
				Point p1 = currentPathCreating [len - 2];
				Point p2 = currentPathCreating [len - 1];

				CreateLink (p1, p2);

			}
		}

		private IEnumerator _MoveAlong (Transform t)
		{
			if (currentPathCreating.Count < 2)
				yield break;

			int idx = 0;
			while (idx < currentPathCreating.Count-1) {
				Point p1 = currentPathCreating [idx];
				Point p2 = currentPathCreating [idx + 1];

				float len = Vector3.Distance (p2.position, p1.position);

				float eTime = (Time.time + len);

				float time = Time.time;

				while (time < eTime) {
					float delta = Time.deltaTime * moveAlongSpeed;
					float f = 1f - (eTime - time) / len;
					t.position = Vector3.Lerp (p1.position, p2.position, f);
					t.rotation = Quaternion.Lerp (t.rotation, p2.rotation, Time.deltaTime);

					time += delta;

					yield return new WaitForEndOfFrame ();
				}
				idx++;
			}
		}

		public void MoveAlong (Transform t)
		{
			StartCoroutine (_MoveAlong (t));
		}

		public void MoveAlong ()
		{
			MoveAlong (moveAlongCamera.transform);
		}

		public void EndPathCreation ()
		{
			ShowSaveUI ();
		}

		public void DeleteSelectedPathNode(){
			if(currentSelected != null)
				currentSelected.Delete ();
		}

		public void AddForwardToSelectedPathNode(){
			if(currentSelected != null)
				currentSelected.AddForward ();
		}

		private PathNode currentSelected;

		public void PathNodeSelected(PathNode pn){
			if (currentSelected != null) {
				currentSelected.gameObject.GetComponent<Renderer> ().material.color = Color.gray;
			}

			currentSelected = pn;
			currentSelected.gameObject.GetComponent<Renderer> ().material.color = Color.yellow;
		}
	}
}