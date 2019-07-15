using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public interface IEnemy {

    int Damage { get; }

    float AttackTimeout { get; }

    float SpawnRate { get;  }

    EnemyType IsType { get; }

    //event Action<IEnemy> EnemyAttacked;

}
