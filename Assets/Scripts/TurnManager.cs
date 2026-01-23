using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public Player player;
    public Enemy enemy;

    public bool isPlayerTurn = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        player.StartTurn();
    }

    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        player.EndTurn();

        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        if (enemy != null)
            enemy.StartEnemyTurn();
    }
}
