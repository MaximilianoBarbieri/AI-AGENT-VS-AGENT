using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlockEntity
{
    Vector3 Dir { get; }
    Vector3 Pos { get; }
}