using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

    /*
    [System.Serializable]
    public struct PlayerCard
    {
        public Cards cardType;
        public CardBehaviour behaviour;
    }

    public List<PlayerCard> playerCards = new List<PlayerCard>();
    */

    public Dictionary<Cards, CardBehaviour> cardDico = new Dictionary<Cards, CardBehaviour>();
    public List<CardBehaviour> cardOrder = new List<CardBehaviour>();

    [Range(1, 2)]
    public int player;
    public string attack;
    public string shield;
    public string backstep;
    bool canChangeCards = true;
    //public float horCooldown;

    x360_Gamepad gamepad;

    [Header("References")]
    public GameManager gameMan;
    GamepadManager gamepadMan;
    CardBehaviour selectedCard;
  //  CardBehaviour hoveredCard;

    void Start()
    {
        //hoveredCard = cardDico[0];
        //       hoveredCard.Hovered();
/*
        Dictionary<Cards, CardBehaviour>.ValueCollection valueColl = cardDico.Values;
        foreach (CardBehaviour card in valueColl)
        {
            cardOrder.Add(card);
        }
*/
        gamepadMan = GamepadManager.Instance;
        gamepad = gamepadMan.GetGamepad(player);
    }

    void Update()
    {
        if (canChangeCards)
        {
            SelectCard();

            if (selectedCard != null)
            {
                MoveCard();
            }
        }
    }

    void SelectCard()
    {
        if (gamepad.GetButtonDown(attack))
        {
            if (selectedCard != null)
            {
                selectedCard.Deselect();
            }

            if (selectedCard != null && selectedCard.cardNature == Cards.Attack)
            {
                selectedCard = null;
            }

            else
            {
                selectedCard = cardDico[Cards.Attack];
                selectedCard.Select();
            }
        }

        if (gamepad.GetButtonDown(shield))
        {
            if (selectedCard != null)
            {
                selectedCard.Deselect();
            }

            if (selectedCard != null && selectedCard.cardNature == Cards.Shield)
            {
                selectedCard = null;
            }

            else
            {
                selectedCard = cardDico[Cards.Shield];
                selectedCard.Select();
            }
        }

        if (gamepad.GetButtonDown(backstep))
        {
            if (selectedCard != null)
            {
                selectedCard.Deselect();
            }

            if (selectedCard != null && selectedCard.cardNature == Cards.Backstep)
            {
                selectedCard = null;
            }

            else
            {
                selectedCard = cardDico[Cards.Backstep];
                selectedCard.Select();
            }
        }
    }

    /* void HoverOverCards()
    {
        if (gamepad.GetStick_L().X == -1)
        {
            hoveredCard.NotHovered();

            int cardIndex = cardList.IndexOf(hoveredCard) - 1;

            if (cardIndex < 0)
            {
                hoveredCard = cardList[cardList.Count - 1];
            }

            else
                hoveredCard = cardList[cardIndex];

            hoveredCard.Hovered();

            canChangeCards = false;
            StartCoroutine(HorizontalCooldown()); 
        }

        if (gamepad.GetStick_L().X == 1)
        {
            hoveredCard.NotHovered();

            int cardIndex = cardList.IndexOf(hoveredCard) + 1;

            if (cardIndex == cardList.Count)
            {
                hoveredCard = cardList[0];
            }

            else
                hoveredCard = cardList[cardIndex];

            hoveredCard.Hovered();

            canChangeCards = false;
            StartCoroutine(HorizontalCooldown());
        }
    }
    */

    void MoveCard()
    {
        if (gamepad.GetStick_L().X == 1)
        {
            //Get card indexes
            int cardIndex = cardOrder.IndexOf(selectedCard);
            int otherCardIndex = cardIndex + 1;

            if (otherCardIndex == cardOrder.Count)
            {
                otherCardIndex = 0;
            }

            //Get other card
            CardBehaviour otherCard = cardOrder[otherCardIndex];
            CardBehaviour middleCard = null;
            if (otherCardIndex == 0)
            {
                middleCard = cardOrder[1];
            }

            //Swap card positions
            selectedCard.Move(new Vector3 (otherCard.transform.position.x, selectedCard.offsetY, otherCard.transform.position.z));

            if (middleCard != null)
            {
                otherCard.Move(new Vector3(middleCard.transform.position.x, middleCard.originY, middleCard.transform.position.z));
                middleCard.Move(new Vector3(selectedCard.transform.position.x, selectedCard.originY, selectedCard.transform.position.z));

                //Update card order
                cardOrder[1] = otherCard;
                cardOrder[otherCardIndex] = selectedCard;
                cardOrder[cardIndex] = middleCard;
            }

            else
            {
                otherCard.Move(new Vector3(selectedCard.transform.position.x, selectedCard.originY, selectedCard.transform.position.z));

                //Update card order
                cardOrder[cardIndex] = otherCard;
                cardOrder[otherCardIndex] = selectedCard;
            }

            //Input cooldown
            canChangeCards = false;
            StartCoroutine(HorizontalCooldown(selectedCard.moveTime));
        }

        if (gamepad.GetStick_L().X == -1)
        {
            //Get card indexes
            int cardIndex = cardOrder.IndexOf(selectedCard);
            int otherCardIndex = cardIndex - 1;

            if (otherCardIndex < 0)
            {
                otherCardIndex = cardOrder.Count-1;
            }

            //Get other card
            CardBehaviour otherCard = cardOrder[otherCardIndex];
            CardBehaviour middleCard = null;
            if (otherCardIndex == cardOrder.Count - 1)
            {
                middleCard = cardOrder[1];
            }

            //Swap card positions
            selectedCard.Move(new Vector3(otherCard.transform.position.x, selectedCard.offsetY, otherCard.transform.position.z));

            if (middleCard != null)
            {
                otherCard.Move(new Vector3(middleCard.transform.position.x, middleCard.originY, middleCard.transform.position.z));
                middleCard.Move(new Vector3(selectedCard.transform.position.x, selectedCard.originY, selectedCard.transform.position.z));

                //Update card order
                cardOrder[1] = otherCard;
                cardOrder[otherCardIndex] = selectedCard;
                cardOrder[cardIndex] = middleCard;
            }

            else
            {
                otherCard.Move(new Vector3(selectedCard.transform.position.x, selectedCard.originY, selectedCard.transform.position.z));

                //Update card order
                cardOrder[cardIndex] = otherCard;
                cardOrder[otherCardIndex] = selectedCard;
            }

            //Input cooldown
            canChangeCards = false;
            StartCoroutine(HorizontalCooldown(selectedCard.moveTime));
        }
    }

    IEnumerator HorizontalCooldown(float _horCooldown)
    {
        yield return new WaitForSeconds(_horCooldown);
        canChangeCards = true;
    }
}
