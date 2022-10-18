using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GatherGame.Actors.Player
{
    public class PlayerBehaviour : ActorBehaviour
    {
        #region Variables
        bool jogToggle = false;
        public Inventory.InventoryScriptable tempBackpack;
        #endregion

        #region Unity Functions
        public override void Awake()
        {
            base.Awake();
            tempBackpack.CreateInventory(transform);
        }

        // Update is called once per frame
        public override void Update()
        {
            if (!canChange)
                return;

            direction = PlayerVariables.controls.Player.Movement.ReadValue<Vector3>();
            if (direction != Vector3.zero || velocity > 0f)
            {
                if (PlayerVariables.controls.Player.Sprinting.inProgress)
                {
                    ChangeState(StateType.Sprint);
                    return;
                }
                else if (jogToggle)
                {
                    ChangeState(StateType.Jog);
                    return;
                }
                else
                {
                    ChangeState(StateType.Walk);
                    return;
                }
            }

            int rotation = (int)PlayerVariables.controls.Player.Turning.ReadValue<float>();
            switch (rotation)
            {
                case 0:
                    break;
                case 1:
                    ChangeState(StateType.TurnLeft);
                    return;
                case -1:
                    ChangeState(StateType.TurnRight);
                    return;
            }

            base.Update();
        }

        public void JogToggle()
        {
            jogToggle = !jogToggle;
        }

        public void HarvestObject(InputAction.CallbackContext context)
        {
            if (context.started && canChange)
                ChangeState(StateType.Interact);
        }

        public static IEnumerator disableMovement(float time)
        {
            PlayerVariables.controls.Player.Disable();
            yield return new WaitForSeconds(time);
            PlayerVariables.controls.Player.Enable();
        }

        #endregion
    }
}
