using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

    public Dictionary<Cards, CardBehaviour> cardDico = new Dictionary<Cards, CardBehaviour>();
    public List<CardBehaviour> cardOrder = new List<CardBehaviour>();

    [Range(1, 2)]
    public int player;
    public string attack;
    public string shield;
    public string backstep;
    public bool canChangeCards = true;

    x360_Gamepad gamepad;

    [Header("References")]
    public GameManager gameMan;
    public AgentBehaviour agent;
    GamepadManager gamepadMan;
    CardBehaviour selectedCard;

    void Start()
    {
        gamepadMan = GamepadManager.Instance;
        gamepad = gamepadMan.GetGamepad(player);

        for (int i = 0; i < cardOrder.Count; i++)
        {
            cardOrder[i].cardIndex = i;
        }
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
                cardOrder[cardIndex].cardIndex = cardIndex;
                cardOrder[otherCardIndex].cardIndex = cardIndex;
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

    void OnDisable()
    {
        if (selectedCard != null && selectedCard.isSelected && !selectedCard.isMoving)
            selectedCard.Deselect();
    }
}
