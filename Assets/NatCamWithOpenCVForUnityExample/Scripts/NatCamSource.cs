using UnityEngine;
using System;
using OpenCVForUnity;
using NatCamU.Core;

namespace NatCamWithOpenCVForUnityExample {

    public class NatCamSource : ICameraSource {

        private DeviceCamera camera;
        private Action startCallback, frameCallback;
        private byte[] sourceBuffer;
        
        #region --Client API--

        public Texture Preview { get { return NatCam.Preview; } }
        public DeviceCamera ActiveCamera { get { return NatCam.Camera; }}

        public NatCamSource (int width, int height, int framerate, bool front) {
            camera = front ? DeviceCamera.FrontCamera : DeviceCamera.RearCamera;
            camera.PreviewResolution = new Vector2Int(width, height);
            camera.Framerate = framerate;
        }

        public void Dispose () {
            NatCam.StopPreview();
            sourceBuffer = null;
        }

        public void StartPreview (Action startCallback, Action frameCallback) {
            this.startCallback = startCallback;
            this.frameCallback = frameCallback;
            NatCam.StartPreview(camera, OnStart, frameCallback);
        }

        public void CaptureFrame (Mat matrix) {
            NatCam.CaptureFrame(sourceBuffer);
            Utils.copyToMat(sourceBuffer, matrix);
            Core.flip(matrix, matrix, 0);
        }

        public void CaptureFrame (Color32[] pixelBuffer) {
            NatCam.CaptureFrame(sourceBuffer);
            Buffer.BlockCopy(sourceBuffer, 0, pixelBuffer, 0, sourceBuffer.Length);
        }

        public void SwitchCamera () {
            camera = NatCam.Camera.IsFrontFacing ? DeviceCamera.RearCamera : DeviceCamera.FrontCamera;
            NatCam.StartPreview(camera, OnStart, frameCallback);
        }
        #endregion


        #region --Operations--

        private void OnStart () {
            sourceBuffer = new byte[Preview.width * Preview.height * 4];
            startCallback();
        }
        #endregion
    }
}