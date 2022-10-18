
using UnityEngine;
using UnityEngine.EventSystems;

namespace GatherGame.UI
{
    public class CloseUI : EventTrigger
    {
        [SerializeField]
        protected GameObject obj;
        protected Vector3 offset;

        public virtual void Awake()
        {
            EventTrigger trigger = this;
            Entry entry = new Entry();
            entry.eventID = EventTriggerType.PointerDown;
            trigger.triggers.Add(entry);
        }

        public void SetObject(GameObject obj)
        {
            this.obj = obj;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            disable();
        }

        public virtual void disable()
        {
            obj.SetActive(false);
        }
    }
}
