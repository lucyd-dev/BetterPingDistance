using UnityEngine;
using TMPro;

namespace BetterPingDistance;

public class PingDistance : MonoBehaviour
{
    public Vector3 position;
    public Color color = Color.white;
    private TextMeshProUGUI? TMP;

    void LateUpdate()
    {
        if (Camera.main == null) return;

        float distance = Mathf.Round(Vector3.Distance(position, Camera.main.transform.position));
        float angle = Vector3.Angle(Camera.main.transform.forward, position - Camera.main.transform.position);
        if (TMP == null) TMP = GetComponentInChildren<TextMeshProUGUI>();

        transform.position = Camera.main.WorldToScreenPoint(position);

        TMP.color = color;
        TMP.text = $"{distance}m";
        TMP.gameObject.SetActive(angle < 90f);
    }
}
