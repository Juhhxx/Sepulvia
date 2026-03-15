public interface IPausable
{
    public bool Paused { get; }

    public abstract void TogglePause(bool onOff);
}