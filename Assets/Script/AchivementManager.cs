using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    private int p1Dashes = 0;
    private int p2Dashes = 0;
    private int totalInteractions = 0;

    private bool syncDashUnlocked = false;
    private bool heavyLifterUnlocked = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LogDash(string playerName)
    {
        if (playerName == "Player 1") p1Dashes++;
        if (playerName == "Player 2") p2Dashes++;

        // Achievement: Both players have dashed at least 5 times
        if (!syncDashUnlocked && p1Dashes >= 5 && p2Dashes >= 5)
        {
            syncDashUnlocked = true;
            Debug.Log("🏆 ACHIEVEMENT UNLOCKED: [Synchronized Synergy] - Both players dashed 5 times!");
        }
    }

    public void LogInteraction()
    {
        totalInteractions++;
        if (!heavyLifterUnlocked && totalInteractions >= 10)
        {
            heavyLifterUnlocked = true;
            Debug.Log("🏆 ACHIEVEMENT UNLOCKED: [Circuit Overload] - Triggered 10 world interactions!");
        }
    }
}