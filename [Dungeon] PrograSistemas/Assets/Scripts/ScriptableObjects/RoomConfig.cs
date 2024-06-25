using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewRoomConfig", menuName = "Room Config", order = 1)]
public class RoomConfig : ScriptableObject
{
    public int easyDiffPoints;
    public int hardDiffPoints;
}
