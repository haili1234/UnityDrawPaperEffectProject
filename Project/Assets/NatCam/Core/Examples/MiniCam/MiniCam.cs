/* 
*   NatCam Core
*   Copyright (c) 2016 Yusuf Olokoba
*/

using NatCamU.Pro;

namespace NatCamU.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using Core;
    using Core.UI;

    public class MiniCam : NatCamBehaviour {
        
        [Header("UI")]
        public AspectRatioFitter aspectFitter;
        public NatCamPanel photoPanel;
        public NatCamFocuser focuser;
        public Text flashText;
        public Button switchCamButton, flashButton;
        public Image checkIco, flashIco;
        private Texture2D photo;

#if NATCAM_PRO || NATCAM_PROFESSIONAL

        public void StartRecording () {
            // Start recording
            NatCam.StartRecording(Configuration.Default, OnVideo);
        }

        public void StopRecording () {
            // Stop recording // The OnVideo callback will then be invoked with the video path
            NatCam.StopRecording();
        }

        private void OnVideo (string path) {
#if UNITY_IOS || UNITY_ANDROID
            // Play the video
            Debug.Log("原生录制："+path);
            //Handheld.PlayFullScreenMovie(path);
#endif
        }
#endif
        

        #region --Unity Messages--

        // Use this for initialization
        public override void Start () {
            // Start base
            base.Start();
            // Set the flash icon
            SetFlashIcon();
        }
        #endregion

        
        #region --NatCam and UI Callbacks--

        public override void OnStart () {
            // Display the preview
            preview.texture = NatCam.Preview;
            // Scale the panel to match aspect ratios
            aspectFitter.aspectRatio = NatCam.Preview.width / (float)NatCam.Preview.height;
            // Start tracking focus gestures
            focuser.StartTracking();
        }
        
        protected virtual void OnPhoto (Texture2D photo, Orientation orientation) {
            // Cache the photo
            this.photo = photo;
            // Display the photo
            preview.texture = photo;
            // Scale the panel to match aspect ratios
            aspectFitter.aspectRatio = photo.width / (float)photo.height;
            // Set the orientation so that the photo is displayed upright
            photoPanel.orientation = orientation;
            // Enable the check icon
            checkIco.gameObject.SetActive(true);
            // Disable the switch camera button
            switchCamButton.gameObject.SetActive(false);
            // Disable the flash button
            flashButton.gameObject.SetActive(false);
        }

        private void OnView () {
            // Disable the check icon
            checkIco.gameObject.SetActive(false);
            // Display the preview
            preview.texture = NatCam.Preview;
            // Scale the panel to match aspect ratios
            aspectFitter.aspectRatio = NatCam.Preview.width / (float)NatCam.Preview.height;
            // Reset the panel orientation
            photoPanel.orientation = 0;
            // Enable the switch camera button
            switchCamButton.gameObject.SetActive(true);
            // Enable the flash button
            flashButton.gameObject.SetActive(true);
            // Free the photo texture
            Texture2D.Destroy(photo); photo = null;
        }
        #endregion
        
        
        #region --UI Ops--

        public virtual void CapturePhoto () {
            // Divert control if we are checking the captured photo
            if (!checkIco.gameObject.activeInHierarchy) NatCam.CapturePhoto(OnPhoto);
            // Check captured photo
            else OnView();
        }
        
        public void SwitchCamera () {
            // Switch camera
            base.SwitchCamera();
            // Set the flash icon
            SetFlashIcon();
        }
        
        public void ToggleFlashMode () {
            // Set the active camera's flash mode
            NatCam.Camera.FlashMode = NatCam.Camera.IsFlashSupported ? NatCam.Camera.FlashMode == FlashMode.Auto ? FlashMode.On : NatCam.Camera.FlashMode == FlashMode.On ? FlashMode.Off : FlashMode.Auto : NatCam.Camera.FlashMode;
            // Set the flash icon
            SetFlashIcon();
        }

        public void ToggleTorchMode () {
            // Set the active camera's torch mode
            NatCam.Camera.TorchMode = NatCam.Camera.TorchMode == TorchMode.Off ? TorchMode.On : TorchMode.Off;
        }
        #endregion


        #region --Utility--
        
        private void SetFlashIcon () {
            // Null checking
            if (!NatCam.Camera) return;
            // Set the icon
            flashIco.color = !NatCam.Camera.IsFlashSupported || NatCam.Camera.FlashMode == FlashMode.Off ? (Color)new Color32(120, 120, 120, 255) : Color.white;
            // Set the auto text for flash
            flashText.text = NatCam.Camera.IsFlashSupported && NatCam.Camera.FlashMode == FlashMode.Auto ? "A" : "";
        }
        #endregion
    }
}