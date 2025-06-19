using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Networking/CardInfo")]
public class CardInfo : ScriptableObject
{
    public List<CardSO> cards;
}
