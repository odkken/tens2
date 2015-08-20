using Assets.Scripts.Card;
using UnityEngine;

namespace Assets.Scripts.Initialization
{
    public interface ICardTextureProvider
    {
        Sprite GetCardTexture(Rank rank, Suit suit);
    }
}