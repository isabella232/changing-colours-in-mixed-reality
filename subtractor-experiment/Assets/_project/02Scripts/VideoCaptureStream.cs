// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;

namespace MagicLeap
{
    /// <summary>
    /// This class handles video recording and loading based on controller
    /// input.
    /// </summary>
    [RequireComponent(typeof(PrivilegeRequester))]
    public class VideoCaptureStream : MonoBehaviour
    {
        [System.Serializable]
        private class VideoCaptureEvent : UnityEvent<string>
        {}

        #region Private Variables
        [SerializeField, Tooltip("The maximum amount of time the camera can be recording for (in seconds.)")]
        private float _maxRecordingTime = 10.0f;

        [SerializeField, Tooltip("The Raw Image surfaces where the video should be shown")]
        private List<RawImage> _rawImage = new List<RawImage>();

        [SerializeField, Tooltip("The material where the video should be shown")]
        private Material _material;


        [SerializeField, Tooltip("The maximum amount of time the camera can be recording for (in seconds.)")]
        private Text _text;

        [Space, SerializeField, Tooltip("ControllerConnectionHandler reference.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        [Space, Header("Events")]
        [SerializeField, Tooltip("Event called when recording starts")]
        private UnityEvent OnVideoCaptureStarted = null;

        [SerializeField, Tooltip("Event called when recording stops")]
        private VideoCaptureEvent OnVideoCaptureEnded = null;

        private const string _validFileFormat = ".mp4";

        private const float _minRecordingTime = 1.0f;

        // Is the camera currently recording
        private bool _isCapturing;

        // The file path to the active capture
        private string _captureFilePath;

        private bool _isCameraConnected = false;

        private float _captureStartTime;

        private PrivilegeRequester _privilegeRequester = null;

        private bool _hasStarted = false;

        private bool _appPaused = false;

        private Texture2D _texture;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Validate that _maxRecordingTime is not less than minimum possible.
        /// </summary>
        void OnValidate()
        {
            if (_maxRecordingTime < _minRecordingTime)
            {
                Debug.LogWarning(string.Format("You can not have a MaxRecordingTime less than {0}, setting back to minimum allowed!", _minRecordingTime));
                _maxRecordingTime = _minRecordingTime;
            }
        }

        // Using Awake so that Privileges is set before PrivilegeRequester Start
        void Awake()
        {
            _texture = new Texture2D(1920, 1080);
            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: VideoCamptureExample._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }

            // If not listed here, the PrivilegeRequester assumes the request for
            // the privileges needed, CameraCapture and AudioCaptureMic in this case, are in the editor.
            _privilegeRequester = GetComponent<PrivilegeRequester>();

            // Before enabling the Camera, the scene must wait until the privileges have been granted.
            _privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
        }

        void Update()
        {
           if (_isCapturing)
           {
                // _rawImage.texture = _texture;
                // If the recording has gone longer than the max time
                if (Time.time - _captureStartTime > _maxRecordingTime)
                {
                    EndCapture();
                }
            }
        }

        /// <summary>
        /// Stop the camera, unregister callbacks, and stop input and privileges APIs.
        /// </summary>
        void OnDisable()
        {
            // MLInput.OnControllerButtonDown -= OnButtonDown;

            if (_isCameraConnected)
            {
                DisableMLCamera();
            }
        }

        /// <summary>
        /// Cannot make the assumption that a privilege is still granted after
        /// returning from pause. Return the application to the state where it
        /// requests privileges needed and clear out the list of already granted
        /// privileges. Also, disable the camera and unregister callbacks.
        /// </summary>
        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                _appPaused = true;

                if (_isCameraConnected && MLCamera.IsStarted)
                {
                    MLResult result = MLCamera.ApplicationPause(_appPaused);
                    if(!result.IsOk)
                    {
                        Debug.LogErrorFormat("Error: VideoCaptureExample failed to pause MLCamera, disabling script. Reason: {0}", result);
                        enabled = false;
                        return;
                    }

                    if (_isCapturing)
                    {
                        OnVideoCaptureEnded.Invoke(_captureFilePath);
                        _captureFilePath = null;
                    }

                    _isCapturing = false;
                    _captureStartTime = 0;
                    _isCameraConnected = false;
                }

                //MLInput.OnControllerButtonDown -= OnButtonDown;
            }
        }

