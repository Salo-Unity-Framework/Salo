using UnityEngine;

namespace Salo.Infrastructure
{
    public class InspectorComment : MonoBehaviour
    {
        [TextArea(3, 10)]
        [SerializeField] private string comment;
        public string Comment => comment;
    }
}
