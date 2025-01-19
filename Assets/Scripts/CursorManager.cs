using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D openHandCursorTexture;
    public Texture2D closedHandCursorTexture;
    public Vector2 openHandCursorCenter;
    public Vector2 closedHandCursorCenter;


    void Start()
    {
        // Calculate the center of the hand textures
        openHandCursorCenter  = new Vector2(openHandCursorTexture.width / 2, openHandCursorTexture.height / 2);
        closedHandCursorCenter = new Vector2(closedHandCursorTexture.width / 2, closedHandCursorTexture.height / 2);

        // Set default cursor
        Cursor.SetCursor(openHandCursorTexture, openHandCursorCenter, CursorMode.Auto);
    }

    private void OnEnable()
    {
        DragAndDrop.OnBlockHold += HandleBlockHold;
        DragAndDrop.OnBlockRelease += HandleBlockRelease;
    }

    private void OnDisable()
    {
        DragAndDrop.OnBlockHold -= HandleBlockHold;
        DragAndDrop.OnBlockRelease -= HandleBlockRelease;
    }


    private void HandleBlockHold()
    {
        Cursor.SetCursor(closedHandCursorTexture, closedHandCursorCenter, CursorMode.Auto);
    }

    private void HandleBlockRelease()
    {
        Cursor.SetCursor(openHandCursorTexture, openHandCursorCenter, CursorMode.Auto);
    }


    void OnApplicationQuit()
    {
        // Reset the cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
