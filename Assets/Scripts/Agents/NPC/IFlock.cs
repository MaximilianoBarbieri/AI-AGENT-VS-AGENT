using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlock
{
    Vector3 GetDir(List<NPC> entities, IFlockEntity entity);
}