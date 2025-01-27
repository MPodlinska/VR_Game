using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowColision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Throw_Aim")
        {
            print("ENTER");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Throw_Aim")
        {
            print("COLISSION");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Throw_Aim")
        {
            print("EXIT");
        }
    }
}
