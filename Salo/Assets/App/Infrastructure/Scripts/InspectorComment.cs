using UnityEngine;

public class InspectorComment : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string comment;
    public string Comment => comment;
}
