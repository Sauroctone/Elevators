﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Cards { Attack, Shield, Backstep };
public enum Rounds { Startup, Wait, Fight, End };
public enum Cases { Attack, Hurt, BlockedByAttack, BlockedByShield, BackstepPose, BackstepDodge, Block };
public enum Turns { Player1, Player2 };