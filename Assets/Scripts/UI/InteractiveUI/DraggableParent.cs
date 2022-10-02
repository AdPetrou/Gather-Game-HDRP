using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GatherGame.UI;

namespace GatherGame.UI
{
    public class DraggableParent : DraggableUI
    {
        public Vector3 height = Vector3.zero;

        public override void OnDrag(PointerEventData eventData)
        {
            height = new Vector3(0, -transform.localPosition.y, 0);
            transform.parent.position = new Vector3(eventData.position.x, eventData.position.y) + height - offset;
        }
    }
}
