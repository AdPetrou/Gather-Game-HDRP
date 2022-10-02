using UnityEngine;
using System.Collections;
using GatherGame.Utilities;

namespace GatherGame.UI
{
    public class UIManager : Singleton<UIManager>
    {
        public static PlayerUI playerUI { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            playerUI = FindObjectOfType<PlayerUI>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setActivePopup(GameObject popup)
        {
            popup.SetActive(true);
            popup.transform.SetAsLastSibling();
        }     
    }
}
