using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private ModularRooms activeRoom;
    
    private void OnEnable()
    {
        ModularRooms.OnPlayerEnteredRoom += HandlePlayerEnteredRoom;

        EnemyManager.OnRoomCompleted += HandlerRoomCompleted;
    }

    private void OnDisable()
    {
        ModularRooms.OnPlayerEnteredRoom -= HandlePlayerEnteredRoom;
        
        EnemyManager.OnRoomCompleted -= HandlerRoomCompleted;
    }

    private void HandlePlayerEnteredRoom(ModularRooms room)
    {
        activeRoom = room;
    }

    private void HandlerRoomCompleted()
    {
        StartCoroutine(UnlockRoomAfterDelay(2f));
    }
    
    private IEnumerator UnlockRoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        activeRoom.UnlockRoom();
    }
}
