using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets
{
    public class SeatManager : NetworkBehaviour
    {
        public bool IsNorthOpen;
        public bool IsSouthOpen;
        public bool IsEastOpen;
        public bool IsWestOpen;

        void Start()
        {
            IsNorthOpen = true;
            IsSouthOpen = true;
            IsWestOpen = true;
            IsEastOpen = true;
        }


        public delegate void SitClicked(Position pos);
        public static event SitClicked OnClickedSit;

        public void PlayerClickedSit(Position position)
        {
            if (OnClickedSit == null)
                return;
            switch (position)
            {
                case Position.North:
                    if (IsNorthOpen)
                    {
                        OnClickedSit(position);
                    }
                    break;
                case Position.South:
                    if (IsSouthOpen)
                    {
                        OnClickedSit(position);
                    }
                    break;
                case Position.East:
                    if (IsEastOpen)
                    {
                        OnClickedSit(position);
                    }
                    break;
                case Position.West:
                    if (IsWestOpen)
                    {
                        OnClickedSit(position);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void Update()
        {
            IsNorthOpen = true;
            IsSouthOpen = true;
            IsEastOpen = true;
            IsWestOpen = true;


            var allPlayers = FindObjectsOfType<FourPlayer>();
            foreach (var fourPlayer in allPlayers)
            {
                if (!fourPlayer.IsSeated) continue;

                switch (fourPlayer.Position)
                {
                    case Position.North:
                        IsNorthOpen = false;
                        break;
                    case Position.South:
                        IsSouthOpen = false;
                        break;
                    case Position.East:
                        IsEastOpen = false;
                        break;
                    case Position.West:
                        IsWestOpen = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (isClient)
            {
                var allSeatButtons = FindObjectsOfType<SeatButton>();
                foreach (var seatButton in allSeatButtons)
                {
                    seatButton.SetText("Sit");
                }

                foreach (var player in allPlayers)
                {
                    if (player.IsSeated)
                    {
                        allSeatButtons.Single(a => a.Position == player.Position).SetText(player.Name);
                    }
                }
            }
            if (isServer)
            {
                if (!IsEastOpen && !IsWestOpen && !IsNorthOpen && !IsSouthOpen)
                {
                    if (OnAllPlayersSeated != null)
                        OnAllPlayersSeated();
                    RpcHide();
                    gameObject.SetActive(false);
                }
            }

        }

        [ClientRpc]
        void RpcHide()
        {
            transform.position = new Vector3(50000, 0);
        }

        public delegate void AllPlayersSeated();
        public static event AllPlayersSeated OnAllPlayersSeated;
    }
}
