// Singleton class to manage game states
public class GameStateManager {
    private static GameStateManager _instance;
    public static GameStateManager Instance {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStateManager();
            }
            return _instance;
        }
    }

    public GameState CurrentState { get; private set; }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
    }
}