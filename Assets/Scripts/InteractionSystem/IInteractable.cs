public interface IInteractable
{
    public bool CanInteract { get; }
    public abstract void ToggleSelected(bool onOff);
    public abstract void Interact();
}
