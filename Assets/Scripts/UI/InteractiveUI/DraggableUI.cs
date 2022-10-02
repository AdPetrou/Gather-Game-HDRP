using UnityEngine;
using UnityEngine.EventSystems;

namespace GatherGame.UI
{ 
    public class DraggableUI : EventTrigger
    {
        protected Vector3 offset;

        public virtual void Awake()
        {
            EventTrigger trigger = this;
            Entry entry = new Entry();
            entry.eventID = EventTriggerType.Drag;
            trigger.triggers.Add(entry);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            transform.position = new Vector3(eventData.position.x, eventData.position.y) - offset;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
        }
    }
}
