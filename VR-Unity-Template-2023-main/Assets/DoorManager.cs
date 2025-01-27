using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
   
    public GameObject movingDoor; 
    public GameObject notMovingDoor;


    // Start is called before the first frame update
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {

        if (ThrowCollision.total_points == 4)
        {
            // Wyłączamy drzwi nieruchome i włączamy ruchome
            notMovingDoor.SetActive(false);
            movingDoor.SetActive(true);
        }
    }
}
