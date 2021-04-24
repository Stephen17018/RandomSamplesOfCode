using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(LineRenderer))]
public class SelectionManager : MonoBehaviour
{
    [SerializeField]

    private SteamVR_Input_Sources ControllerInput;
    [SerializeField]

    private SteamVR_Action_Boolean ToggleTriggerButton;

    [SerializeField]
    private Material BeamMaterial;

    private LineRenderer selectionBeam;

    private Transform Aimer;

    RaycastHit hit;

    void Start()
    {
        selectionBeam = GetComponent<LineRenderer>();
        selectionBeam.startWidth = selectionBeam.endWidth = 0.05f;
        selectionBeam.enabled = false;
        Aimer = transform.Find("SnapPosition").transform;
    }

    // Update is called once per frame
    void Update()
    {

        int layerMask = 1 << 8;
        if (Physics.Raycast(Aimer.position, -Aimer.transform.up, out hit, Mathf.Infinity, layerMask))
        {
            BuildBeam();
        }
        else
        {
            DestoryBeam();
        }


    }

    void BuildBeam()
    {
        selectionBeam.enabled = true;
        if (selectionBeam.material != BeamMaterial)
        {
            selectionBeam.material = BeamMaterial;
        }
        selectionBeam.SetPosition(0,Aimer.transform.position);
        selectionBeam.SetPosition(1, hit.point);
    }

    void DestoryBeam()
    {
        selectionBeam.enabled = false;
    }
}
