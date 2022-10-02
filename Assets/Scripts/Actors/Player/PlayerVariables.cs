using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GatherGame.Actors.Player
{
    public static class PlayerVariables
    {
        public static PlayerInput playerInput
        {
            get
            {
                if (playerInputRef == null)
                    playerInputRef = GameObject.FindObjectOfType<PlayerInput>();

                return playerInputRef;
            }
            set
            {
                playerInputRef = value;
            }
        }
        private static PlayerInput playerInputRef;
        public static Controls controls
        {
            get
            {
                if (controlsRef == null)
                {
                    controlsRef = new Controls();
                    controlsRef.Enable();
                }

                return controlsRef;
            }
            set
            {
                controlsRef = value;
            }
        }
        private static Controls controlsRef;
    }
}
