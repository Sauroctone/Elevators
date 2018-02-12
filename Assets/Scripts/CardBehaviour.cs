using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviour : MonoBehaviour {
    
    [Header("Nature")]

    public Cards cardNature;
    public CardController player;
    public int cardIndex;
    
    [Header("Selection")]

    public AnimationCurve offsetCurve;
    public float offsetTime;
    public float offsetY;
    public float originY;
    public float scaleFactor;
    Vector3 originScale;
    Vector3 newScale;
    public bool isSelected;
    public Material hoverMat;
    Material originMat;

    [Header("Movement")]

    public AnimationCurve moveCurve;
    public float moveTime;
    public bool isMoving;
    Vector3 originPos;

    [Header("Flip")]
    public AnimationCurve flipCurve;
    public float flipTime;
    Vector3 originRot;

    [Header("References")]
    public Renderer rend;
    public Renderer faceRend;
    public Material matAttack;
    public Material matBackstep;
    public Material matShield;

    void Start()
    {
        player.cardDico.Add(cardNature, this);
        originY = transform.position.y;
        originScale = transform.localScale;
        originRot = transform.eulerAngles;
        originMat = rend.material;
        newScale = new Vector3(originScale.x * scaleFactor, originScale.y, originScale.z * scaleFactor);
    }

	public void Select()
    {
        StartCoroutine(SelectCor());
    }

    public void Deselect()
    {
        StartCoroutine(DeselectCor()); 
    }

    public void Move(Vector3 _newPos)
    {
        StartCoroutine(MoveCor(_newPos));
    }

    public void Flip()
    {
        StartCoroutine(FlipCor());
    }

    public void UpdateCard()
    {
        if (cardNature == Cards.Attack)
            faceRend.material = matAttack;
        if (cardNature == Cards.Backstep)
            faceRend.material = matBackstep;
        if (cardNature == Cards.Shield)
            faceRend.material = matShield;
    }

    IEnumerator SelectCor()
    {
        isSelected = true;
        originPos = transform.position; 

        Vector3 pos = transform.position;
        float time = 0f;
        while(time < offsetTime)
        {
            transform.position = Vector3.Lerp(pos, new Vector3(pos.x, offsetY, pos.z), offsetCurve.Evaluate(time/offsetTime));
            transform.localScale = Vector3.Lerp(originScale, newScale, offsetCurve.Evaluate(time / offsetTime));

            time += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(pos.x, offsetY, pos.z);
        transform.localScale = newScale;
    }

    IEnumerator DeselectCor()
    {
        isSelected = false;

        Vector3 pos = transform.position;
        float time = 0f;
        while (time < offsetTime)
        {
            transform.position = Vector3.Lerp(pos, new Vector3(pos.x, originY, pos.z), offsetCurve.Evaluate(time / offsetTime));
            transform.localScale = Vector3.Lerp(newScale, originScale, offsetCurve.Evaluate(time / offsetTime));
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(pos.x, originY, pos.z);
        transform.localScale = originScale;

        if (transform.position != originPos && player.gameObject.activeSelf)
        {
            player.gameMan.EndTurn();
        }
    }

    IEnumerator MoveCor(Vector3 _newPos)
    {
        isMoving = true;
        Vector3 firstPos = transform.position;

        float time = 0f;
        while (time < moveTime)
        {
            transform.position = Vector3.Lerp(firstPos, _newPos, moveCurve.Evaluate(time / moveTime));
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _newPos;

        if (!player.gameObject.activeSelf && isSelected)
        {
            Deselect();
        }

        isMoving = false;
    }

    IEnumerator FlipCor()
    {
        float time = 0;
        originRot = transform.eulerAngles;
        Vector3 newRot = new Vector3(originRot.x, originRot.y, originRot.z + 180);
        while (time < flipTime)
        {
            transform.eulerAngles = Vector3.Lerp(originRot, newRot, flipCurve.Evaluate(time/flipTime));
            time += Time.deltaTime;
            yield return null;
        }
        transform.eulerAngles = newRot;
    }
}
