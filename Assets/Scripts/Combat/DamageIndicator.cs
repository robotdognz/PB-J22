using UnityEngine.UI;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private void Update()
    {
        transform.position += Vector3.one * Time.deltaTime / 2;
        GetComponent<Text>().color = new Color(GetComponent<Text>().color.r, GetComponent<Text>().color.g, GetComponent<Text>().color.b, GetComponent<Text>().color.a - Time.deltaTime * 2);
    }
}
