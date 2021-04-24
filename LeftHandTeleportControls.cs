using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandTeleportControls : MonoBehaviour
{
    public float turnSpeed = 100f;
    public GameObject Head,Avatar, TeleportMarker;
    public bool Tele = false;
    Vector3 TelePos;
    private void Start()
    {
        TeleportMarker.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 8;

        Vector3 euler = transform.rotation.eulerAngles;
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        euler.y += secondaryAxis.x;
        Avatar.transform.RotateAround(Head.transform.position, Vector3.up, secondaryAxis.x * turnSpeed * Time.deltaTime);

        RaycastHit hit;
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {

            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, Vector3.forward * Mathf.Infinity, Color.red);

                if (hit.collider.gameObject.tag.Equals("Floor"))
                {

                    TeleportMarker.SetActive(true);
                    TeleportMarker.transform.position = hit.point;
                    Tele = true;
                    TelePos = new Vector3(hit.point.x, Avatar.transform.position.y, hit.point.z);
                }
                else
                {
                    TeleportMarker.SetActive(false);
                    Tele = false;
                }

                if (hit.collider.gameObject.tag.Equals("Interactable"))
                {

                    hit.collider.gameObject.SendMessage("Action");
                }

            }

        }
        else
        {
            if (Tele)
            {
                Vector3 temp = new Vector3(Head.transform.localPosition.x/2f, 0, Head.transform.localPosition.z/2f);
                Avatar.transform.position = TelePos;// + temp;
                TeleportMarker.SetActive(false);
                Tele = false;
            }

        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, Vector3.forward * Mathf.Infinity, Color.red);

            if (hit.collider.gameObject.tag.Equals("Floor"))
            {

                TeleportMarker.SetActive(true);

                TeleportMarker.transform.position = hit.point;
            }
            else
            {
                TeleportMarker.SetActive(false);

                Tele = false;
            }
            if (hit.collider.gameObject.tag.Equals("Interactable"))
            {

                hit.collider.gameObject.SendMessage("HighLight");
            }


        }


    }
}
