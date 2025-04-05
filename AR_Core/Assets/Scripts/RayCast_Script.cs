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
    public List<GameObject> spawn_prefab; //Take a list of prefabs that can be spawned in the scene. 

    List<GameObject> spawned_objects = new List<GameObject>(); //Also, keep a list of objects that are placed in scene

    ARRaycastManager arrayman; //The AR ray cast manager to handle the placing of objects

    public Button changeShapeButton; //Get the UI button for changing object in scene

    public Button placeObjectButton;     //Get the UI button for placiing object in scene
    private bool objectPlaced = false;   //Default false for object placed

    private int currentPrefabIndex = 0; //Keep the index of total prefabs 

    List<ARRaycastHit> hits = new List<ARRaycastHit>(); // keep the location of different ray casts to place objects


    //pinch to zoom in and zoom out
    Vector2 First_Touch;
    Vector2 Second_Touch;
    float Distance_Current;
    float Distance_Previous;
    bool First_Pinch = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Setup the ray cast manager, and UI buttons
        arrayman = GetComponent<ARRaycastManager>();
        changeShapeButton.onClick.AddListener(ChangeShape);

        placeObjectButton.onClick.AddListener(PlaceObject);
    }

    void Update()
    {
        //Handling the zoom in and out of prefabs
        //Check for more than 2 touches and if any object was placed before hand
        if (Touchscreen.current.touches.Count > 1 && spawned_objects.Count > 0 && objectPlaced)
        {
            var touches = Touchscreen.current.touches; //Store the location of both touch

            First_Touch = touches[0].position.ReadValue();
            Second_Touch = touches[1].position.ReadValue();

            Distance_Current = Vector2.Distance(First_Touch, Second_Touch);//Get the distance between two touches

            if (First_Pinch)
            {
                //Setup the distance if it is first zoom in , zoom out
                Distance_Previous = Distance_Current;
                First_Pinch = false;
            }

            if (Distance_Current != Distance_Previous)
            {
                //IF the distance changes change the scale of the object accordingly
                GameObject currentObject = spawned_objects[spawned_objects.Count - 1];
                Vector3 Scale_Value = currentObject.transform.localScale * (Distance_Current / Distance_Previous);
                currentObject.transform.localScale = Scale_Value;
                Distance_Previous = Distance_Current;
            }
        }
        else
        {
            First_Pinch = true;
        }
    }

    //Changing the shape that has been placed
    void ChangeShape()
    {
        //if object placed then...
        if (spawned_objects.Count > 0)
        {
            //Get the old object;s position, scale, rotation
            GameObject oldObject = spawned_objects[spawned_objects.Count - 1];
            Vector3 position = oldObject.transform.position;
            Quaternion rotation = oldObject.transform.rotation;
            Vector3 scale = oldObject.transform.localScale;

            //destroy old object and tremove if the the list of objects placed
            Destroy(oldObject);
            spawned_objects.RemoveAt(spawned_objects.Count - 1);

            // Cycle through the list
            currentPrefabIndex = (currentPrefabIndex + 1) % spawn_prefab.Count;//get the next shape from the list

            GameObject newObject = Instantiate(spawn_prefab[currentPrefabIndex], position, rotation);
            newObject.transform.localScale = scale; // Keep previous scale
            spawned_objects.Add(newObject);
        }
    }

    void PlaceObject()
    {
        //you could either delete all the pre existing shapes when u place a new object or simply keep them
        /*if (spawned_objects.Count > 0)
        {
            Destroy(spawned_objects[spawned_objects.Count - 1]);
            spawned_objects.RemoveAt(spawned_objects.Count - 1);
        }*/

        //Get the touch location from user's input
        Vector2 touchPosition;
        if (Touchscreen.current != null)
        {
            touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else
        {
            touchPosition = new Vector2(Screen.width / 2, Screen.height / 2); // center screen
        }
        //Place new object 
        if (arrayman.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            GameObject prefabToSpawn = spawn_prefab[currentPrefabIndex];
            GameObject newObject = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
            spawned_objects.Add(newObject);
            objectPlaced = true;
        }
        else
        {
            Debug.Log("No valid surface at screen center.");
        }
    }
}