using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRandomly : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField]
    private AIEntity _aI;

    [SerializeField]
    private GameObject _bulletPrefab;

    #endregion

    #region private variables

    List<GameObject> _bulletPool = new List<GameObject>();

    List<GameObject> _enemiesInLevel = new List<GameObject>();

    float _bulletPoolSize = 50f;

    int _currentPoolIndex = 0;

    Quaternion _fireAngle;

    GameObject _player;

    AudioSource _audioSource;

    #endregion

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _audioSource = GetComponent<AudioSource>();

        //The AI will be using an object pool to contain all its possible bullets, which will be reused to save on resources.
        initializeBulletPool();


    }

    private void OnEnable()
    {
        //Reset the values back to default
        _aI._resetValues();
    }

    void initializeBulletPool() 
    {
        //Create a new object to contain the pooled bullet objects inside...
        GameObject _aiBulletPool = new GameObject();

        _aiBulletPool.name = _aI.name + "BulletPool";

        _aiBulletPool.transform.position = Vector3.zero;

        //Give it a name and zero out its position...

        for (int i = 0; i < _bulletPoolSize; i++)
        {
            GameObject a = Instantiate(_bulletPrefab);

            _bulletPool.Add(a);

            a.SetActive(false);

            a.transform.parent = _aiBulletPool.transform;
        }

        //Then iterate for the size of the bullet pool creating an instance each time of the bullet prefab, storing it inside the pool.
    }

    void Update()
    {
        //Coroutine as there is a rate of fire, so it doesn't need to be constantly checked and called ahead of everything else.
        StartCoroutine("_shootAtTarget");
    }

    IEnumerator _shootAtTarget()
    {
        //Initialize a temporary variable
        Vector3 v_angleToTarget = new Vector3(0,0,0);

        //If the associated object has a certain value then calulate the angle from this gameobject to the appropriate target.
        switch (_aI._target) 
        {
            case AIEntity.target.player:

                v_angleToTarget = _player.transform.position - transform.position;

                break;

            case AIEntity.target.enemy:

                /*
                 * If the AIEntity is the drone, then it will check to see
                 * what the closest enemy gameobject is to itself. It will then caculate the angle
                 * to this object based on positioning.
                 * 
                 */

                v_angleToTarget = _findNearestEnemy() - transform.position;

                break;
        }

        //Add in the accuracy field to influence the aim angle.
        v_angleToTarget = m_randomizeShootAngleCalculator(v_angleToTarget, _aI._accuracy);

        //Ensure the angle has 0 in the y rotation
        v_angleToTarget.y = 0;

        //Convert the angle to a Quaternion to be used in the final coroutine for shooting ht bullet.
        _fireAngle = Quaternion.LookRotation(v_angleToTarget);

        //Check to see if the rate of fire should be decreasing or remaining the same.
        _aI._rateOfFire = (!_aI._destroyed) ? _aI._rateOfFire - Time.deltaTime : _aI._originalRateOfFire;

        //Reset Rate of fire
        if (_aI._rateOfFire <= 0)
        {
            _aI._rateOfFire = _aI._originalRateOfFire;

            _audioSource.PlayOneShot(_aI._fireAudio);

            //Call the method that controls the object pool output.
            _useNextBullet();
        }

        yield return null;

    }

    /*
     * Simply gets all 'enemy' tagged gameobjects into an array and just cycle through
     * it to find the closest curret gameobject.
     */
    Vector3 _findNearestEnemy() 
    { 
        _enemiesInLevel.Clear();

        _enemiesInLevel.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        float v_distanceToClosestEnemy = Mathf.Infinity;

        Vector3 _target = Vector3.zero;

        foreach (GameObject enemy in _enemiesInLevel)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < v_distanceToClosestEnemy)
            {
                _target = enemy.transform.position;

            }
        }
        return _target;
    }

    /*
     * Using a variable to represent the index in the array to be used
     * enable the gameobject at that position and index the index so that next use will simply
     * use the enxt pooled object.
     * 
     * Ensure that if the index goes higher the array legnth it just cycles back to the begining.
     */
    public void _useNextBullet()
    {
        _currentPoolIndex = (_currentPoolIndex > _bulletPool.Count - 1) ? 0 : _currentPoolIndex;

        _bulletPool[_currentPoolIndex].SetActive(true);

        _bulletPool[_currentPoolIndex].transform.position = transform.position;

        _bulletPool[_currentPoolIndex].transform.rotation = _fireAngle;

        _currentPoolIndex++;
    }


    //Determine accuracy by generating a random float and use the accuracy to influence this float. If the accuracy is high then the resuling value will be low. 
    Vector3 m_randomizeShootAngleCalculator(Vector3 _fireAngle, float _accuracy)
    {
        float v_y = _fireAngle.y;

        _fireAngle.x = _fireAngle.x + (1 - _accuracy) * Random.Range(-20, 20);

        _fireAngle.y = _fireAngle.y + (1 - _accuracy) * Random.Range(-20, 20);

        _fireAngle.z = _fireAngle.z + (1 - _accuracy) * Random.Range(-20, 20);

        return _fireAngle;
    }
}
