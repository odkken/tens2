﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class BidGui : MonoBehaviour
    {
        public int CurrentBid
        {
            get { return _currentBid; }
            private set
            {
                _currentBid = value;
                transform.FindChild("CurrentBid").GetComponent<Text>().text = CurrentBid.ToString();
            }
        }

        private int minBid;
        private int _currentBid;

        void Start()
        {
            Hide();
        }

        public delegate void BidSubmitted(int bid);
        public static event BidSubmitted OnBidSubmitted;
        public void SetMinBid(int min)
        {
            minBid = min;
            CurrentBid = min;
            transform.FindChild("MinBid").GetComponent<Text>().text = "Min Bid: " + minBid;
        }
        public void UpBid()
        {
            CurrentBid += 5;
            if (CurrentBid > 100)
                CurrentBid = 100;
        }

        public void DownBid()
        {
            CurrentBid -= 5;
            if (CurrentBid < minBid)
                CurrentBid = minBid;
        }

        public void Hide()
        {
            GetComponent<RectTransform>().position = new Vector3(50000, 0, 0);
        }

        public void Show()
        {
            GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
        }

        public void SubmitBid()
        {
            if (OnBidSubmitted != null)
                OnBidSubmitted(CurrentBid);
        }

        public void Pass()
        {
            if (OnBidSubmitted != null)
                OnBidSubmitted(0);
        }
    }
}
