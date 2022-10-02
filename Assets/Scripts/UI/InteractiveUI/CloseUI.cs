
using UnityEngine;
using UnityEngine.EventSystems;

namespace GatherGame.UI
{
    public class CloseUI : EventTrigger
    {
        protected Vector3 offset;

        public virtual void Awake()
        {
            EventTrigger trigger = this;
            Entry entry = new Entry();
            entry.eventID = EventTriggerType.Drag;
            trigger.triggers.Add(entry);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            disable();
        }

        public virtual void disable()
        {
            gameObject.SetActive(false);
        }
    }
}
