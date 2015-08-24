using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

namespace Assets
{
    public class Matchmaker : MonoBehaviour
    {
        NetworkMatch _networkMatch;
        private bool _matchCreated;

        void Awake()
        {
            _networkMatch = gameObject.AddComponent<NetworkMatch>();
        }

        public void StartServer(string playerName, string serverName, string password = "", bool advertise = true)
        {
            var create = new CreateMatchRequest
            {
                name = serverName,
                size = 4,
                advertise = advertise,
                password = password
            };
            _networkMatch.CreateMatch(create, response =>
            {
                if (response.success)
                {
                    Debug.Log("Create match succeeded");
                    _matchCreated = true;
                    Utility.SetAccessTokenForNetwork(response.networkId, new NetworkAccessToken(response.accessTokenString));
                    NetworkServer.Listen(new MatchInfo(response), 9000);
                }
                else
                {
                    Debug.LogError("Create match failed");
                }
            });
        }

        public void JoinServer(string playerName, string serverName)
        {
            _networkMatch.ListMatches(0, 20, "", response =>
            {
                if (response.success && response.matches != null)
                {
                    var specifiedServer = response.matches.FirstOrDefault(a => a.name.ToLower() == serverName.ToLower());
                    if (specifiedServer == null)
                        Debug.LogWarningFormat("No server named {0} could be found!", serverName);
                    else
                        _networkMatch.JoinMatch(specifiedServer.networkId, "", matchResponse =>
                        {
                            if (matchResponse.success)
                            {
                                Debug.Log("Join match succeeded");
                                if (_matchCreated)
                                {
                                    Debug.LogWarning("Match already set up, aborting...");
                                    return;
                                }
                                Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
                                NetworkClient myClient = new NetworkClient();
                                myClient.RegisterHandler(MsgType.Connect, msg => Debug.Log("Connected!"));
                                myClient.Connect(new MatchInfo(matchResponse));
                            }
                            else
                            {
                                Debug.LogError("Join match failed");
                            }
                        });
                }
            });
        }
    }
}
