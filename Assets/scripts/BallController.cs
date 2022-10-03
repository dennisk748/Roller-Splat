using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;

    public float speed;

    private bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPos;
    private int minSwipeRecognition = 500;


    private Vector2 swipelastframepos;
    private Vector2 swipecurrentframepos;
    private Vector2 currentSwipe;

    private Color colourvalue;

    private void Start()
    {
        colourvalue = Random.ColorHSV(0.5f, 1f);
        GetComponent<MeshRenderer>().material.color = colourvalue;
    }

    private void LateUpdate()
    {
        if (isTravelling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i < hitColliders.Length)
        {
            Ground ground = hitColliders[i].transform.GetComponent<Ground>();
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(colourvalue);
            }
            i++;
        }

        if(Vector3.Distance(transform.position, nextCollisionPos) < 1)
        {
            isTravelling = false;
            travelDirection = Vector3.zero;
            nextCollisionPos = Vector3.zero;
        }

        if(isTravelling)
            return;

        if (Input.GetMouseButton(0))
        {
            swipecurrentframepos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipelastframepos != Vector2.zero)
            {
                currentSwipe = swipecurrentframepos - swipelastframepos;
                if(currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back );
                }
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }
            swipelastframepos = swipecurrentframepos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipelastframepos = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if(Physics.Raycast(transform.position,direction,out hit, 100f))
        {
            nextCollisionPos = hit.point;
        }

        isTravelling = true;
    }
}
