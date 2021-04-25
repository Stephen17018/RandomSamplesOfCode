using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    /// <summary>
    /// This script is useful if you need different sources of gravity.
    /// The addition of functions collision/trigger detections would enable this script to change the gravity source on the fly.
    /// </summary>
    Rigidbody rb;

    [SerializeField]
    private GameObject GravitySource;

    [SerializeField]
    private float acceleration = 9.8f;

    [SerializeField]
    private float OrientationSpeed = 5f;

    //Tick this variable to set the gravity source as the parent of this gameobjcet. Useful for moving or rotating sources.
    [SerializeField]
    private bool MakeGravitySourceParent = false;

    [SerializeField]
    private bool MakeCollisionGravitySource = false;

    [SerializeField]
    private bool EnableOrentation = false;

    public bool TouchingGround = false;

    bool Gravity = true;

    void Start()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }


    void Update()
    {
        if (GravitySource != null) {

            if (MakeGravitySourceParent)
            {

                transform.parent = GravitySource.transform;

            }
            /*
             *In order for us to stick to flat surfaces like planes and not just fly towards the dead center of the object we must find the point on the source right below us and gravitate towards that instead.
             * 
             * Thats what Physics.closestPoint does. It will find the closest pont on a given collider to our gameObject and returns it as a Vector3. 
             * 
             * We then simply use that as our position to gravitate towards.
             */

            Vector3 GravityPoint = Physics.ClosestPoint(transform.position, GravitySource.GetComponent<Collider>(), GravitySource.transform.position, GravitySource.transform.rotation);
            //transform.position = Vector3.MoveTowards(transform.position, GravityPoint, acceleration * Time.deltaTime);
            
            rb.AddForce((GravityPoint - transform.position).normalized * acceleration * Time.deltaTime);





            /*
             * To orientate ourselves correctly we take this Gravity Point variable which is directly (in terms of the gravity) below us and use our own position to determine what vector would be needed to go straight 'down'.
             * 
             * We then use this new variable to calculate what vector we would need to go 'forwards'.
             * 
             * We combine these into a Quaternion which results in our gameobject orentating itself correctly to match the face that it is standing on on the gravity source.
             */

            if (EnableOrentation)
            {
                Vector3 down = (GravityPoint - transform.position).normalized;
                Vector3 forward = Vector3.Cross(transform.right, down);
                Quaternion lR = Quaternion.LookRotation(-forward, -down);
                transform.rotation = Quaternion.Lerp(transform.rotation, lR, OrientationSpeed * Time.deltaTime);
            }
            


            /*
             * REMEMBER THAT THIS ALL WORKS WELL BECAUSE THE GRAVITY SOURCE COLLIDER IS THE SAME SHAPE AS THE GRAVITY SOURCE MESH. A DIFFERENT SHAPE BETWEEN THE MESH & COLLIDER WILL RESULT
             * IN A MUCH WORSE LOOKING OUTCOME E.G.: .
             * */
        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("GravitySource") && MakeCollisionGravitySource)
        {
            GravitySource = collision.gameObject;
        }

        
    }






}
