using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;


[System.Serializable]
public class CardData
{
    public int Value; // 1~13 (A~K)
    public string Suit; // "spades", "hearts", "diamonds", "clubs"
    public bool IsJoker;

    public CardData(int value, string suit, bool isJoker = false)
    {
        Value = value;
        Suit = suit;
        IsJoker = isJoker;
    }

    public static string GetCardTextureAddress(int number, string suit, bool isJoker = false)
    {
        if (isJoker)
        {
            return "Cards (large)/card_joker_red";
        }
        else
        {
            string cardValueString = "";
            switch (number)
            {
                case 1:
                    cardValueString = "A";
                    break;
                case 11:
                    cardValueString = "J";
                    break;
                case 12:
                    cardValueString = "Q";
                    break;
                case 13:
                    cardValueString = "K";
                    break;
                case 10:
                    cardValueString = number.ToString();
                    break;
                default:
                    cardValueString = "0" + number.ToString();
                    break;
            }

            string texturePath = $"Cards (large)/card_{suit}_{cardValueString}";
            return texturePath;
        }
    }
}

public class CardItem : ItemBase
{
    public CardData cardData;
    private SpriteRenderer _spriteRenderer;
    private GameObject _handUIGameObject;
    private PlayerHandUIController _handUIComponent;

    protected override IEnumerator Start()
    {
        // what if cannot find?
        _handUIGameObject = GameObject.Find("HandUI");
        
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _handUIComponent = _handUIGameObject.GetComponent<PlayerHandUIController>();
        UpdateVisuals(CardData.GetCardTextureAddress(cardData.Value, cardData.Suit, cardData.IsJoker));
        return base.Start();
    }

    public void Initialize(CardData data)
    {
        cardData = data;
        UpdateVisuals(CardData.GetCardTextureAddress(cardData.Value, cardData.Suit, cardData.IsJoker));
    }

    private void UpdateVisuals(string texturePath = null)
    {
        _spriteRenderer.sprite = Resources.Load<Sprite>(texturePath);
    }

    protected override void OnItemCollected()
    {
        //Debug.Log("Card Collected");
        var playerHand = WorldEntityManager.GetComponentData<PlayerHand>(PlayerEntity);
        if (playerHand.Cards.Length >= 5)
        {
            Debug.Log("Full Hand");
        }
        else
        {
            var newPokerCard = new PokerCard(cardData.Value, cardData.Suit);
            playerHand.Cards.Add(newPokerCard);

            List<PokerCard> pokerCardsList = new List<PokerCard>();
            for (int i = 0; i < playerHand.Cards.Length; i++)
            {
                pokerCardsList.Add(playerHand.Cards[i]);
            }

            var debughandtype = PokerHandEvaluator.EvaluateHand(pokerCardsList);
            Debug.Log(debughandtype);
            playerHand.HandType = PokerHandEvaluator.EvaluateHand(pokerCardsList);
            // 플레이어 핸드의 카드들중 어떤 카드가 족보에 들어가는지 아닌지 판별도 해주고 넣어주면 좋겠다.
            WorldEntityManager.SetComponentData(PlayerEntity, playerHand);

            // make event to update hand UI
            // now just direct call
            _handUIComponent.UpdateHandUI();


        }
    }
}
