public interface ISavable
{
    public SaveIDSO IdData { get; }
    public string GetSaveData();
    public void RestoreData(string data);
}
