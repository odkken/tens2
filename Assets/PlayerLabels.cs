using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class PlayerLabels : MonoBehaviour
    {
        void Start()
        {
            Hide();
        }

        public void Show()
        {
            transform.position = Vector3.zero;

            var players = FindObjectsOfType<FourPlayer>();
            //var localPlayer = players.Single(a => a.isLocalPlayer);

            var children = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i));
            }

            transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -Camera.main.transform.eulerAngles.z);
            foreach (var child in children)
            {
                child.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -Camera.main.transform.eulerAngles.z);
                child.GetComponent<Text>().text = players.Single(a => a.Position.ToString() == child.name).Name;
            }
        }

        public void Hide()
        {
            transform.position = new Vector3(50000, 0, 0);
        }
    }
}
