using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscSender : MonoBehaviour
{
    /*
    OscOut _oscOut;

    OscMessage _message; // Cached message.

    const string address = "/biodata";

    private int port = 7000;
    public string ip = "192.168.1.101";

    public int testPulse = 40;
    Vector2 minMaxPulse = new Vector2(40, 80);

    void Awake()
    {
        _oscOut = GetComponent<OscOut>();
        _oscOut.Open( port, ip );
    }
    void Start() {
        StartCoroutine(TestSend());
    }

    private IEnumerator TestSend() {
        yield return new WaitForSeconds(1f);
        int number = (int) Random.Range(minMaxPulse.x - 5, minMaxPulse.y + 5);
        _oscOut.Send( address, number );
        StartCoroutine(TestSend());
    }
    */
}
