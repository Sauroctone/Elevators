using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour {

    public int health;
    public bool breaksShield;
    public AgentBehaviour agent;
    public int target;
    public Transform[] targets;
   
	public void Attack(Cards _previousCard)
    {
        if (_previousCard == Cards.Backstep)
        {
            target = target + 1;
            breaksShield = true;
        }

        if (_previousCard == Cards.Shield)
        {
            target = target + 2;
        }

        if (_previousCard == Cards.Attack)
        {
            target = target + 1;
        }

        target = Mathf.Clamp(target, 0, targets.Length-1);
        transform.position = targets[target].transform.position;
    }
}
