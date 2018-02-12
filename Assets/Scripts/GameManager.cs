using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public List<Cards> cardTypes = new List<Cards>();

    public Rounds round;
    Coroutine roundCorWait;
    Coroutine roundCorFight;
    Coroutine playerTurns;

    [Header("Level movement")]
    public Vector3 waitPos;
    public Vector3 fightPos;
    public Vector3 fightPosTimer;
    public Vector3 buttonOutOfScreenPos;
    public float levelMoveTime;
    public AnimationCurve levelMoveCurve;
    public AnimationCurve timerMoveCurve;

    [Header("Round : Wait")]
    public Turns turn;
    public float flipDelay;
    public float peepTime;
    public float turnTime;
    public float roundTimeWait;
    public float rotTimeArrow;
    public AnimationCurve rotCurveArrow;
    public Color[] playerColors;

    [Header("Round : Fight")]
    public float agentTransTime;
    Transform agent1;
    Transform agent2;
    Vector3 agentOriginPos1;
    Vector3 agentOriginPos2;
    public AnimationCurve agentTransCurve;
    public float outOfScreenY;
    public float fightPosY;
    public int health1;
    public int health2;   

    [Header("References")]
    public CardController player1;
    public CardController player2;
    public Transform level;
    public RectTransform timer;
    public RectTransform buttons1;
    public RectTransform buttons2;
    public Image waitTimer;
    public Image turnTimer;
    x360_Gamepad gamepad1;
    x360_Gamepad gamepad2;
    GamepadManager gamepadMan;
    public GameObject arrow;

    void Start()
    {
        gamepadMan = GamepadManager.Instance;
        gamepad1 = gamepadMan.GetGamepad(1);
        gamepad2 = gamepadMan.GetGamepad(2);

        agent1 = player1.agent.transform.parent;
        agent2 = player2.agent.transform.parent;

        agentOriginPos1 = agent1.localPosition;
        agentOriginPos2 = agent2.localPosition;
    }

    void Update()
    {
        if (round == Rounds.Startup)
        {
            if (gamepad1.GetButtonDown("Start") || gamepad2.GetButtonDown("Start"))
            {
                round = Rounds.Wait;
                roundCorWait = StartCoroutine(WaitCor());
            }
        }

        if (round == Rounds.Fight)
        {
            if (health1 == 0)
            {
                round = Rounds.End;
                StartCoroutine(EndGame(player2));
            }

            if (health2 == 0)
            {
                round = Rounds.End;
                StartCoroutine(EndGame(player1));
            }
        }
    }

    public void EndTurn()
    {
        if (playerTurns != null)
        {
            StopCoroutine(playerTurns);
        }

        if (player1.gameObject.activeSelf)
        {
            player1.gameObject.SetActive(false);
            player2.gameObject.SetActive(true);
            StartCoroutine(ArrowCor(1));
            turn = Turns.Player2;
        }

        else
        {
            player2.gameObject.SetActive(false);
            player1.gameObject.SetActive(true);
            StartCoroutine(ArrowCor(-1));
            turn = Turns.Player1;
        }

        playerTurns = StartCoroutine(PlayerTurns());
    }

    IEnumerator WaitCor ()
    {
        StartCoroutine(FightToWaitCor());

        waitTimer.fillAmount = 1;
        turnTimer.fillAmount = 1;

        player1.Randomize();
        player2.Randomize();
        yield return new WaitForSeconds(2);

        //Show cards

        foreach (CardBehaviour card in player1.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        foreach (CardBehaviour card in player2.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        yield return new WaitForSeconds(peepTime);

        //Hide cards

        foreach (CardBehaviour card in player1.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        foreach (CardBehaviour card in player2.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        //Start turns

        playerTurns = StartCoroutine(PlayerTurns());

        if (turn == Turns.Player1)
            player1.gameObject.SetActive(true);
        if (turn == Turns.Player2)
            player2.gameObject.SetActive(true);

        StartCoroutine(WaitTimer());
        yield return new WaitForSeconds(roundTimeWait);
        
        //End turns

        StopCoroutine(playerTurns);
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);

        //Level transition
        StartCoroutine(WaitToFightCor());
        yield return new WaitForSeconds(levelMoveTime);

        //Agents transition

        float time = 0f;
        while (time < agentTransTime)
        {
            agent1.localPosition = Vector3.Lerp(agentOriginPos1, new Vector3(agentOriginPos1.x, outOfScreenY, agentOriginPos1.z), agentTransCurve.Evaluate(time / agentTransTime));
            agent2.localPosition = Vector3.Lerp(agentOriginPos2, new Vector3(agentOriginPos2.x, outOfScreenY, agentOriginPos2.z), agentTransCurve.Evaluate(time / agentTransTime));
            time += Time.deltaTime;
            yield return null;
        }
        agent1.localPosition = new Vector3(agentOriginPos1.x, outOfScreenY, agentOriginPos1.z);
        agent2.localPosition = new Vector3(agentOriginPos2.x, outOfScreenY, agentOriginPos2.z);

        player1.agent.anim.SetTrigger("FightMode");
        player2.agent.anim.SetTrigger("FightMode");
        yield return new WaitForSeconds(.5f);

        time = 0f;
        while (time < agentTransTime)
        {
            agent1.localPosition = Vector3.Lerp(new Vector3(agentOriginPos1.x, outOfScreenY, agentOriginPos1.z), new Vector3(agentOriginPos1.x, fightPosY, agentOriginPos1.z), agentTransCurve.Evaluate(time / agentTransTime));
            agent2.localPosition = Vector3.Lerp(new Vector3(agentOriginPos2.x, outOfScreenY, agentOriginPos2.z), new Vector3(agentOriginPos2.x, fightPosY, agentOriginPos2.z), agentTransCurve.Evaluate(time / agentTransTime));
            time += Time.deltaTime;
            yield return null;
        }
        agent1.localPosition = new Vector3(agentOriginPos1.x, fightPosY, agentOriginPos1.z);
        agent2.localPosition = new Vector3(agentOriginPos2.x, fightPosY, agentOriginPos2.z);

        //Fight

        round = Rounds.Fight;
        roundCorFight = StartCoroutine(FightCor());
    }

    IEnumerator FightCor()
    {
        CardBehaviour previousCard1 = null;
        CardBehaviour previousCard2 = null;

        for (int i = 0; i < 3; i++)
        {
            CardBehaviour card1 = player1.cardOrder[i];
            CardBehaviour card2 = player2.cardOrder[i];
            bool breaksShield1 = false;
            bool breaksShield2 = false;
            bool leaps1 = false;
            bool leaps2 = false;

            card1.Flip();
            card2.Flip();
            yield return new WaitForSeconds(1);

            if (card1.cardNature == Cards.Attack)
            {
                //Check attack's special properties

                if (previousCard1 != null && previousCard1.cardNature == Cards.Backstep)
                {
                    breaksShield1 = true;
                }

                if (previousCard1 != null && previousCard1.cardNature == Cards.Shield)
                {
                    leaps1 = true;
                }

                //Compare to other card

                if (card2.cardNature == Cards.Attack)
                {
                    player1.agent.Resolve(Cases.BlockedByAttack);
                    player2.agent.Resolve(Cases.BlockedByAttack);
                }

                if (card2.cardNature == Cards.Backstep)
                {
                    if (leaps1)
                    {
                        player1.agent.Resolve(Cases.Attack);
                        player2.agent.Resolve(Cases.Hurt);
                        health2--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.Attack);
                        player2.agent.Resolve(Cases.BackstepDodge);
                    }
                }

                if (card2.cardNature == Cards.Shield)
                {
                    if (breaksShield1)
                    {
                        player1.agent.Resolve(Cases.Attack);
                        player2.agent.Resolve(Cases.Hurt);
                        health2--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.BlockedByShield);
                        player2.agent.Resolve(Cases.Block);
                    }
                }
            }

            if (card1.cardNature == Cards.Backstep)
            {
                //Compare to other card

                if (card2.cardNature == Cards.Attack)
                {
                    if (previousCard2 != null && previousCard2.cardNature == Cards.Shield)
                    {
                        leaps2 = true;
                    }

                    if (leaps2)
                    {
                        player1.agent.Resolve(Cases.Hurt);
                        player2.agent.Resolve(Cases.Attack);
                        health1--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.BackstepDodge);
                        player2.agent.Resolve(Cases.Attack); 
                    }
                }

                if (card2.cardNature == Cards.Backstep)
                {
                    player1.agent.Resolve(Cases.BackstepPose);
                    player2.agent.Resolve(Cases.BackstepPose);
                }

                if (card2.cardNature == Cards.Shield)
                {
                    player1.agent.Resolve(Cases.BackstepPose);
                    player2.agent.Resolve(Cases.Block);
                }
            }

            if (card1.cardNature == Cards.Shield)
            {
                //Compare to other card

                if (card2.cardNature == Cards.Attack)
                {
                    if (previousCard2 != null && previousCard2.cardNature == Cards.Backstep)
                    {
                        breaksShield2 = true;
                    }

                    if (breaksShield2)
                    {
                        player1.agent.Resolve(Cases.Hurt);
                        player2.agent.Resolve(Cases.Attack);
                        health1--;
                    }

                    else
                    {
                        player1.agent.Resolve(Cases.Block);
                        player2.agent.Resolve(Cases.BlockedByShield);
                    }
                }

                if (card2.cardNature == Cards.Backstep)
                {
                    player1.agent.Resolve(Cases.Block);
                    player2.agent.Resolve(Cases.BackstepPose);
                }

                if (card2.cardNature == Cards.Shield)
                {
                    player1.agent.Resolve(Cases.Block);
                    player2.agent.Resolve(Cases.Block);
                }
            }

            //Save cards for next phase
            previousCard1 = card1;
            previousCard2 = card2;
            yield return new WaitForSeconds(2);
        }

        foreach(CardBehaviour card in player1.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }

        foreach (CardBehaviour card in player2.cardOrder)
        {
            card.Flip();
            yield return new WaitForSeconds(flipDelay);
        }
   
        //Agents transition

        float time = 0f;
        while (time < agentTransTime)
        {
            agent1.localPosition = Vector3.Lerp(new Vector3(agentOriginPos1.x, fightPosY, agentOriginPos1.z), new Vector3(agentOriginPos1.x, outOfScreenY, agentOriginPos1.z), agentTransCurve.Evaluate(time / agentTransTime));
            agent2.localPosition = Vector3.Lerp(new Vector3(agentOriginPos2.x, fightPosY, agentOriginPos2.z), new Vector3(agentOriginPos2.x, outOfScreenY, agentOriginPos2.z), agentTransCurve.Evaluate(time / agentTransTime));
            time += Time.deltaTime;
            yield return null;
        }
        agent1.localPosition = new Vector3(agentOriginPos1.x, outOfScreenY, agentOriginPos1.z);
        agent2.localPosition = new Vector3(agentOriginPos2.x, outOfScreenY, agentOriginPos2.z);

        player1.agent.anim.SetTrigger("WaitMode");
        player2.agent.anim.SetTrigger("WaitMode");
        yield return new WaitForSeconds(.5f);

        time = 0f;
        while (time < agentTransTime)
        {
            agent1.localPosition = Vector3.Lerp(new Vector3(agentOriginPos1.x, outOfScreenY, agentOriginPos1.z), agentOriginPos1, agentTransCurve.Evaluate(time / agentTransTime));
            agent2.localPosition = Vector3.Lerp(new Vector3(agentOriginPos2.x, outOfScreenY, agentOriginPos2.z), agentOriginPos2, agentTransCurve.Evaluate(time / agentTransTime));
            time += Time.deltaTime;
            yield return null;
        }
        agent1.localPosition = agentOriginPos1;
        agent2.localPosition = agentOriginPos2;

        //Wait

        round = Rounds.Wait;
        roundCorWait = StartCoroutine(WaitCor());
    }

    IEnumerator PlayerTurns ()
    {
        if (turn == Turns.Player1)
            turnTimer.color = playerColors[0];
        else
            turnTimer.color = playerColors[1];

        float time = 0;
        while (time < turnTime)
        {
            turnTimer.fillAmount = 1- time / turnTime;
            time += Time.deltaTime;
            yield return null;
        }
        
        EndTurn();
    }

    IEnumerator WaitTimer()
    {
        float time = 0;
        while (time < roundTimeWait)
        {
            waitTimer.fillAmount = 1 - time / roundTimeWait;
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator EndGame (CardController _winner)
    {
        yield return null;
    }

    IEnumerator ArrowCor(int _factor)
    {
        float time = 0;
        Vector3 originRot = arrow.transform.eulerAngles;
        Vector3 newRot = new Vector3(originRot.x, originRot.y, originRot.z - 180 * _factor);
        while (time < rotTimeArrow)
        {
            arrow.transform.eulerAngles = Vector3.Lerp(originRot, newRot, rotCurveArrow.Evaluate(time / rotTimeArrow));
            time += Time.deltaTime;
            yield return null;
        }
        arrow.transform.eulerAngles = newRot;
    }

    IEnumerator WaitToFightCor()
    {
        float time = 0f;
        while (time < levelMoveTime)
        {
            level.position = Vector3.Lerp(waitPos, fightPos, levelMoveCurve.Evaluate(time / levelMoveTime));
            timer.anchoredPosition = Vector3.Lerp(waitPos, fightPosTimer, timerMoveCurve.Evaluate(time / levelMoveTime));
            buttons1.anchoredPosition = Vector3.Lerp(waitPos, -buttonOutOfScreenPos, levelMoveCurve.Evaluate(time / levelMoveTime));
            buttons2.anchoredPosition = Vector3.Lerp(waitPos, buttonOutOfScreenPos, levelMoveCurve.Evaluate(time / levelMoveTime));
            time += Time.deltaTime;
            yield return null;
        }
        level.position = fightPos;
        timer.anchoredPosition = fightPosTimer;
        buttons1.anchoredPosition = -buttonOutOfScreenPos;
        buttons2.anchoredPosition = buttonOutOfScreenPos;
    }

    IEnumerator FightToWaitCor()
    {
        float time = 0f;
        while (time < levelMoveTime)
        {
            level.position = Vector3.Lerp(fightPos, waitPos, levelMoveCurve.Evaluate(time / levelMoveTime));
            timer.anchoredPosition = Vector3.Lerp(fightPosTimer, waitPos, timerMoveCurve.Evaluate(time / levelMoveTime));
            buttons1.anchoredPosition = Vector3.Lerp(-buttonOutOfScreenPos, waitPos, levelMoveCurve.Evaluate(time / levelMoveTime));
            buttons2.anchoredPosition = Vector3.Lerp(buttonOutOfScreenPos, waitPos, levelMoveCurve.Evaluate(time / levelMoveTime));
            time += Time.deltaTime;
            yield return null;
        }
        level.position = waitPos;
        timer.anchoredPosition = waitPos;
        buttons1.anchoredPosition = waitPos;
        buttons2.anchoredPosition = waitPos;
    }
}
