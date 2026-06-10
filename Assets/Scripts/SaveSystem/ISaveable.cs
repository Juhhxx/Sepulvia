public interface ISaveable
{
    public object GetSaveData();

    public void LoadSaveData(string json);
}