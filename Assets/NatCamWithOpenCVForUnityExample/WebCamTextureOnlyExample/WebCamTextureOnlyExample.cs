﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.UI;

namespace NatCamWithOpenCVForUnityExample
{

    /// <summary>
    /// WebCamTexture Only Example
    /// An example of displaying the preview frame of camera only using WebCamTexture API.
    /// </summary>
    public class WebCamTextureOnlyExample : ExampleBase<WebCamSource>
    {

        Texture2D texture;
        Color32[] pixelBuffer;

        FpsMonitor fpsMonitor;

        protected override void Start ()
        {
            // Load global camera benchmark settings.
            int width, height, framerate;
            NatCamWithOpenCVForUnityExample.CameraConfiguration (out width, out height, out framerate);
            NatCamWithOpenCVForUnityExample.ExampleSceneConfiguration (out performImageProcessingEachTime);
            // Create camera source
            cameraSource = new WebCamSource (width, height, framerate, useFrontCamera);
            cameraSource.StartPreview (OnStart, OnFrame);
            // Update UI
            imageProcessingTypeDropdown.value = (int)imageProcessingType;

            fpsMonitor = GetComponent<FpsMonitor> ();
            if (fpsMonitor != null) {
                fpsMonitor.Add ("Name", "WebCamTextureOnlyExample");
                fpsMonitor.Add ("performImageProcessingEveryTime", performImageProcessingEachTime.ToString ());
                fpsMonitor.Add ("onFrameFPS", onFrameFPS.ToString ("F1"));
                fpsMonitor.Add ("drawFPS", drawFPS.ToString ("F1"));
                fpsMonitor.Add ("width", "");
                fpsMonitor.Add ("height", "");
                fpsMonitor.Add ("orientation", "");
            }
        }

        protected override void OnStart ()
        {
            // Create pixel buffer
            pixelBuffer = new Color32[cameraSource.width * cameraSource.height];
            // Create texture
            if (texture != null)
                Texture2D.Destroy (texture);     
            texture = new Texture2D (
                cameraSource.width,
                cameraSource.height,
                TextureFormat.RGBA32,
                false,
                false
            );
            // Display texture
            rawImage.texture = texture;
            aspectFitter.aspectRatio = cameraSource.width / (float)cameraSource.height;
            Debug.Log ("WebCam camera source started with resolution: " + cameraSource.width + "x" + cameraSource.height);

            if (fpsMonitor != null) {
                fpsMonitor.Add ("width", cameraSource.width.ToString ());
                fpsMonitor.Add ("height", cameraSource.height.ToString ());
                fpsMonitor.Add ("orientation", Screen.orientation.ToString ());
            }
        }

        protected override void Update ()
        {
            base.Update ();
        
            if (updateCount == 0) {
                if (fpsMonitor != null) {
                    fpsMonitor.Add ("onFrameFPS", onFrameFPS.ToString ("F1"));
                    fpsMonitor.Add ("drawFPS", drawFPS.ToString ("F1"));
                    fpsMonitor.Add ("orientation", Screen.orientation.ToString ());
                }
            }
        }

        protected override void UpdateTexture ()
        {
            cameraSource.CaptureFrame (pixelBuffer);
            ProcessImage (pixelBuffer, texture.width, texture.height, imageProcessingType);
            texture.SetPixels32 (pixelBuffer);
            texture.Apply ();
        }

        protected override void OnDestroy ()
        {
            if (cameraSource != null) {
                cameraSource.Dispose ();
                cameraSource = null;
            }
            Texture2D.Destroy (texture);
            texture = null;
            pixelBuffer = null;
        }
    }
}