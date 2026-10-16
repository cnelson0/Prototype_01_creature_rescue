using System.Collections;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public static CreatureSpawner Instance { get; private set; }

    [SerializeField] private Creature creaturePrefab;
    [Tooltip("Empty objects placed in different areas of the level")]
    [SerializeField] private Transform[] creatureLocations;
    [SerializeField] private float delayBetweenCreatures = 2f;
    [SerializeField] private bool avoidSameLocationTwice = true;

    public Creature Current { get; private set; }

    private int lastIndex = -1;

    private void Awake() { Instance = this; }
    private void OnEnable() { Creature.Rescued += OnCreatureRescued; }
    private void OnDisable() { Creature.Rescued -= OnCreatureRescued; }

    private void Start()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
        StartCoroutine(SpawnAfterDelay());
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
    }

    private void OnCreatureRescued(Creature creature)
    {
        if (creature != Current) return;
        Current = null;

        if (GameManager.Instance.State == GameState.Playing)
            StartCoroutine(SpawnAfterDelay());
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.Playing) return;
        StopAllCoroutines();
        if (Current != null) Destroy(Current.gameObject);
        Current = null;
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(delayBetweenCreatures);

        if (GameManager.Instance.State != GameState.Playing) yield break;
        if (creatureLocations == null || creatureLocations.Length == 0 || creaturePrefab == null)
        {
            Debug.LogWarning("CreatureSpawner: assign the creature prefab and locations.");
            yield break;
        }

        int index = PickIndex();
        Transform spot = creatureLocations[index];
        Current = Instantiate(creaturePrefab, spot.position, spot.rotation);
        lastIndex = index;
    }

    private int PickIndex()
    {
        if (creatureLocations.Length == 1 || !avoidSameLocationTwice)
            return Random.Range(0, creatureLocations.Length);

        int index;
        do { index = Random.Range(0, creatureLocations.Length); }
        while (index == lastIndex);
        return index;
    }
}
