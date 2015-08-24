using Assets.Scripts.Card;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Initialization
{
    public static class ResourceFolderCardTextureProvider
    {
        public static Sprite GetCardTexture(Rank rank, Suit suit)
        {
            var rankChar = "";
            if ((int)rank < 9)
                rankChar = ((int)rank + 2).ToString();
            else
            {
                switch ((int)rank)
                {
                    case 9:
                        rankChar = "J";
                        break;
                    case 10:
                        rankChar = "Q";
                        break;
                    case 11:
                        rankChar = "K";
                        break;
                    case 12:
                        rankChar = "A";
                        break;
                }
            }

            var cardString = "card" + suit + rankChar;
            var frontSprite = Resources.Load<Sprite>("CardTextures/" + cardString);
            if (frontSprite == null)
            {
                DebugConsole.Log("Couldn't load " + cardString);
            }
            return frontSprite;
        }
    }
}