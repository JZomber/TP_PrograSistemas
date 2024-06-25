using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private void OnEnable()
    {
        ModularRooms.OnPlayerEnteredRoom += HandlePlayerEnteredRoom;
    }

    private void OnDisable()
    {
        ModularRooms.OnPlayerEnteredRoom -= HandlePlayerEnteredRoom;
    }

    private void HandlePlayerEnteredRoom(ModularRooms room)
    {
        StartCoroutine(UnlockRoomAfterDelay(room, 3f)); //Espera 3 segundos antes de ejecutarse
    }
    
    private IEnumerator UnlockRoomAfterDelay(ModularRooms room, float delay)
    {
        yield return new WaitForSeconds(delay);
        room.UnlockRoom();
    }
}
