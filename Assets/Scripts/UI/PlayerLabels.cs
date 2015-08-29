using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerLabels : MonoBehaviour
    {
        void Start()
        {
            Hide();
        }

        public void HighlightPlayerName(string player)
        {
            var texts = GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.color = Color.white;
                if (text.text == player)
                    text.color = Color.magenta;
            }
        }
        public void Show()
        {
            transform.position = Vector3.zero;

            var players = FindObjectsOfType<FourPlayer>();
            var localPlayer = players.Single(a => a.isLocalPlayer);

            var children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }

            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            foreach (var child in children)
            {
                child.rotation = Quaternion.LookRotation(Vector3.forward, Camera.main.transform.up);
                child.GetComponent<Text>().text = players.Single(a => a.Position.ToString() == child.name).Name;
            }
            transform.FindChild(localPlayer.Position.ToString()).GetComponent<Text>().text = "";
        }

        public void Hide()
        {
            transform.position = new Vector3(50000, 0, 0);
        }
    }
}
