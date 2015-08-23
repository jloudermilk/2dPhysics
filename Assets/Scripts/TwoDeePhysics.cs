using UnityEngine;
using System.Collections;

public class TwoDeePhysics : MonoBehaviour
{

    

    public bool debug = false;
    public bool playerControlled = false;
    public Vector3 connectdistance;

    //basic properties, in units/second
    public float acceleration = 4f;
    public float maxSpeed = 150f;
    public float gravity = 6f;
    public float maxFall = 160f;

    public float jumpPressedTime = 0;
    public float jumpPressLeeway = .3f;
    public float jump = 200f;
    public Vector3 velocity;

    bool lastInput = true;

    //a layer make set in Start()
    int layerMask;

    Rect box;

    
    bool grounded = false;
    bool falling = false;
    float distance;
    float verticalAxis;
    float horizontalAxis;


    public int horizontalRays = 6;
    public int verticalRays = 4;
    public float margin = .2f;


    // Use this for initialization
    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("normalCollisions");
        velocity = Vector3.zero;
    }

    void Update()
    {

        box = new Rect
            (
                GetComponent<Collider>().bounds.min.x,
                GetComponent<Collider>().bounds.min.y,
                GetComponent<Collider>().bounds.size.x,
                GetComponent<Collider>().bounds.size.y
            );

        #region Y Axis Movement

        if (!grounded)
            velocity.y = Mathf.Max((velocity.y - (gravity)), -maxFall);
        if (velocity.y < 0)
            falling = true;

        if (grounded || falling)
        {
            Vector3 startPoint = new Vector3(box.xMin + margin, box.center.y, transform.position.z);
            Vector3 endPoint = new Vector3(box.xMax - margin, box.center.y, transform.position.z);

            RaycastHit[] hitInfo = new RaycastHit[verticalRays];
            //i had a x2 in there for some reason
            distance = (box.height / 2) + (grounded ? margin : Mathf.Abs((velocity.y) * Time.deltaTime));


            bool connected = false;
            float shortestRay = Mathf.Infinity;
            int indexUsed = -1;
            for (int i = 0; i < verticalRays; ++i)
            {
                Vector3 starter = startPoint;
                starter.y += velocity.y * Time.deltaTime;


                float lerpAmount = (float)i / (float)(verticalRays - 1);
                Vector3 origin = Vector3.Lerp(startPoint, endPoint, lerpAmount);
                Ray ray = new Ray(origin, Vector3.down);


                connected = Physics.Raycast(ray, out hitInfo[i], distance, layerMask);
                if (debug)
                    Debug.DrawRay(origin, Vector3.down, Color.green);

                if (connected)
                    if (hitInfo[i].distance < shortestRay)
                    {
                        indexUsed = i;
                        shortestRay = hitInfo[i].distance;
                    }
            }
            if (connected)
            {
                grounded = true;
                falling = false;
                connectdistance = Vector3.down * (hitInfo[indexUsed].distance - box.height / 2);
                transform.Translate(Vector3.down * (hitInfo[indexUsed].distance - box.height / 2));
                velocity = new Vector3(velocity.x, 0, velocity.z);
            }
            else
            {
                grounded = false;
            }

        }
        #endregion

        #region X Axis Movement
        if (playerControlled)
        {
            horizontalAxis = Input.GetAxisRaw("Horizontal");
        }
        float newVelocityX = velocity.x;
        if (horizontalAxis != 0)
        {
            newVelocityX += acceleration * horizontalAxis;
            newVelocityX = Mathf.Clamp(newVelocityX, -maxSpeed, maxSpeed);
        }
        else if (velocity.x != 0)
        {

            newVelocityX = 0;
        }


        velocity.x = newVelocityX;
        if (velocity.x != 0)
        {
            Vector3 startPoint = new Vector3(box.center.x, box.yMin + margin, transform.position.z);
            Vector3 endPoint = new Vector3(box.center.x, box.yMax - margin, transform.position.z);

            RaycastHit hitInfo;

            float sideRayLength = (box.width / 2) + Mathf.Abs(newVelocityX * Time.deltaTime);
            Vector3 direction = newVelocityX > 0 ? Vector3.right : Vector3.left;

            bool connected = false;

            for (int i = 0; i < horizontalRays; ++i)
            {
                float lerpAmout = ((float)i / (float)(horizontalRays - 1));
                Vector3 origin = Vector3.Lerp(startPoint, endPoint, lerpAmout);
                Ray ray = new Ray(origin, direction);

                connected = Physics.Raycast(ray, out hitInfo, sideRayLength);
                if (debug)
                    Debug.DrawRay(origin, direction, Color.green);
                if (connected)
                {
                    transform.Translate(direction * (hitInfo.distance - box.width / 2));
                    velocity.x = 0;

                }
            }
        }
        #endregion

        #region Z Axis Movement
        if (playerControlled)
        {
            verticalAxis = Input.GetAxisRaw("Vertical");
        }
        float newVelocityZ = velocity.z;
        if (verticalAxis != 0)
        {
            newVelocityZ += acceleration * verticalAxis;
            newVelocityZ = Mathf.Clamp(newVelocityZ, -maxSpeed, maxSpeed);
        }
        else if (velocity.z != 0)
        {

            newVelocityZ = 0;
        }

        velocity.z = newVelocityZ;
        if (velocity.z != 0)
        {
            Vector3 startPoint = new Vector3(box.center.x, box.yMin + margin, transform.position.z);
            Vector3 endPoint = new Vector3(box.center.x, box.yMax - margin, transform.position.z);

            RaycastHit hitInfo;

            float sideRayLength = (box.width / 2) + Mathf.Abs(newVelocityZ * Time.deltaTime);
            Vector3 direction = newVelocityZ > 0 ? Vector3.forward : Vector3.back;

            bool connected = false;

            for (int i = 0; i < horizontalRays; ++i)
            {
                float lerpAmout = ((float)i / (float)(horizontalRays - 1));
                Vector3 origin = Vector3.Lerp(startPoint, endPoint, lerpAmout);
                Ray ray = new Ray(origin, direction);

                connected = Physics.Raycast(ray, out hitInfo, sideRayLength);
                if (debug)
                    Debug.DrawRay(origin, direction, Color.green);
                if (connected)
                {
                    transform.Translate(direction * (hitInfo.distance - box.width / 2));
                    velocity.z = 0;
                    break;
                }
            }
        }
        #endregion

        #region Jump

        bool input = false;

        if (playerControlled)
        {
            input = Input.GetButton("Jump");
        }
             if (input && !lastInput)
        {
            jumpPressedTime = Time.time;
        }
        else if (!input)
        {
            jumpPressedTime = 0;
        }
        if (grounded && (Time.time - jumpPressedTime) < jumpPressLeeway && Time.time > jumpPressLeeway)
        {
            velocity.y = jump;
            jumpPressedTime = 0;
        }

        #endregion

        lastInput = input;
    }

    void LateUpdate()
    {

        transform.Translate(velocity * Time.deltaTime);
    }
}
