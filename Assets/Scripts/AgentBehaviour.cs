using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour {

    public Animator anim;

	public void Resolve(Cases _resolveCase)
    {
        anim.SetTrigger(_resolveCase.ToString());
    }
}
