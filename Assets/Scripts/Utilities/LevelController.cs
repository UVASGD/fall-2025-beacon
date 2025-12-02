using TMPro;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public TMP_Text level_text;
    [SerializeField]
    private int level = 1;

    public delegate void Simple();
    public event Simple onLevelUp;

    private void Awake()
    {
        level_text.transform.eulerAngles = Vector3.right * 90f;
        UpdateDisplay();
    }

    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
        WaveManager.Singleton.onWaveStart += OnWaveStart;
        level_text.gameObject.SetActive(false);
    }

    void OnWaveEnd()
    {
        level_text.gameObject.SetActive(true);
    }

    void OnWaveStart()
    {
        //level_text.gameObject.SetActive(false);
    }

    void UpdateDisplay()
    {
        level_text.text = $"Level {level}";
    }

    public bool SameBuilding(Building building)
    {
        return transform.name.Contains(building.buildingPrefab.name);  
    }

    public int GetLevel()
    {
        return level;
    }

    public void LevelUp()
    {
        level++;
        if (onLevelUp != null)
        {
            onLevelUp();
        }
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        WaveManager.Singleton.onWaveFinished -= OnWaveEnd;
        WaveManager.Singleton.onWaveStart -= OnWaveStart;
    }
}
