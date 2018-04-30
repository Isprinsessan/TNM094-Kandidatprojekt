using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;


public class PET : MonoBehaviour {
    public bool mPersistActivated;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!mPersistActivated)
        {
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.PersistExtendedTracking(true);
            mPersistActivated = true;
        }
    }
}
