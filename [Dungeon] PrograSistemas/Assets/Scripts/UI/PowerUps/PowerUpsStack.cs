using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PowerUps
{
    public class PowerUpsStack : MonoBehaviour
    {
        private Stack<PowerUpData> powerUpsStack = new Stack<PowerUpData>(); //Cola de powerUps
        private int maxSize = 3; // Tamaño del array
        public PowerUpData currentPowerUp;

        public event Action<Sprite> OnPowerUpChanged;
        
        public PowerUpData CheckCurrentPowerUp()
        {
            if (powerUpsStack.Count == 0)
            {
                return null;
            }
            return currentPowerUp;
        }

        public void AddPowerUp(PowerUpData powerUp)
        {
            if (powerUpsStack.Count >= maxSize)
            {
                Debug.Log("La cola está llena");
                return;
            }
            
            powerUpsStack.Push(powerUp); //Agrego la referencia del objeto a la lista
            CheckUpdateList();
        }

        public void RemovePowerUp() //Quito el primer objeto que entr� a la lista
        {
            if (powerUpsStack.Count == 0)
            {
                return;
            }
            powerUpsStack.Pop();
            CheckUpdateList();
        }

        private void CheckUpdateList()
        {
            if (powerUpsStack.Count == 0)
            {
                currentPowerUp = null;
            }
            else
            {
                currentPowerUp = powerUpsStack.Peek();
                OnPowerUpChanged?.Invoke(currentPowerUp.GetPowerUpIcon);
            }
        }
    }
}
