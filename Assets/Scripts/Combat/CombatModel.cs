using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostSloth.Combat
{
    public interface ITurnInfo
    {
        public bool IsEnemyTurn { get; }
    }
}
