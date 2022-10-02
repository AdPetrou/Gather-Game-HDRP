using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using GatherGame.Actors.Stats;
using MyBox;

namespace GatherGame.Actors 
{
    public class ActorBehaviour : MonoBehaviour
    {
        #region Variables
        #region Editor Variables

        [Header("State Variables")]
        [SerializeField] [DisplayInspector]
        protected StateMachine stateMachine;
        [SerializeField]
        protected State defaultState;   // Mostly used as a return State after most actions, might move to State Machine
        [SerializeField] [MyBox.ReadOnly]
        protected State currentState;
        public State CurrentState
        {
            get
            {
                if (currentState != null)
                    return currentState;
                else
                {
                    ChangeState(defaultState.StateRef);
                    return currentState;
                }
            }
            protected set { currentState = value; }
        }

        [Header("Actor Rules")]
        [SerializeField]
        protected bool canHarvest = false;      
        // This should only be Active for Actors that have the ability to harvest objects,
        // otherwise this needs to be turned off for performance reasons
        // Unclear if this will need to change for interactable objects that aren't harvestable

        [SerializeField] [ConditionalField(nameof(canHarvest))] [Range(0f, 2f)]
        protected float scanRadius;             // Radius of the Spherecast to get an object, 0.5f seems to be the sweet spot for now
        [SerializeField]
        protected bool canChange = true;        // Wether the state can change or not, this is disabled for uninterruptable Actions

        public AnimationController animationController { get; protected set; }
        protected CharacterController controller;
        #endregion

        public Vector3 direction { get; protected set; }
        public float velocity { get; protected set; } = 0;  // The velocity used to move the Character Controller, needs tweaking

        internal FindInteractableCommand findClosestInteractable; // The Spherecast Job
        public GameObject interactableObject { get; protected set; }    // The Return value from the Spherecast Job
        public bool commandRunning { get; private set; } = false;   // This is True while the Spherecast Job is running
        #endregion

        #region Jobs
        internal struct FindInteractableCommand
        {
            public RaycastHit result;
            private NativeArray<RaycastHit> sphereCastResult;
            private NativeArray<SpherecastCommand> command;

            private bool FindClosestInteractable()
            {
                bool found = false;
                //NativeArray<SpherecastCommand> filler = new NativeArray<SpherecastCommand>(1, Allocator.TempJob);
                if (sphereCastResult.Length == 0)
                    return false;

                float closestDistance = 10000f;

                for (int i = 0; i < sphereCastResult.Length; i++)
                {
                    RaycastHit hit = sphereCastResult[i];
                    // These conditions should be met if the object can be interacted
                    if (hit.transform && hit.transform.tag[0] == '~' && hit.transform.GetComponent<Renderer>().enabled)
                    {
                        //Debug.DrawRay(command[i].origin, hit.transform.position, Color.green, 2f);
                        float distance = (command[i].origin - hit.transform.position).sqrMagnitude;
                        found = true;
                        if (distance < closestDistance)
                        {
                            result = hit;
                            closestDistance = distance;
                        }
                    }
                }
                return found;
            }

            public IEnumerator RunJob(System.Action<GameObject> thisObject, System.Action<bool> finished)
            {
                GameObject resultObj;
                JobHandle spherecastJob = SpherecastCommand.ScheduleBatch(command, sphereCastResult, 1, new JobHandle());
                spherecastJob.Complete();   // Schedules the Jobs to run and returns the results in SphereCastResult
                bool hasObject = FindClosestInteractable(); // Checks if it's hit an object and returns true if it has

                if (!hasObject)
                    resultObj = null;
                else
                    resultObj = result.transform.gameObject;
                Dispose();  // Native Arrays need to be disposed quickly after being created to avoid Memory Leaks

                // Callbacks the result to set in the class
                thisObject(resultObj);
                yield return new WaitForSeconds(0.2f);  
                // This waits before returning true to give some downtime to the Jobs and stop 100s of jobs a second
                finished(true);
                yield break;
            }

