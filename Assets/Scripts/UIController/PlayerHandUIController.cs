using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> handGameUIs;
    //private List<Image> _handImageList;
    private EntityManager _entityManager;
    private Entity _playerEntity;
    private PlayerHand _playerHand;
    
    public void UpdateHandUI()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerEntity = _entityManager.CreateEntityQuery(typeof(Player)).GetSingletonEntity();

        Debug.Log("UpdateHandUI");
        _playerHand = _entityManager.GetComponentData<PlayerHand>(_playerEntity);
        for (int i = 0; i < _playerHand.Cards.Length; i++)
        {
            var card = _playerHand.Cards[i];
            string suit;
            switch (card.Suit)
            {
                case Suit.Spades:
                    suit = "spades";
                    break;
                case Suit.Hearts:
                    suit = "hearts";
                    break;
                case Suit.Diamonds:
                    suit = "diamonds";
                    break;
                case Suit.Clubs:
                    suit = "clubs";
                    break;
                default:
                    // later change this to empty card.
                    suit = "spades";
                    break;
            }

            string texturePath = CardData.GetCardTextureAddress(card.Number, suit);
            handGameUIs[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(texturePath);
        }

    }
    // Start is called before the first frame update
    // private IEnumerator Start()
    // {
    //     _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    //     
    //     
    //     // wait until ecs world loaded.
    //     // get singleton이 계속 말썽...
    //     yield return new WaitForSeconds(0.5f);
    //     _playerEntity = _entityManager.CreateEntityQuery(typeof(Player)).GetSingletonEntity();
    //     
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
