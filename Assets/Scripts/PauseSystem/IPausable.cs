public interface IPausable
{
    public abstract bool Paused { get; set; }

    public abstract void Pause();
    public abstract void UnPause();
}