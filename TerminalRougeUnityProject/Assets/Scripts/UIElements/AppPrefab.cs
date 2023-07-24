namespace UIElements
{
    public abstract class AppPrefab : FocusableUIElement
    {
        protected SOApp app;

        public virtual void Setup(SOApp app)
        {
            this.app = app;
        }
    }
}