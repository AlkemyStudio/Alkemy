
namespace Game
{
    public struct GameEndedEvent
    {
        public string[] WinnerNames;
        public string[] LastPlayerNamesDeadAtTheSameTime;
        public float GameEndTime;
    }
}