// Fetch hand position from server, transform to (Orthgraphical)camera-space coordinate
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using MiniJSON;

public class FetchHandData : MonoBehaviour
{
    public float frequency = 10f;
    public Vector3 handPos;
    public int handType;
    private string URI = "http://localhost:8000/hand";
    private bool isDelay;
    private float height = 1920f;
    private float width = 1080f;

    void Start()
    {
    }

    void Update()
    {
        if(!isDelay)
        {
            isDelay = true;
            StartCoroutine(GetRequest(this.URI));
        }
    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                //Debug.Log("Get Request Failure");
                //Debug.Log(request.error);
            }
            else
            {
                // Debug.Log("Get Request Success");
                List<object> handCoord = (List<object>) (Json.Deserialize(request.downloadHandler.text) as Dictionary<string, object>)["hand_coor"];
                handPos.x = (float)Convert.ToDouble(handCoord[0]);
                handPos.y = (float)Convert.ToDouble(handCoord[1]);
                resize();
                handPos = Camera.main.ScreenToWorldPoint(new Vector3(handPos.x, handPos.y, 20));

                HandData data = JsonUtility.FromJson<HandData>(request.downloadHandler.text);
                // Debug.Log(data.hand_type);
                handType = data.hand_type;

            }
        }
        yield return new WaitForSeconds(1/frequency);
        isDelay = false;
    }
    
    private void resize() {
        handPos.x = height * (1-handPos.x);
        handPos.y = width * (1-handPos.y);
        // Debug.Log("resized handPos x = " + handPos.x + " y = " + handPos.y);
    }

    [Serializable]
    public class HandData
    {
        public float[] hand_coor;
        public int hand_type;
    }

}