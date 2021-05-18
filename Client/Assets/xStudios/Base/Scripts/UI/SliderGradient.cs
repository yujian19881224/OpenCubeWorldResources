using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace X.UI
{
    [LuaCallCSharp]
    public class SliderGradient : MonoBehaviour
    {
        [SerializeField]
        Image image;

        [SerializeField]
        Color color = Color.white;

        [SerializeField]
        ImageGradient rGradient;
        [SerializeField]
        ImageGradient gGradient;
        [SerializeField]
        ImageGradient bGradient;
        [SerializeField]
        ImageGradient aGradient;

        [SerializeField]
        Slider rSlider;
        [SerializeField]
        Slider gSlider;
        [SerializeField]
        Slider bSlider;
        [SerializeField]
        Slider aSlider;

        GradientColorKey[] rGradientColor = new GradientColorKey[ 2 ];
        GradientColorKey[] gGradientColor = new GradientColorKey[ 2 ];
        GradientColorKey[] bGradientColor = new GradientColorKey[ 2 ];
        GradientColorKey[] aGradientColor = new GradientColorKey[ 2 ];

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                UpdateGradient();
            }
        }

        public ImageGradient RGradient
        {
            get
            {
                return rGradient;
            }
        }
        public ImageGradient GGradient
        {
            get
            {
                return gGradient;
            }
        }
        public ImageGradient BGradient
        {
            get
            {
                return bGradient;
            }
        }
        public ImageGradient AGradient
        {
            get
            {
                return aGradient;
            }
        }

        private void Start()
        {
            if ( rSlider )
            {
                rSlider.onValueChanged.RemoveListener( OnValueChange );
                rSlider.onValueChanged.AddListener( OnValueChange );
            }
            if ( gSlider )
            {
                gSlider.onValueChanged.RemoveListener( OnValueChange );
                gSlider.onValueChanged.AddListener( OnValueChange );
            }
            if ( bSlider )
            {
                bSlider.onValueChanged.RemoveListener( OnValueChange );
                bSlider.onValueChanged.AddListener( OnValueChange );
            }
            if ( aSlider )
            {
                aSlider.onValueChanged.RemoveListener( OnValueChange );
                aSlider.onValueChanged.AddListener( OnValueChange );
            }

            rGradientColor[ 0 ] = new GradientColorKey( new Color( 0f , color.g , color.b ) , 0f );
            rGradientColor[ 1 ] = new GradientColorKey( new Color( 1f , color.g , color.b ) , 1f );

            gGradientColor[ 0 ] = new GradientColorKey( new Color( color.r , 0f , color.b ) , 0f );
            gGradientColor[ 1 ] = new GradientColorKey( new Color( color.r , 1f , color.b ) , 1f );

            bGradientColor[ 0 ] = new GradientColorKey( new Color( color.r , color.g , 0f ) , 0f );
            bGradientColor[ 1 ] = new GradientColorKey( new Color( color.r , color.g , 1f ) , 1f );

            aGradientColor[ 0 ] = new GradientColorKey( new Color( 0f , 0f , 0f ) , 0f );
            aGradientColor[ 1 ] = new GradientColorKey( new Color( 1f , 1f, 1f ) , 1f );

            UpdateGradient();
        }

        void UpdateGradient()
        {
            if ( rGradient )
            {
                rGradientColor[ 0 ] = new GradientColorKey( new Color( 0f , color.g , color.b ) , 0f );
                rGradientColor[ 1 ] = new GradientColorKey( new Color( 1f , color.g , color.b ) , 1f );
                rGradient.gradientColor.SetKeys( rGradientColor , rGradient.gradientColor.alphaKeys );
                rGradient.enabled = false;
                rGradient.enabled = true;
            }
            if ( gGradient )
            {
                gGradientColor[ 0 ] = new GradientColorKey( new Color( color.r , 0f , color.b ) , 0f );
                gGradientColor[ 1 ] = new GradientColorKey( new Color( color.r , 1f , color.b ) , 1f );
                gGradient.gradientColor.SetKeys( gGradientColor , gGradient.gradientColor.alphaKeys );
                gGradient.enabled = false;
                gGradient.enabled = true;
            }
            if ( bGradient )
            {
                bGradientColor[ 0 ] = new GradientColorKey( new Color( color.r , color.g , 0f ) , 0f );
                bGradientColor[ 1 ] = new GradientColorKey( new Color( color.r , color.g , 1f ) , 1f );
                bGradient.gradientColor.SetKeys( bGradientColor , bGradient.gradientColor.alphaKeys );
                bGradient.enabled = false;
                bGradient.enabled = true;
            }
            if ( aGradient )
            {
                aGradient.gradientColor.SetKeys( aGradientColor , aGradient.gradientColor.alphaKeys );
            }

            if ( image )
            {
                image.color = color;
            }
        }

        void OnValueChange( float f )
        {
            if ( rSlider )
            {
                color.r = rSlider.value;
            }
            if ( rSlider )
            {
                color.g = gSlider.value;
            }
            if ( bSlider )
            {
                color.b = bSlider.value;
            }
            if ( aSlider )
            {
                color.a = aSlider.value;
            }

            UpdateGradient();
        }


    }

}
