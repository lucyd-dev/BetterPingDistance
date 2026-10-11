using UnityEngine;
using TMPro;

namespace BetterPingDistance;

public class PingDistance : MonoBehaviour
{
    public Vector3 position;
    public Color color = Color.white;
    private TextMeshProUGUI? TMP;

    void Start()
    {
        TMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    void LateUpdate()
    {
        if (Camera.main == null || TMP == null) return;

        float units = Vector3.Distance(position, Camera.main.transform.position);
        float distance = units * CharacterStats.unitsToMeters;
        float scale = Mathf.Clamp(2 / (units / 10f), 0.5f, 1.5f);
        float angle = Vector3.Angle(Camera.main.transform.forward, position - Camera.main.transform.position);

        transform.position = Camera.main.WorldToScreenPoint(position);
        transform.localScale = new Vector3(scale, scale, scale);

        TMP.color = color;
        TMP.text = $"{Mathf.Round(distance)}m";
        TMP.gameObject.SetActive(angle < 90f);
    }
}
