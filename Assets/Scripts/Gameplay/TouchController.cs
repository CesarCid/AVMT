using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    private Vector2 touchCurrentPos;
    private Vector2 touchOriginPos;

    private LayerMask interactableMask;

    private static TouchController tc;
    public static TouchController Instance => tc;

    public Action TouchedDown;
    public Action<GameObject> TouchedDownOnInteractable;
    public Action TouchedUp;
    public Action<GameObject> TouchedUpOnInteractable;

    void Awake()
    {
        if (tc == null)
            tc = this;

        LayerMask interactableMask = LayerMask.GetMask("TouchInteractable");
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //Debug.Log("[TouchController] Touch - Phase: "+ touch.phase+ " | Position: "+touch.position + " | FingerId: " + touch.fingerId);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                {
                    touchOriginPos = touch.position;
                    touchCurrentPos = touch.position;

                    if (GetCollidingInteractable(touch.position, out GameObject goBegan))
                        TouchedDownOnInteractable?.Invoke(goBegan);

                    TouchedDown?.Invoke();

                    return;
                }
                case TouchPhase.Moved:
                {
                    touchCurrentPos = touch.position;
                    return;
                }
                case TouchPhase.Ended:
                {
                    touchCurrentPos = touch.position;

                    if (GetCollidingInteractable(touch.position, out GameObject goEnd))
                        TouchedUpOnInteractable?.Invoke(goEnd);

                    TouchedUp?.Invoke();

                    return;
                }
            }            
        }
    }

    bool GetCollidingInteractable(Vector2 screenPosition, out GameObject interactable)
    {
        //Debug.Log("[TouchController] CheckCollision on screenPosition: "+ screenPosition);
        interactable = null;
        
        Ray ray = Camera.main.ScreenPointToRay(screenPosition); 
        RaycastHit hit;
        interactableMask = LayerMask.GetMask("TouchInteractable");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableMask))
        {
            //Debug.Log("[TouchController] CheckCollision hit: " + hit.transform.gameObject.name);
            interactable = hit.transform.gameObject;
            return true;  
        }

        return false;
    }

    #region MouseDebug

#if UNITY_EDITOR
    private void OnMouseDown()
    {
        DebugOnMouseDown();
    }
    public void DebugOnMouseDown()
    {
        Debug.Log("[TouchController] DebugOnMouseDown");

        if (GetCollidingInteractable(Input.mousePosition, out GameObject go))
            TouchedDownOnInteractable?.Invoke(go);

        TouchedDown?.Invoke();
    }
    private void OnMouseUp()
    {
        DebugOnMouseUp();
    }

    public void DebugOnMouseUp() 
    {
        Debug.Log("[TouchController] DebugOnMouseUp");

        if (GetCollidingInteractable(Input.mousePosition, out GameObject go))
            TouchedUpOnInteractable?.Invoke(go);

        TouchedUp?.Invoke();
    }

#endif

    #endregion
}
