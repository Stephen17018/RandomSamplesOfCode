using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(SphereCollider))]
public class GrabManager : MonoBehaviour
{
    [SerializeField]
    private SteamVR_Input_Sources InputSource;
    [SerializeField]
    private SteamVR_Action_Boolean GrabButton;
    [SerializeField]
    private SteamVR_Action_Boolean UseButton;

    [SerializeField]
    private UnityEvent AdditionalOnTouchMethod, AdditionalOnGrabMethod, AdditionalOnInteractMethod;

    private Rigidbody rb;

    private GameObject ConnectedObject;
    private bool Touching, connected = false;
    private List<GameObject> NearObjects = new List<GameObject>();
    private Vector3 ObjectsLastPosition;
    private float ObjectsSpeed, throwMultiplier = 500f;
    private Transform OldParent;
    private void Start()
    {
        if (!transform.Find("SnapPosition")) {
            CreateSnapPosition();
        }
        rb = GetComponent<Rigidbody>();
        GetComponent<SphereCollider>().radius = 0.025f;
        GetComponent<SphereCollider>().isTrigger = true;
        rb.useGravity = false;
        rb.isKinematic = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            NearObjects.Clear();
            NearObjects.Add(other.gameObject);
            Touching = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(8))
        {
            Touching = false;
        }
        
    }

    private void Update()
    {
       if (Touching)
       {
            AdditionalOnTouchMethod.Invoke();
            if (GrabButton.GetState(InputSource))// Check if we want to drop the object
            {
                Grab();
            }
            else
            {
                Release();
            }
            if (UseButton.GetState(InputSource) && ConnectedObject != null)// Check if we want to drop the object
            {
                if (ConnectedObject.GetComponent<ObjectAttributes>() && ConnectedObject.GetComponent<ObjectAttributes>().Interactable)
                {
                    ConnectedObject.GetComponent<ObjectAttributes>().InvokeInteraction();
                    AdditionalOnInteractMethod.Invoke();
                }
            }
        }

        if (ConnectedObject != null)
        {
            ObjectsLastPosition = ConnectedObject.transform.position;
        }



        
    }

    void CreateSnapPosition()
    {
        GameObject SnapPosition = new GameObject();
        SnapPosition.transform.parent = transform;
        SnapPosition.name = "SnapPosition";
        SnapPosition.transform.localPosition = new Vector3(0,0,0);
        SnapPosition.transform.localEulerAngles = new Vector3(-65, 0,0);
    }

    void Release()
    {
        if (ConnectedObject != null)
        {
            GetComponent<FixedJoint>().connectedBody = null;
            GetComponent<ConfigurableJoint>().connectedBody = null;
            ConnectedObject.transform.parent = null;
            Vector3 throwVector = ConnectedObject.transform.position - ObjectsLastPosition;
            ConnectedObject.GetComponent<Rigidbody>().AddForce(throwVector * throwMultiplier, ForceMode.Impulse);
            ConnectedObject = null;
            connected = false;
        }
        
    }

    void Grab()
    {
        if (ClosestGrabbable() != null)
        {
            if (ClosestGrabbable().GetComponent<ObjectAttributes>() && ClosestGrabbable().GetComponent<ObjectAttributes>().Grabbable == true)
            {
                if (ClosestGrabbable().GetComponent<ObjectAttributes>().SnapToPosition)
                {
                    if (!connected) {
                        ConnectedObject = ClosestGrabbable();
                        ConnectedObject.transform.parent = transform;
                        ConnectedObject.transform.localRotation = transform.Find("SnapPosition").transform.localRotation;
                        ConnectedObject.transform.localPosition = transform.Find("SnapPosition").transform.localPosition;
                        GetComponent<FixedJoint>().connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                        GetComponent<ConfigurableJoint>().connectedBody = ConnectedObject.GetComponent<Rigidbody>();
                        connected = true;
                    }
                    AdditionalOnGrabMethod.Invoke();
                }
                else
                {
                    GetComponent<FixedJoint>().connectedBody = ClosestGrabbable().GetComponent<Rigidbody>();
                    GetComponent<ConfigurableJoint>().connectedBody = ClosestGrabbable().GetComponent<Rigidbody>();
                    ClosestGrabbable().transform.parent = transform;
                    ConnectedObject = ClosestGrabbable();
                    AdditionalOnGrabMethod.Invoke();
                }

            }         
        }
    }

    GameObject ClosestGrabbable()
    {
        //find the object in our list of grabbable that is closest and return it.
        GameObject ClosestGameObj = null;
        float Distance = float.MaxValue;
        if (NearObjects != null)
        {
            foreach (GameObject GameObj in NearObjects)
            {
                if ((GameObj.transform.position - transform.position).sqrMagnitude < Distance)
                {
                    ClosestGameObj = GameObj;
                    Distance = (GameObj.transform.position - transform.position).sqrMagnitude;
                }
            }
        }
        return ClosestGameObj;
    }
}
