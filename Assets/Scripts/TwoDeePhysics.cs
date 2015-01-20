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
						velocity.y = Mathf.Max ((velocity.y - (gravity)), -maxFall);
		if (velocity.y < 0)
						falling = true;
		if (grounded || falling) 
		{
		 Vector3 startPoint = new Vector3(box.xMin + margin,box.center.y, transform.position.z);
			Vector3 endPoint = new Vector3(box.xMax - margin,box.center.y, transform.position.z);

			RaycastHit hitInfo;
			float distance = box.height/2 + (grounded? margin: Mathf.Abs((velocity.y) * (2*Time.deltaTime)));


			bool connected = false;

			for(int i = 0; i < verticalRays;++i)
			{
				Vector3 starter =startPoint;
				starter.y += velocity.y * Time.deltaTime;
				Vector3 ender = starter;
				ender.y += distance;

				float lerpAmount = (float) i/ (float) (verticalRays -1);
				Vector3 origin = Vector3.Lerp(startPoint,endPoint,lerpAmount);
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
		float horizontalAxis = Input.GetAxisRaw("Horizontal");

		float newVelocityX = velocity.x;
		if(horizontalAxis != 0)
		{
			newVelocityX += acceleration * horizontalAxis;
			newVelocityX = Mathf.Clamp(newVelocityX,-maxSpeed,maxSpeed);
		}
		else if(velocity.x != 0)
		{
			int modifiter = velocity.x > 0? -1: 1;
			newVelocityX += acceleration * modifiter;
		}

		velocity.x = newVelocityX;
		if(velocity.x !=0)
		{
			Vector3 startPoint = new Vector3(box.center.x,box.yMin + margin, transform.position.z);
			Vector3 endPoint = new Vector3(box.center.x,box.yMax - margin, transform.position.z);

			RaycastHit hitInfo;

			float sideRayLength = (box.width/2) + Mathf.Abs(newVelocityX * Time.deltaTime);
			Vector3 direction =  newVelocityX > 0 ? Vector3.right: Vector3.left;

			bool connected = false;

			for(int i = 0; i < horizontalRays; ++i)
			{
				float lerpAmout = ((float) i / (float)(horizontalRays - 1));
				Vector3 origin = Vector3.Lerp(startPoint,endPoint,lerpAmout);
				Ray ray = new Ray(origin,direction);

				connected = Physics.Raycast(ray,out hitInfo,sideRayLength);
				if(debug)
					Debug.DrawRay(origin,direction,Color.green);
				if(connected)
				{
					transform.Translate(direction *(hitInfo.distance - box.width/2));
					velocity.z =0;
					break;
				}
			}
		}
		float verticalAxis = Input.GetAxisRaw("Vertical");
		
		float newVelocityZ = velocity.z;
		if(verticalAxis != 0)
		{
			newVelocityZ += acceleration * verticalAxis;
			newVelocityZ = Mathf.Clamp(newVelocityZ,-maxSpeed,maxSpeed);
		}
		else if(velocity.z != 0)
		{
			int modifiter = velocity.z > 0? -1: 1;
			newVelocityZ += acceleration * modifiter;
		}
		
		velocity.z = newVelocityZ;
		if(velocity.x !=0)
		{
			Vector3 startPoint = new Vector3(box.center.x,box.yMin + margin, transform.position.z);
			Vector3 endPoint = new Vector3(box.center.x,box.yMax - margin, transform.position.z);
			
			RaycastHit hitInfo;
			
			float sideRayLength = (box.width/2) + Mathf.Abs(newVelocityZ * Time.deltaTime);
			Vector3 direction =  newVelocityZ > 0 ? Vector3.forward: Vector3.back;
			
			bool connected = false;
			
			for(int i = 0; i < horizontalRays; ++i)
			{
				float lerpAmout = ((float) i / (float)(horizontalRays - 1));
				Vector3 origin = Vector3.Lerp(startPoint,endPoint,lerpAmout);
				Ray ray = new Ray(origin,direction);
				
				connected = Physics.Raycast(ray,out hitInfo,sideRayLength);
				if(debug)
					Debug.DrawRay(origin,direction,Color.green);
				if(connected)
				{
					transform.Translate(direction *(hitInfo.distance - box.width/2));
					velocity.z =0;
					break;
				}
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
