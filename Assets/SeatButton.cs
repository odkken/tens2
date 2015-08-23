using Assets.Scripts.Player;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class SeatButton : MonoBehaviour
    {
        public Position Position;

        public void Click()
        {
            FindObjectOfType<SeatManager>().PlayerClickedSit(Position);
        }

        public void SetText(string text)
        {
            GetComponentInChildren<Text>().text = text;
        }
    }
}
