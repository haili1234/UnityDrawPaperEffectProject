/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;

namespace NatCamU.Core.UI {

    using UnityEngine.UI;
    using Utilities;

    [CoreDoc(199), RequireComponent(typeof(RawImage))]
    public sealed class NatCamPanel : MonoBehaviour {

        public Orientation orientation {
            get {
                return _orientation;
            }
            set {
                _orientation = orientation;
                image.materialForRendering.SetFloat("_Rotation", ((int)value & 7) * 0.5f);
                image.materialForRendering.SetFloat("_Mirror", (int)value >> 3);
            }
        }
        
        private Material currMat, viewMat;
        private RawImage image;
        private Orientation _orientation;

        private void Awake () {
            image = GetComponent<RawImage>();
            currMat = image.material;
            viewMat = new Material(Shader.Find("Hidden/NatCam/Transform2D"));
            viewMat.SetFloat("_Zoom", 1f);
            image.material = viewMat;
        }

        private void OnDestroy () {
            if (image) image.material = currMat;
            Destroy(viewMat);
        }
    }
}