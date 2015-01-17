using UnityEngine;
using System.Collections;

public class TwoDeePhysics : MonoBehaviour {

	public bool debug = false;

	//basic properties, in units/second
	float acceleration = 4f;
	float maxSpeed = 150f;
	float gravity = 6f;
	float maxFall = 160f;
	float jump = 200f;

	//a layer make set in Start()
	int layerMask;

	Rect box;

	Vector3 velocity;

	bool grounded = false;
	bool falling = false;


	int horizontalRays = 6;
	int verticalRays = 4;
	float margin = .2f;


	// Use this for initialization
	void Start () {
		layerMask = LayerMask.NameToLayer ("normalCollisions");
		velocity = Vector3.zero;
	}

	void FixedUpdate()
	{
		box = new Rect 
			(
				collider.bounds.min.x,
				collider.bounds.min.y,
				collider.bounds.size.x,
				collider.bounds.size.y
			);


		if (!grounded)
						velocity = new Vector3 (velocity.x, Mathf.Max ((velocity.y - (gravity)), -maxFall), velocity.z);
		if (velocity.y < 0)
						falling = true;
		if (grounded || falling) 
		{
		 Vector3 startPoint = new Vector3(box.xMin + margin,box.center.y, transform.position.z);
			Vector3 endPoint = new Vector3(box.xMax - margin,box.center.y, transform.position.z);

			RaycastHit hitInfo;
			float distance = box.height/2 + (grounded? margin: Mathf.Abs((velocity.y) * Time.deltaTime));


			bool connected = false;

			for(int i = 0; i < verticalRays;++i)
			{
				Vector3 starter =startPoint;
				starter.y += velocity.y * Time.deltaTime;
				Vector3 ender = starter;
				ender.y += distance;

				float lerpAmount = (float) i/ (float) (verticalRays -1);
				Vector3 origin = Vector3.Lerp(starter,endPoint,lerpAmount);
				Ray ray = new Ray(origin, Vector3.down);
			

				connected = Physics.Raycast(ray, out hitInfo, distance, layerMask);
				if(debug)
					Debug.DrawRay(starter,Vector3.down,Color.green);

				if(connected)
				   {
					grounded = true;
					falling = false;
					transform.Translate(Vector3.down * (hitInfo.distance - box.height/2));
					velocity = new Vector3 (velocity.x,0,velocity.z);
					break;
				}
			}

				if(!connected)
				{
					grounded = false;
				}

		}
	}

	
	// Update is called once per frame
	void Update () {
		if(transform.position.y < .9f)
			Debug.Break();
	}

	void LateUpdate()
	{

		transform.Translate (velocity * Time.deltaTime);
	}
}
