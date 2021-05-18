using UnityEngine;
using System.Collections;
using X;
using UnityEngine.UI;
using XLua;

namespace X.UI
{
    [LuaCallCSharp]
    public class Select : MonoBehaviour
    {
        [SerializeField]
        Image image;

        bool selected = false;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                float f = 70f / 255f;
                image.color = selected ? Color.yellow : new Color( f , f , f );
            }
        }

        private void Awake()
        {
            image = GetComponent<Image>();
        }


    }
}
