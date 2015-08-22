using UnityEngine;

namespace Assets.Scripts.Card
{
    public class Clickable : MonoBehaviour
    {
        void OnMouseDown()
        {
            transform.RotateAround(transform.position, transform.up, 180);
        }
    }
}
