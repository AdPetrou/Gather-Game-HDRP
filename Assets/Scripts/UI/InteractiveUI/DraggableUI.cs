using UnityEngine;
using UnityEngine.EventSystems;

namespace GatherGame.UI
{ 
    public class DraggableUI : EventTrigger
    {
        [SerializeField]
        private GameObject obj;
        protected Vector2 offset;

        public virtual void Awake()
        {
            obj = gameObject;
            EventTrigger trigger = this;
            Entry entry = new Entry();
            entry.eventID = EventTriggerType.Drag;
            trigger.triggers.Add(entry);
        }

        public void SetObject(GameObject obj)
        {
            this.obj = obj;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            obj.transform.position = new Vector2(eventData.position.x, eventData.position.y) - offset;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            offset = eventData.position - new Vector2(obj.transform.position.x, obj.transform.position.y);
        }
    }
}
