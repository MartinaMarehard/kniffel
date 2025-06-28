namespace Kniffel.models;

public class GameState
{
    public int RollCount { get; set; }
    public HashSet<string> UsedCategory { get; set; }
    public List<Die> CurrentDice { get; set; } = new();
    
    public event Action? OnResetRequested;

    public void RequestReset()
    {
        RollCount = 0;
        OnResetRequested?.Invoke();
    }
}