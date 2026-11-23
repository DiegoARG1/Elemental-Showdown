using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class PrewarmItem
    {
        public GameObject prefab;
        public int initialCount = 5;
    }

    [Header("Pre-carga al iniciar (Opcional)")]
    public List<PrewarmItem> itemsToPrewarm = new List<PrewarmItem>();

    private Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PrewarmAll();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PrewarmAll()
    {
        foreach (var item in itemsToPrewarm)
        {
            if (item.prefab == null) continue;

            for (int i = 0; i < item.initialCount; i++)
            {
                GameObject obj = CreateNewObject(item.prefab);
                obj.SetActive(false);
            }
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new List<GameObject>();
        }

        List<GameObject> objectList = pools[prefab];

        // Buscamos un objeto inactivo disponible para reciclar
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] != null && !objectList[i].activeInHierarchy)
            {
                GameObject recycled = objectList[i];
                recycled.transform.SetPositionAndRotation(position, rotation);
                recycled.SetActive(true);
                return recycled;
            }
        }

        // Si todos estan en uso, creamos uno nuevo y lo registramos
        GameObject newObj = CreateNewObject(prefab);
        newObj.transform.SetPositionAndRotation(position, rotation);
        newObj.SetActive(true);
        return newObj;
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new List<GameObject>();
        }

        GameObject obj = Instantiate(prefab, transform);
        pools[prefab].Add(obj);
        return obj;
    }
}