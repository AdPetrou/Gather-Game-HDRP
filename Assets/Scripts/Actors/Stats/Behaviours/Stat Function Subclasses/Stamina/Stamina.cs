using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.Actors.Stats
{
    public class Stamina : StatFunction
    {
        [Header("Stamina Drain Values")]
        [SerializeField][Range(0f, 10f)]
        protected float actionCost;
        [SerializeField]
        protected bool movementDrain;
        [SerializeField][ConditionalField("movementDrain")][Range(1f, 5f)]
        protected float movementDrainMultiplier;

        protected ActorBehaviour actor;

        // Start is called before the first frame update
        void Start()
        {
            actor = gameObject.GetComponent<ActorBehaviour>();
            StatBehaviour stats = gameObject.GetComponent<StatBehaviour>();
            isEnabled = false;

            foreach (ResourceStatBehaviour r in stats.resourceStats)
                if (r.type == StatType.Stamina)
                {
                    isEnabled = true;
                    elementUI = r;
                    value = r.value;
                }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isEnabled)
                return;

            if (preValue != value)
                main();
        }

        public override void main()
        {

        }

        public void drainStamina(float multiplier)
        {
            modValue(-actionCost * movementDrainMultiplier * multiplier);
        }

        private void modValue(float amount)
        {
            value += amount;
            elementUI.runUpdateUI(value);
        }
    } 
}
