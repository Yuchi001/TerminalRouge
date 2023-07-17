public interface IChangeCursor
{
    public ECursorType cursorType { get; protected set; }
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