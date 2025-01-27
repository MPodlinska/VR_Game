using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ThrowCollision : MonoBehaviour
{
    private bool hasHitTarget = false;  
    private Vector3 originalPosition;  
    private bool isThrown = false;   
    private XRGrabInteractable grabInteractable;  
    private Rigidbody rb;

    void Start()
    {
        originalPosition = transform.position;  
        grabInteractable = GetComponent<XRGrabInteractable>();  
        rb = GetComponent<Rigidbody>();  

        
        grabInteractable.onSelectExited.AddListener(OnThrow);

        rb.isKinematic = true; 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Throw_Aim"))
        {
            hasHitTarget = true;  
            print($"{gameObject.name} hit the target!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Throw_Aim"))
        {
            hasHitTarget = false; 
            print($"{gameObject.name} exited the target zone.");
        }
    }

    private void OnThrow(XRBaseInteractor interactor)
    {
        isThrown = true;
        print($"{gameObject.name} was thrown!");

        rb.isKinematic = false;

        StartCoroutine(CheckReturn());
    }

    private IEnumerator CheckReturn()
    {
        yield return new WaitForSeconds(2f);

        while (rb.velocity.magnitude > 0.1f)
        {
            yield return null;  
        }

        if (!hasHitTarget)
        {
            ResetObject();
        }
    }

    private void ResetObject()
    {
        print($"{gameObject.name} is resetting...");

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = originalPosition;
        transform.rotation = Quaternion.identity;

        isThrown = false;
    }
}