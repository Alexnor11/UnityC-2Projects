using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Коллекции в C# стр. 424

public class CubeSpawner3 : MonoBehaviour
{
    public GameObject cubePrefabVar;
    public List<GameObject> gameObjectList;
    public float scalingFactor = 0.95f;
    public int numCubes = 0;

    void Start()
    {
        gameObjectList = new List<GameObject>();
    }

    void Update()
    {
        numCubes++;
        GameObject gObj = Instantiate<GameObject>(cubePrefabVar);

        gObj.name = "Cube " + numCubes;
        Color c = new Color(Random.value, Random.value, Random.value);
        gObj.GetComponent<Renderer>().material.color = c;
        gObj.transform.position = Random.insideUnitSphere;

        gameObjectList.Add(gObj);

        List<GameObject> RemoveList = new List<GameObject>();

        foreach (GameObject goTemp in gameObjectList)
        {
            float scale = goTemp.transform.localScale.x;
            scale *= scalingFactor;
            goTemp.transform.localScale = Vector3.one * scale;

            if(scale <= 0.1f)
            {
                RemoveList.Add(goTemp);
            }
        }

        foreach(GameObject goTemp in RemoveList)
        {
            gameObjectList.Remove(goTemp);
            Destroy(goTemp);
        }
    }
}
