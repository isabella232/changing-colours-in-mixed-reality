using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.MagicLeap;

public class OscReceiver : MonoBehaviour
{

		 /*
    private TextMeshProUGUI oscText;
		
    private OscIn _oscIn;
		const string address = "/biodata";
    bool messageReceived = false;
    private PrivilegeRequester _privilegeRequester = null;
		private HueController hueController;
		public Vector2 minMaxPulse = new Vector2(40f, 80f);

		
    void Start() {
			_oscIn = transform.GetChild(0).GetComponent<OscIn>();
			oscText = GameObject.Find("Text (TMP) IP").GetComponent<TextMeshProUGUI>();
			hueController = GameObject.Find("HueController").GetComponent<HueController>();
      // If not listed here, the PrivilegeRequester assumes the request for
      // the privileges needed, CameraCapture and AudioCaptureMic in this case, are in the editor.
      _privilegeRequester = GetComponent<PrivilegeRequester>();

      // Before enabling the Camera, the scene must wait until the privileges have been granted.
      _privilegeRequester.OnPrivilegesDone += HandlePrivilegesDone;
    }

		
		private IEnumerator TestOpacity() {
			yield return new WaitForSeconds(1f);
			int number = (int) Random.Range(minMaxPulse.x - 5, minMaxPulse.y + 5);
			ReceiveInt(number);
			StartCoroutine(TestOpacity());
		}
    private void HandlePrivilegesDone(MLResult result) {
      if (!result.IsOk){
          if (result.Code == MLResultCode.PrivilegeDenied)
          {
              Instantiate(Resources.Load("PrivilegeDeniedError"));
          }

          Debug.LogErrorFormat("Error: OscReceiver failed to get all requested privileges, disabling script. Reason: {0}", result);
          return;
      }

      Debug.Log("Succeeded in requesting all privileges");
      _oscIn.gameObject.SetActive(true);
    }

    void Update()
    {
      if (messageReceived) {
        return;
      }
      if( OscIn.localIpAddress != oscText.text ){
        if( string.IsNullOrEmpty( OscIn.localIpAddress ) ) {
          oscText.text = "Local IP Not found";
        }
        else {
          oscText.text = OscIn.localIpAddress;
        }
      }
    }

    public void ReceiveInt( int value )
		{
      messageReceived = true;
			oscText.text = value.ToString();
			float pulse = (float) value;
			if (pulse < minMaxPulse.x) {
				pulse = minMaxPulse.x;
			}
			else if (pulse > minMaxPulse.y) {
				pulse = minMaxPulse.y;
			}
			pulse -= minMaxPulse.x;
			pulse /= (minMaxPulse.y - minMaxPulse.x);
			hueController.TweenOpacity(pulse);
		}
		*/
}