        void OnDestroy()
        {
            if (_privilegeRequester != null)
            {
                _privilegeRequester.OnPrivilegesDone -= HandlePrivilegesDone;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start capturing video.
        /// </summary>
        public void StartCapture()
        {
            _text.text = "StartPreview called";
            if(!_isCapturing && MLCamera.IsStarted && _isCameraConnected)
            {
                // MLCamera.Settings.Apply();
                MLResult result = MLCamera.StartPreview(_texture);
                if (result.Code == MLResultCode.Ok)
                {
                    _text.text = "StartPreview is OK";
                    // _material.mainTexture = _texture;
                    for (int i = 0; i < _rawImage.Count; i++) {
                        _rawImage[i].texture = _texture;
                    }
                    _isCapturing = true;
                    _captureStartTime = Time.time;
                    OnVideoCaptureStarted.Invoke();
                }
                else
                {
                    _text.text = "StartPreview is NOT OK";
                    if (result.Code == MLResultCode.InvalidParam) {
                        _text.text = "Failed due to an invalid input parameter";
                    }
                    if (result.Code == MLResultCode.Pending) {
                        _text.text = "PCF system isn't fully initialized";
                    }
                    if (result.Code == MLResultCode.UnspecifiedFailure) {
                        _text.text = "Other internal error";
                    }
                    if (result.Code == MLResultCode.SnapshotPoseNotFound) {
                        _text.text = "Coordinate Frame is valid, but not found";
                    }
                    if (result.Code == MLResultCode.PrivilegeDenied)
                    {
                        _text.text = "Privilege denied";
                        Instantiate(Resources.Load("PrivilegeDeniedError"));
                    }
                    Debug.LogErrorFormat("Error: VideoCaptureExample failed to start video capture for {0}. Reason: {1}", MLCamera.GetErrorCode().ToString());
                }
            }
            else
            {
                _text.text = "StartPreview failed";
                Debug.LogErrorFormat("Error: VideoCaptureExample failed to start video capture for {0} because '{1}' is already recording!");
            }
        }

        /// <summary>
        /// Stop capturing video.
        /// </summary>
        public void EndCapture()
        {
            if (_isCapturing)
            {
                MLResult result = MLCamera.StopPreview();
                if (result.IsOk)
                {
                    _isCapturing = false;
                    _captureStartTime = 0;
                    OnVideoCaptureEnded.Invoke(_captureFilePath);
                }
                else
                {
                    if (result.Code == MLResultCode.PrivilegeDenied)
                    {
                        Instantiate(Resources.Load("PrivilegeDeniedError"));
                    }

                    Debug.LogErrorFormat("Error: VideoCaptureExample failed to end video capture. Error Code: {0}", MLCamera.GetErrorCode().ToString());
                }
            }
            else
            {
                Debug.LogError("Error: VideoCaptureExample failed to end video capture because the camera is not recording.");
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Connects the MLCamera component and instantiates a new instance
        /// if it was never created.
        /// </summary>
        private void EnableMLCamera()
        {
            MLResult result = MLCamera.Start();
            if (result.IsOk)
            {
                result = MLCamera.Connect();
                _isCameraConnected = true;
                StartCapture();
            }
            else
            {
                if (result.Code == MLResultCode.PrivilegeDenied)
                {
                    Instantiate(Resources.Load("PrivilegeDeniedError"));
                }

                Debug.LogErrorFormat("Error: VideoCaptureExample failed starting MLCamera, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Disconnects the MLCamera if it was ever created or connected.
        /// Also stops any video recording if active.
        /// </summary>
        private void DisableMLCamera()
        {
            if (MLCamera.IsStarted)
            {
                if (_isCapturing)
                {
                    EndCapture();
                }
                MLCamera.Disconnect();
                _isCameraConnected = false;
                MLCamera.Stop();
            }
        }

        /// <summary>
        /// Enable the camera and callbacks. Called once privileges have been granted.
        /// </summary>
        private void EnableCapture()
        {
            if (!_hasStarted)
            {
                EnableMLCamera();

                //MLInput.OnControllerButtonDown += OnButtonDown;

                _hasStarted = true;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Responds to privilege requester result.
        /// </summary>
        /// <param name="result"/>
        private void HandlePrivilegesDone(MLResult result)
        {
            if (!result.IsOk)
            {
                if (result.Code == MLResultCode.PrivilegeDenied)
                {
                    Instantiate(Resources.Load("PrivilegeDeniedError"));
                }

                Debug.LogErrorFormat("Error: VideoCaptureExample failed to get all requested privileges, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }

            Debug.Log("Succeeded in requesting all privileges");

            // Called here because it needs privileges to be granted first on resume by PrivilegeRequester.
            if (_appPaused)
            {
                _appPaused = false;

                result = MLCamera.ApplicationPause(_appPaused);
                if (!result.IsOk)
                {
                    Debug.LogErrorFormat("Error: VideoCaptureExample failed to resume MLCamera, disabling script. Reason: {0}", result);
                    enabled = false;
                    return;
                }

                _isCameraConnected = true;

                // MLInput.OnControllerButtonDown += OnButtonDown;
            }
            else
            {
                EnableCapture();
            }
        }

        /// <summary>
        /// Handles the event for button down. Starts or stops recording.
        /// </summary>
        /// <param name="controllerId">The id of the controller.</param>
        /// <param name="button">The button that is being pressed.</param>
        /* private void OnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            _text.text = "Button down";
            if (_controllerConnectionHandler.IsControllerValid(controllerId) && MLInputControllerButton.Bumper == button)
            {
                _text.text = "Bumper down";
                if (!_isCapturing)
                {
                    StartCapture();
                }
                else if(_isCapturing && Time.time - _captureStartTime > _minRecordingTime)
                {
                    EndCapture();
                }
            }
        } */
        #endregion
    }
}