            public void Stop()
            {
                // This just calls Dispose for now but might need more later
                // Clears the NativeArrays so the job can be reset prematurely
                Dispose();
            }

            public FindInteractableCommand(Vector3 actorPos, Vector3[] directions, float scanRadius)
            {
                // Sets up the Layer Mask so the Raycast only effects the Interactables Layer
                LayerMask layerMask = 1 << Interaction.InteractionManager.InteractableObjects[0].Layer;   
                int casts = directions.Length;
                // Native Array needed to return Values from Jobs
                sphereCastResult = new NativeArray<RaycastHit>(casts, Allocator.TempJob);
                command = new NativeArray<SpherecastCommand>(casts, Allocator.TempJob);
                result = new RaycastHit();
                for (int i = 0; i < casts; i++)
                { 
                    //Debug.DrawRay(actorPos, directions[i] * scanRadius, Color.green, 1);
                    command[i] = new SpherecastCommand(actorPos, scanRadius, 
                        directions[i], 1, layerMask);   // Spherecast is basically just a fat raycast that returns the first object it hits
                }
            }

            private void Dispose()
            {
                sphereCastResult.Dispose();
                command.Dispose();
            }
        }
        #endregion

        #region Unity Functions
        public virtual void Awake()
        {
            animationController = new AnimationController(gameObject.GetComponent<Animancer.AnimancerComponent>());
            controller = gameObject.GetComponent<CharacterController>();

            currentState = defaultState;
            RunCurrentState();
            interactableObject = null;
            commandRunning = false;
        }

        public void FixedUpdate()
        {
            if (canHarvest && canChange && !commandRunning && controller.velocity != Vector3.zero)
                GetInteractableObject(); // Called in Fixed Update so it can run before Update
        }
        
        // Update is called once per frame
        public virtual void Update()
        {
            if (!canChange)
                return;

            //Vector3 direction = controls.Player.Movement.ReadValue<Vector3>();
            //if (direction != Vector3.zero) { Walk(direction); return; }

            //int rotation = (int)controls.Player.Turning.ReadValue<float>();
            //if (rotation != 0) { Turn(rotation); return; }

            if (canChange && velocity == 0f) { DefaultState(); return; }
        }

        public void OnApplicationQuit()
        {
            
        }
        #endregion

        #region Methods

        #region State Methods
        public void DefaultState()
        {
            ChangeState(defaultState.StateRef);
        }

        public void canChangeSwitch(bool val)
        {
            canChange = val;
        }

        public void ChangeState(StateType type)
        {
            if (!canChange)
                return;

            if (type == currentState.StateRef)
            {
                RunCurrentState();
                return;
            }

            //Debug.Log("Called " + type.ToString());

            foreach (State state in stateMachine.States)
                if (type == state.StateRef)
                    foreach (State whitelistState in currentState.PossibleStates)
                        if (type == whitelistState.StateRef)
                        {
                            currentState = state;
                            //Debug.Log(currentState);
                            RunCurrentState();
                            return;
                        }
            RunCurrentState();
        }
        public void ChangeState(string type)
        {
            StateType refType = (StateType)System.Enum.Parse(typeof(StateType), type);
            if (!canChange)
                return;

            if (refType == currentState.StateRef)
            {
                RunCurrentState();
                return;
            }

            //Debug.Log("Called " + type);

            foreach (State state in stateMachine.States)
                if (refType == state.StateRef)
                    foreach (State whitelistState in currentState.PossibleStates)
                        if (refType == whitelistState.StateRef)
                        {
                            currentState = state;
                            RunCurrentState();
                            return;
                        }
            RunCurrentState();
        }

        public void SetState(StateType type)
        {
            foreach (State state in stateMachine.States)
                if (type == state.StateRef && state != currentState)
                    currentState = state;
        }
        public void SetState(string type)
        {
            foreach (State state in stateMachine.States)
                if ((StateType)System.Enum.Parse(typeof(StateType), type) == state.StateRef && state != currentState)
                    currentState = state;
        }

