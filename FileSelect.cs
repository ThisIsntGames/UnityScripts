using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FileSelect : MonoBehaviour
{
    //MAKE SURE TO SET LAYERMASK TO APPROPRIATE LAYER. ICO BM USE UI.
    
    private SpriteRenderer spriteRenderer;
    [SerializeField] Sprite notSelected, Selected;

    public bool isSelected;
    private Vector2 xyInput;
    private float tolerance = 0.5f;
    
    [SerializeField] private GameObject nextSelection;
    public LayerMask layerMask;
    
    private float timer;
    private float waitTime = 0.25f;

    private void Start()
    {
        //All we need to do on start is grab the spriterenderer component.
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Every frame let's check these bools.
        highlightSelection();
        deemphasizeSelection();
        
        //Cast rays on controller input.
        castRay();
    }
    
    private void highlightSelection()
    {
        //If the object is not selected, bail.
        if (!isSelected) return;
        
        //If the object is already set as selected, bail.
        if (spriteRenderer.sprite == Selected) return;
        
        //Set the sprite correctly.
        spriteRenderer.sprite = Selected;
    }
    
    private void deemphasizeSelection()
    {
        //If the object is selected, bail.
        if (isSelected) return;
        
        //If the object is already set as notSelected, bail.
        if (spriteRenderer.sprite == notSelected) return;
        
        //Set the sprite correctly.
        spriteRenderer.sprite = notSelected;
    }

    private void castRay()
    {
        //If it's not selected, bail.
        if (!isSelected) return;
        
        //Give a little bit of a buffer so you can choose through a vertical or horizontal list.
        timer += Time.deltaTime;
        
        //If the timer hasn't gone off... bail.
        if (timer < waitTime) return;
        
        //If the user input is outside of the tolerance. This will help with slightly bad joysticks.
        if (xyInput.x > tolerance || xyInput.x < -tolerance || xyInput.y > tolerance || xyInput.y < -tolerance)
        {
            //Send out a ray, give it the variable selectInfo.
            RaycastHit2D selectInfo = Physics2D.Raycast(transform.position, xyInput, 1000, layerMask);
            
            //Debugging purposes. #Delete on build.
            Debug.DrawRay(transform.position, xyInput, Color.green);

            //If there is no ray sent, bail.
            if (!selectInfo) return;
            
            //Give the variable "nextSelection" to whatever the ray hits.
            nextSelection = selectInfo.transform.gameObject;
            
            //If there is an object in nextSelection. Reset Timer, Disable Selection, Turn on new object, clear old selection.
            if (nextSelection != null)
            {
                timer = 0;
                isSelected = false;
                nextSelection.GetComponent<FileSelect>().isSelected = true;
                nextSelection = null;
            }
        }
    }

    //Get raw data from Joystick.
    private void OnMove(InputValue value)
    {
        xyInput = value.Get<Vector2>();
    }
}
