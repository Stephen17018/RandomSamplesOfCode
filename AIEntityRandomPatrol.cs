using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class AIEntityRandomPatrol : MonoBehaviour
{
    #region Seriazlied Fields

    [SerializeField]
    private AIEntity _aI;

    #endregion

    #region Private Variables

    float _individualWaitDuration;

    Vector3 _destination;
    #endregion

    void OnEnable()
    {
        randomizeStartPosition();

        _individualWaitDuration = _aI._waitDuration;
    }

    //At start place the entity into a random location.
    void randomizeStartPosition() 
    {
        //Limited to always start a certain distance away from the center so they don't spawn directly
        //on top of the player ship
        float x = Random.Range(Random.Range(-30,-15),Random.Range(15,30));

        float y = transform.position.y;

        float z = Random.Range(Random.Range(-30, -15), Random.Range(15, 30));

        transform.position = new Vector3(x,y,z);
    }

    void Awake()
    {
        _individualWaitDuration = _aI._waitDuration;
    }

    void Update()
    {
        StartCoroutine("translateToTarget");
    }

    //Coroutine to move the entity to the target location.
    IEnumerator translateToTarget() 
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, _aI._movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _destination) < 2f)
        {
            //After a certain time of being close to the destination...
            _individualWaitDuration = _individualWaitDuration - Time.deltaTime;

            if (_individualWaitDuration < 0)
            {
                //...find a new destination and reset the wait timer.
                _destination = findRandomPosition();

                _individualWaitDuration = _aI._waitDuration;
            }

        }

        yield return new WaitForSecondsRealtime(0.2f);
    }

    //Return a random vector 3 for the AIEntity to move towards
    Vector3 findRandomPosition() 
    {
        //Range limits it to -38 up to 38 on both the x and z axis to stop the entity from starting outside the play area.
        return new Vector3(Random.Range(-38,38), transform.position.y, Random.Range(-38, 38));
    }

}
