using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidPooler : MonoBehaviour
{
    #region Serilized Variables

    [SerializeField]
    private GameObject _largeAsteroidPrefab, 
        _mediumAsteroidPrefab, 
        _smallAsteroidPrefab;

    //Maximum of 5 large asteroids, these in turn mean each must have 2 medium or 4 small asteroids for each large
    //This means 10 medium and 20 small
    [SerializeField]
    private float _largeAsteroidsAvailable = 5, 
        _mediumAsteroidsAvailable = 10, 
        _smallAsteroidsAvailable = 20;

    [SerializeField]
    int _currentPoolIndex = 0;

    #endregion

    #region Private variables
    List<GameObject> v_largeAsteroidPool = new List<GameObject>();

    List<GameObject> v_mediumAsteroidPool = new List<GameObject>();

    List<GameObject> v_smallAsteroidPool = new List<GameObject>();

    int _currentlyInUseAsteroids = 0;

    #endregion

    void Awake()
    {
        #region All size asteroid pool Initilization

        _largePoolInitilzation();

        _mediumPoolInitilzation();

        _smallPoolInitilzation();

        #endregion

    }

    //Method to create and fill the large asteroid pool
    void _largePoolInitilzation() 
    {
        //Create a pool gameobject...
        GameObject v_largeAsteroidPoolParent = new GameObject();

        v_largeAsteroidPoolParent.transform.parent = transform;

        v_largeAsteroidPoolParent.transform.name = "Large Asteroid Pool Parent";


        //For the required amount of interations, instance the appropriate prefab and fill the gameobject with child instances.
        for (int i = 0; i <= _largeAsteroidsAvailable - 1; i++)
        {
            GameObject a = Instantiate(_largeAsteroidPrefab);
            v_largeAsteroidPool.Add(a);
            a.SetActive(false);
            a.transform.parent = v_largeAsteroidPoolParent.transform;
        }
    }

    //Repeat to create and fill the medium asteroid pool
    void _mediumPoolInitilzation()
    {
        GameObject v_mediumAsteroidPoolParent = new GameObject();

        v_mediumAsteroidPoolParent.transform.parent = transform;

        v_mediumAsteroidPoolParent.transform.name = "Medium Asteroid Pool Parent";

        for (int i = 0; i <= _mediumAsteroidsAvailable - 1; i++)
        {
            GameObject a = Instantiate(_mediumAsteroidPrefab);
            v_mediumAsteroidPool.Add(a);
            a.SetActive(false);
            a.transform.parent = v_mediumAsteroidPoolParent.transform;
        }

    }

    //Repeat to create and fill the small asteroid pool
    void _smallPoolInitilzation()
    {
        GameObject v_smallAsteroidPoolParent = new GameObject();

        v_smallAsteroidPoolParent.transform.parent = transform;

        v_smallAsteroidPoolParent.transform.name = "Small Asteroid Pool Parent";

        for (int i = 0; i <= _mediumAsteroidsAvailable - 1; i++)
        {
            GameObject a = Instantiate(_smallAsteroidPrefab);
            v_smallAsteroidPool.Add(a);
            a.SetActive(false);
            a.transform.parent = v_smallAsteroidPoolParent.transform;
        }
    }

    /*
     * public method that is called when asteroids need to be added into the scene.
     * 
     * p_poolSize is used to represent the size of the asteroid 
     * 
     * 0 = large asteroid to spawn
     * 1 = medium asteroid to spawn
     * 2 = small asteroid to spawn
     * 
     * p_amount is the amount of asteroids of the specified size to spawn
     * 
     * p_randomizeSpawn is the determine if the asteroids should be spawned at a random postion, or if the should spawn 
     * somewhere specific.
     * 
     * p_pos is the vector3 value that will be used in the event that P_randomizeSpawn is false, such as when a large asteroid is destroyed and needs
     * two smaller ones to spawn directly at its location.
     */
    public void m_spawnAsteroids(int p_poolSize, int p_amount, bool p_randomizeSpawn, Vector3 p_pos = default(Vector3))
    {        
        List<GameObject> v_chosenPool = new List<GameObject>();

        _currentlyInUseAsteroids = _currentlyInUseAsteroids + p_amount;


        /*Assign the appropriate pool to a local variable.
         */

        switch (p_poolSize)
        {
            case 0:
                v_chosenPool = v_largeAsteroidPool;
                break;
            case 1:
                v_chosenPool = v_mediumAsteroidPool;
                break;
            case 2:
                v_chosenPool = v_smallAsteroidPool;
                break;
        }

        //Check and cap pool index to prevent errors
        _currentPoolIndex = (_currentPoolIndex >= v_chosenPool.Count - 1) ? 0 : _currentPoolIndex;

        /*
         * Iterate through the pool for a specified amount of times (using p_amout or the amount of asteroids told to spawn.)
         * 
         */
        for (int i = 0; i < p_amount; i++)
        {
            if (v_chosenPool[_currentPoolIndex] != null)
            {
                v_chosenPool[_currentPoolIndex].SetActive(true);

                v_chosenPool[_currentPoolIndex].transform.position = (p_randomizeSpawn) ?
                            new Vector3(
                            Random.Range(Random.Range(-35, -25), Random.Range(25, 35)),
                            3,
                            Random.Range(Random.Range(-35, -25), Random.Range(25, 35))) : p_pos;
            }

            //Increase pool index to spawn available asteroids on next iteration.
            _currentPoolIndex++;
        }
    }
        

    //Public method to remove all asteroids from the game scene if called.
    public void m_disableAllAsteroids() 
    {

        //Just iterate through each asteroid pool and set them all to zero.
        _currentlyInUseAsteroids = 0;

        for (int i = 0; i <= v_largeAsteroidPool.Count - 1; i++)
        {
            if (v_largeAsteroidPool[i] != null)
                v_largeAsteroidPool[i].SetActive(false);
        }

        for (int i = 0; i <= v_mediumAsteroidPool.Count - 1; i++)
        {
            if (v_mediumAsteroidPool[i] != null)
                v_mediumAsteroidPool[i].SetActive(false);
        }

        for (int i = 0; i <= v_smallAsteroidPool.Count - 1; i++)
        {
            if (v_smallAsteroidPool[i] != null)
                v_smallAsteroidPool[i].SetActive(false);
        }

    }
}
