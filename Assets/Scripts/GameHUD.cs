using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [Header("Always visible (UI > Text - TextMeshPro)")]
    [SerializeField] private TMP_Text rescuedText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text promptText;

    [Header("End screen (disable this panel in the scene)")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitleText;
    [SerializeField] private TMP_Text endReasonText;

    private void Start()
    {
        if (endPanel != null) endPanel.SetActive(false);
        GameManager.Instance.StateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
    }

    private void Update()
    {
        var gm = GameManager.Instance;
        var player = FirstPersonController.Instance;
        if (gm == null) return;

        if (rescuedText != null) rescuedText.text = $"Rescued: {gm.RescuedCount} / {gm.CreaturesToWin}";
        if (scoreText != null)   scoreText.text   = $"Score: {gm.Score}";
        if (livesText != null)   livesText.text   = $"Lives: {gm.Lives}";

        if (promptText != null && player != null)
        {
            if (gm.State != GameState.Playing)           promptText.text = "";
            else if (player.CarriedCreature != null)     promptText.text = "Bring it to the safe zone!";
            else if (player.CreatureInReach != null)     promptText.text = "Press E to pick up";
            else                                         promptText.text = "Find the creature!";
        }
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Playing || endPanel == null) return;

        endPanel.SetActive(true);
        if (endTitleText != null)
            endTitleText.text = state == GameState.Won ? "YOU WIN!" : "GAME OVER";
        if (endReasonText != null)
            endReasonText.text = $"{GameManager.Instance.EndMessage}\nFinal score: {GameManager.Instance.Score}\nPress R to play again";
    }

    public void OnRestartButton() => GameManager.Instance.Restart();
}
