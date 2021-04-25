using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionManager : MonoBehaviour
{

    #region Serialized Variables

    [SerializeField]
    Transform CameraPositions;

    [SerializeField]
    Camera View_Cam;
    [SerializeField]
    float camera_Transition_Speed = 1f;

    [SerializeField]
    int current_Room = 0;

    [SerializeField]
    int current_Rooms_View = 0;

    [SerializeField]
    List<Transform> View_Arrays = new List<Transform>();

    [SerializeField]
    List<Transform> Current_Camera_Positions = new List<Transform>();

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in CameraPositions) 
        {
            if (child.parent == CameraPositions) 
            {
                View_Arrays.Add(child);
            }
        }


        foreach (Transform t in View_Arrays[0])
        {
            Current_Camera_Positions.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Current_Camera_Positions.Count != View_Arrays[current_Room].childCount-1) 
        {
            Current_Camera_Positions.Clear();

            foreach (Transform t in View_Arrays[current_Room])
            {
                Current_Camera_Positions.Add(t);
            }
        }

        cameraTargetMovement(Current_Camera_Positions[current_Rooms_View].position, Current_Camera_Positions[current_Rooms_View].rotation);
       
        current_Rooms_View = Mathf.Clamp(current_Rooms_View,0, Current_Camera_Positions.Count - 1);
        if (current_Rooms_View > Current_Camera_Positions.Count-1) 
        {
            current_Rooms_View = Current_Camera_Positions.Count;
        }
        if (current_Rooms_View < 0)
        {
            current_Rooms_View = 0;
        }

        current_Rooms_View = Mathf.Clamp(current_Rooms_View, 0, View_Arrays[current_Room].childCount);
        if (current_Rooms_View < 0) 
        {
            current_Rooms_View = 0;
        }
        else if (current_Rooms_View > View_Arrays[current_Room].childCount)
        {
            current_Rooms_View = View_Arrays[current_Room].childCount;
        }
    }

    void cameraTargetMovement(Vector3 Pos, Quaternion Rot) 
    {

        View_Cam.gameObject.transform.position = Vector3.Lerp(View_Cam.gameObject.transform.position, Pos, camera_Transition_Speed * Time.deltaTime);

        View_Cam.gameObject.transform.rotation = Quaternion.Lerp(View_Cam.gameObject.transform.rotation, Rot, camera_Transition_Speed * Time.deltaTime);

    }

    public void ProgressToNextRoom() 
    {
        current_Room++;
    }

    public void ProgressToPreviousRoom() 
    {
        current_Room--;
    }

    public void ProgressToNextCameraPosition() 
    {
        current_Rooms_View++;
        if (current_Rooms_View < 0)
        {
            current_Rooms_View = 0;
        }
        else if (current_Rooms_View > View_Arrays[current_Room].childCount)
        {
            current_Rooms_View = View_Arrays[current_Room].childCount;
        }
    }
}
