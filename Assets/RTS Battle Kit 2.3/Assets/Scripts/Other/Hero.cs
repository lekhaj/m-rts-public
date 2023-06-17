using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heroes")]
public class Hero : ScriptableObject
{
    public Heroes HeroType;

    public enum Heroes
    {
        Archer,
        Giant,
        Bot,
        Cavalry,
        Healer,
        GoblinCavalry,
        Goblin,
        KnightWithShield,
        Knight,
        UnArmedKnight,
        Wizard
    }
}