        public void ForceState(StateType type)
        {
            foreach (State state in stateMachine.States)
                if (type == state.StateRef)
                {
                    currentState = state;
                    RunCurrentState();
                    return;
                }
        }
        public void ForceState(string type)
        {
            foreach (State state in stateMachine.States)
                if ((StateType)System.Enum.Parse(typeof(StateType), type) == state.StateRef)
                {
                    currentState = state;
                    RunCurrentState();
                    return;
                }
        }

        public bool HasState(StateType type)
        {
            foreach(State state in stateMachine.States)
                if (state.StateRef == type)
                    return true;

            return false;
        }

        public void RunCurrentState()
        {
            currentState.Main(this);
        }
        public void RunState(StateType type)
        {
            foreach (State state in stateMachine.States)
                if (state.StateRef == type)
                {
                    state.Main(this);
                    return;
                }
        }
        #endregion

        public void GetInteractableObject(bool stopCurrent = false)
        {
            if (commandRunning && stopCurrent)
            {
                findClosestInteractable.Stop();
                commandRunning = false;
            }

            commandRunning = true;
            findClosestInteractable =
                new FindInteractableCommand(transform.position + Vector3.up * 2,
            new Vector3[]
            {
                transform.forward + (Vector3.down / 1.5f),
                //transform.forward + (transform.right / 5f) + (Vector3.down / 3f),
                //transform.forward - (transform.right / 5f) + (Vector3.down / 3f)
            },
            scanRadius);

            StartCoroutine(findClosestInteractable.RunJob // Delegates get the result from the coroutine and set it
                (newObject => { interactableObject = newObject;}, finished => { commandRunning = false; }));
        }

        public IEnumerator LookAtPosition(Vector3 targetPos)
        {
            if (!HasState(StateType.TurnLeft) && !HasState(StateType.TurnRight))
                yield break;

            float angle = Vector3.SignedAngle(transform.forward,
                Vector3.Normalize(targetPos - transform.position), Vector3.up);
            // The Angle between the forward vector of the actor and the direction vector of the Harvestable object
            // Signed Angle returns a negative or positive value depending on the closer angle

            float yRotation = Mathf.Abs(Mathf.FloorToInt(transform.eulerAngles.y)),
            // Take the Absolute value because unity uses -360 to 360
            upperBound = Mathf.Abs((Mathf.FloorToInt(yRotation + angle) + 5) % 360),
            // Upper and Lower bounds so the animation doesnt have to be exact
            lowerBound = Mathf.Abs((Mathf.FloorToInt(yRotation + angle) - 5) % 360);
            // Remainder and Absolute value from / 360 so value is between 0 and 360

            // All values are floored to make it easier to follow and debug
            float t = 0f;
            while ((yRotation < lowerBound || yRotation > upperBound) && t < 3f)
            {
                t += Time.deltaTime;
                // This is where the signed angle is useful,it gets the direction (index) to turn based on the angles result
                switch (angle / Mathf.Abs(angle))
                {
                    case 1:
                        if(HasState(StateType.TurnRight))
                            RunState(StateType.TurnRight);
                        else
                            RunState(StateType.TurnLeft);
                        break;
                    case -1:
                        if (HasState(StateType.TurnLeft))
                            RunState(StateType.TurnLeft);
                        else
                            RunState(StateType.TurnRight);
                        break;
                }
                yRotation = Mathf.Abs(Mathf.FloorToInt(transform.eulerAngles.y));
                yield return null;
            }

        }

        public void SetVelocity(float newVelocity)
        {
            velocity = newVelocity;
        }

        //protected void OnAnimatorIK(int layerIndex)
        //{
        //    if (animationController.animator)
        //    {
        //        float rotationOffset = 1f;
        //        if (turning)
        //            rotationOffset = 2f;

        //        animationController.DoIK(AvatarIKGoal.LeftFoot, rotationOffset);
        //        animationController.DoIK(AvatarIKGoal.RightFoot, rotationOffset);
        //    }
        //}
        #endregion
    }
}
