using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GatherGame.Utilities;
using GatherGame.Timers;
using System.Linq;
using System.Reflection;

namespace GatherGame.Interaction
{
    public class InteractionManager : Singleton<InteractionManager>, IThreadedTimerList<GameObject>
    {
        #region Variables
        private static InteractableObject[] interactableObjects;
        public static InteractableObject[] InteractableObjects
        {
            get
            {
                if (interactableObjects == null)
                    interactableObjects = Resources.LoadAll<InteractableObject>("Interactable Objects").ToArray();
                return interactableObjects;
            }

            private set
            {
                interactableObjects = value;
            }
        }

        public ThreadedTimerList<GameObject> Timer { get; protected set; }

        #endregion

        #region Unity Functions
        // Start is called before the first frame update
        private void Start()
        {
            Timer = new ThreadedTimerList<GameObject>();
            if (InteractableObjects != null)
            {

            }
        }
        

        private void Update()
        {
            if (Timer.results.Count > 0)
                ProcessTimer();
        }

        #endregion

        #region Methods
        public InteractableObject GetObjectFromTag(string tag)
        {
            foreach(InteractableObject obj in InteractableObjects)
                if (obj.GetTag() == tag)
                    return obj;

            return null;
        }
        
        public void ProcessTimer()
        {
            GameObject gameObject = Timer.results.Dequeue();
            gameObject.SetActive(true);
        }

        private void OnApplicationQuit()
        {

        }
        #endregion
    }
}

