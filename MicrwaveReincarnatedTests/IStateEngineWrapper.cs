namespace MicrwaveReincarnatedTests
{
    public interface IStateEngineWrapper
    {
        void OpenDoor(object sender, dynamic args);
        void ShowMessage(object sender, string message);
        void SetReady(object sender, dynamic args);
        void SetLightReady(object sender, dynamic args);
    }
}