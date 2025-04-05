using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;


public class RayCast_Script : MonoBehaviour
{
    public List<GameObject> spawn_prefab;

    List<GameObject> spawned_objects = new List<GameObject>();

    ARRaycastManager arrayman;

    public Button changeShapeButton;

    private int currentPrefabIndex = 0;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();


    //pinch to zoom in and zoom out
    Vector2 First_Touch;
    Vector2 Second_Touch;
    float Distance_Current;
    float Distance_Previous;
    bool First_Pinch = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arrayman = GetComponent<ARRaycastManager>();
        changeShapeButton.onClick.AddListener(ChangeShape);

    }

    // Update is called once per frame
    void Update()
    {
        if( Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed )
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Debug.Log("New Input System touch detected at: " + touchPosition);

            if (spawned_objects.Count > 0)
            {
                GameObject lastObject = spawned_objects[spawned_objects.Count - 1];
                Destroy(lastObject);
                spawned_objects.RemoveAt(spawned_objects.Count - 1); // Clean up the list
            }
            if (spawned_objects.Count == 0)
            {
                if (arrayman.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {

                    Pose hitpose = hits[0].pose;

                    int randomIndex = Random.Range(0, spawn_prefab.Count);
                    GameObject prefabToSpawn = spawn_prefab[randomIndex];
                    GameObject new_object = Instantiate(prefabToSpawn, hitpose.position, hitpose.rotation);

                    spawned_objects.Add(new_object);
                }
            }
            else
            {
                Debug.Log("No Valid surface for your objectPlacement");
            }
        }
        if(Touchscreen.current.touches.Count > 1 && spawned_objects.Count > 0 )
        {
           
            var touches =  Touchscreen.current.touches;

            

            First_Touch = touches[0].position.ReadValue();
            Second_Touch = touches[1].position.ReadValue();

            Distance_Current = Vector2.Distance(First_Touch, Second_Touch);

            if (First_Pinch)
            {
                Distance_Previous = Distance_Current;
                First_Pinch = false;
            }
            if(Distance_Current != Distance_Previous)
            {
                GameObject random_object = spawned_objects[spawned_objects.Count - 1];
                Vector3 Scale_Value = random_object.transform.localScale * (Distance_Current / Distance_Previous);
                random_object.transform.localScale = Scale_Value;
                Distance_Previous = Distance_Current;
            }
        }
        else
        {
            First_Pinch = true;
        }
    }

    void ChangeShape()
    {
        if (spawned_objects.Count > 0)
        {
            GameObject oldObject = spawned_objects[spawned_objects.Count - 1];
            Vector3 position = oldObject.transform.position;
            Quaternion rotation = oldObject.transform.rotation;
            Vector3 scale = oldObject.transform.localScale;

            Destroy(oldObject);
            spawned_objects.RemoveAt(spawned_objects.Count - 1);

            // Cycle through the list
            currentPrefabIndex = (currentPrefabIndex + 1) % spawn_prefab.Count;

            GameObject newObject = Instantiate(spawn_prefab[currentPrefabIndex], position, rotation);
            newObject.transform.localScale = scale; // Keep previous scale
            spawned_objects.Add(newObject);
        }
    }

}
