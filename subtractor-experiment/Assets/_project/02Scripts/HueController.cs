using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using TMPro;
using DigitalRuby.Tween;

public class HueController : MonoBehaviour
{
    #region Private Variables
    [SerializeField, Tooltip("A reference to the controller connection handler in the scene.")]
    private ControllerConnectionHandler _controllerConnectionHandler = null;
    #endregion

    public Canvas backButton;
    public Canvas selectionButton;
    public Canvas hueSatButton;

    public RawImage hueReference;
    public RawImage hueResult;

    public Material videoMaterial;

    bool firstTouch = true;
    float firstTouchX = 0f;
    float firstTouchY = 0f;
    float differenceX = 0f;
    float differenceY = 0f;

    Vector4 startHsla = new Vector4();
    Vector4 hsla = new Vector4();
    float startTarget = 0f;
    float startSpread = 0f;
    float target = 0f;
    float spread = 0f;

    float opacity = 0f;

    string state = "root";

    public Object obj;

    void Awake()
    {
        MLInput.OnControllerButtonDown += HandleOnButtonDown;
    }

    void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= HandleOnButtonDown;
    }

    void Start()
    {
        Debug.Log(obj);
        startHsla = videoMaterial.GetVector("_HSLAAdjust");
        hueResult.material.SetFloat("_Hue", startHsla.x);
        hueResult.material.SetFloat("_Saturation", startHsla.y);

        startTarget = videoMaterial.GetFloat("_Target");
        startSpread = videoMaterial.GetFloat("_Spread");
        hueResult.material.SetFloat("_Target", startTarget);
        hueResult.material.SetFloat("_Spread", startSpread);
        hueReference.material.SetFloat("_Target", startTarget);
    }

    Vector4 tempHsla = new Vector4();
    public void TweenOpacity(float to) {
        tempHsla = videoMaterial.GetVector("_HSLAAdjust");
        System.Action<ITween<float>> SetOpacity = (t) => {
            tempHsla = videoMaterial.GetVector("_HSLAAdjust");
            tempHsla.w = t.CurrentValue;
            videoMaterial.SetVector("_HSLAAdjust", tempHsla);
        };
        TweenFactory.Tween(opacity, tempHsla.w, to, 1.3f, TweenScaleFunctions.CubicEaseOut, SetOpacity);
    }

    // Update is called once per frame
    void Update()
    {
        MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (controller.Touch1Active)
        {
            float x = controller.Touch1PosAndForce.x;
            float y = controller.Touch1PosAndForce.y;
            if (firstTouch) {
                firstTouchX = x;
                firstTouchY = y;
            }

            differenceX = x - firstTouchX;
            differenceY = y - firstTouchY;

            if (state == "root") {
                if (x < -0.3f) {
                    state = "selection";
                    startTarget = videoMaterial.GetFloat("_Target");
                    startSpread = videoMaterial.GetFloat("_Spread");
                    UpdateSelection();
                    ToggleMenu(false);
                }
                else if (x > 0.3f) {
                    state = "hueSat";
                    startHsla = videoMaterial.GetVector("_HSLAAdjust");
                    hsla = startHsla;
                    ToggleMenu(false);
                }
            }

            else if (state == "selection") {
                if (firstTouch) {
                    startTarget = videoMaterial.GetFloat("_Target");
                    target = startTarget;
                    startSpread = videoMaterial.GetFloat("_Spread");
                    spread = startSpread;
                    UpdateSelection();
                }
                target = startTarget + differenceX / 10f;
                if (target < -0.5f) {
                    target += 1.0f;
                }
                else if (target > 0.5f) {
                    target -= 1.0f;
                }

                spread = startSpread + differenceY / 10f;
                if (spread < 0f) {
                    spread = 0f;
                }
                else if (spread > 1f) {
                    spread = 1f;
                }

                UpdateSelection();
            }

            else if (state == "hueSat") {
                if (firstTouch) {
                    startHsla = videoMaterial.GetVector("_HSLAAdjust");
                    hsla = startHsla;
                }
                hsla.x = startHsla.x + differenceX / 10f;
                if (hsla.x > 1f) {
                    hsla.x -= 1f;
                }
                else if (hsla.x < 0f) {
                    hsla.x += 1f;
                }
                hsla.y = startHsla.y + differenceY / 4f;
                if (hsla.y > 1f) {
                    hsla.y = 1f;
                }
                else if (hsla.y < -1f) {
                    hsla.y = -1f;
                }
                videoMaterial.SetVector("_HSLAAdjust", hsla);
                hueResult.material.SetFloat("_Hue", hsla.x);
                hueResult.material.SetFloat("_Saturation", hsla.y);
            }

            
            // Get angle of touchpad position.
            float angle = -Vector2.SignedAngle(Vector2.up, controller.Touch1PosAndForce);
            if (angle < 0.0f)
            {
                angle += 360.0f;
            }

            firstTouch = false;
        }
        else {
            firstTouchX = 0f;
            firstTouchY = 0f;
            firstTouch = true;
        }
    }
    void UpdateSelection() {
        videoMaterial.SetFloat("_Target", target);
        videoMaterial.SetFloat("_Spread", spread);
        hueResult.material.SetFloat("_Target", target);
        hueResult.material.SetFloat("_Spread", spread);
        hueReference.material.SetFloat("_Target", target);
    }
    
    #region Event Handlers
    private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        if (_controllerConnectionHandler.IsControllerValid(controllerId))
        {
            if (button == MLInputControllerButton.HomeTap)
            {
                if (state != "root") {
                    state = "root";
                    ToggleMenu(true);
                }
            }
            if (button == MLInputControllerButton.Bumper)
            {
                float currentVideoBool = videoMaterial.GetFloat("_VideoBool");
                if (currentVideoBool == 1.0f) {
                    videoMaterial.SetFloat("_VideoBool", 0.0f);
                }
                else {
                    videoMaterial.SetFloat("_VideoBool", 1.0f);
                }
            }
        }
    }

    void ToggleMenu (bool on) {
        backButton.enabled = !on;
        selectionButton.enabled = on;
        hueSatButton.enabled = on;
    }
    #endregion
}
