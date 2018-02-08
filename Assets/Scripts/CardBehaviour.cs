using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehaviour : MonoBehaviour {

    
    [Header("Nature")]

    public Cards cardNature;
    public CardController player;
    
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

    [Header("References")]
    public Renderer rend;

    void Start()
    {
        player.cardDico.Add(cardNature, this);
        originY = transform.position.y;
        originScale = transform.localScale;
        originMat = rend.material;
        newScale = new Vector3(originScale.x * scaleFactor, originScale.y, originScale.z * scaleFactor);
    }

    public void Hovered()
    {
        rend.material = hoverMat;
    }

    public void NotHovered()
    {
        rend.material = originMat;
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

    IEnumerator SelectCor()
    {
        isSelected = true;

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
    }

    IEnumerator MoveCor(Vector3 _newPos)
    {
        Vector3 firstPos = transform.position;

        float time = 0f;
        while (time < moveTime)
        {
            transform.position = Vector3.Lerp(firstPos, _newPos, moveCurve.Evaluate(time / moveTime));
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = _newPos;
    }
}
