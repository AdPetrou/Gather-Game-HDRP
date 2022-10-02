using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatherGame.UI
{
    public class CloseParent : CloseUI
    {
        public override void disable()
        {
            transform.parent.parent.gameObject.SetActive(false);
        }
    }
}
