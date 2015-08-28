using System.Linq;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets
{
    public class Matchmaker : MonoBehaviour
    {
        public NetworkManager manager;

        void Awake()
        {
            manager.StartMatchMaker();
        }
        void Hide()
        {
            transform.position = new Vector3(50000, 0, 0);
        }

        void Show()
        {
            transform.position = new Vector3(0, 0, 0);
        }

        public void StartServer(string playerName, string serverName, string password = "", bool advertise = true)
        {
            manager.matchMaker.CreateMatch(serverName, 4, true, "", response =>
            {
                PLAYER_NAME = playerName;
                manager.OnMatchCreate(response);
            });
            Hide();
        }

        public static string PLAYER_NAME;

        public void JoinServer(string playerName, string serverName)
        {
            manager.matchMaker.ListMatches(0, 10, "",
                response =>
                    manager.matchMaker.JoinMatch(response.matches.First(a => a.name == serverName).networkId, "",
                        matchResponse =>
                        {
                            manager.OnMatchJoined(matchResponse);
                            PLAYER_NAME = playerName;
                            Hide();
                        }));
        }
    }
}
