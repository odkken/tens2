using UnityEngine;

namespace Assets.Scripts.Card
{
    public class Clickable : MonoBehaviour
    {
        void OnMouseDown()
        {
            transform.rotation = Quaternion.LookRotation(-transform.forward);
        }
    }
}
