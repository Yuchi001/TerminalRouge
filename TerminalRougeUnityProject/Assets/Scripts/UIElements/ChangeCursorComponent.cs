using UIElements;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(Image))]
public class ChangeCursorComponent : MonoBehaviour
{
    [SerializeField] private ECursorType cursorType;

    [SerializeField] private bool manualSize = false;

    private RetroCursor mainCursor => RetroCursor.Instance;

    private void Awake()
    {
        var collider2D = GetComponent<BoxCollider2D>();
        var rb2d = GetComponent<Rigidbody2D>();
        var image = GetComponent<Image>();

        collider2D.isTrigger = true;
        rb2d.gravityScale = 0;

        if (manualSize)
            return;

        var spriteSize = image.rectTransform.sizeDelta;
        collider2D.size = spriteSize;
    }

    private void OnMouseEnter()
    {
        mainCursor.SetCursorSprite(cursorType);
    }

    private void OnMouseExit()
    {
        mainCursor.SetCursorDefault();
    }
}

public enum ECursorType
{
    Default,
    ResizeHorizontal,
    ResizeVertical,
    Resize,
    Select,
    Write,
    Loading
}