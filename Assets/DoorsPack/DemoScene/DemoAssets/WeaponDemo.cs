using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class WeaponDemo : MonoBehaviour {
	private RaycastHit hit;
	public GameObject sight;
	private bool CursorLocked;
	private EventSystem eventSys;
	// Use this for initialization
	void Start () {
		sight.SetActive (true);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		CursorLocked = true;
		eventSys = GameObject.Find ("EventSystem").GetComponent<EventSystem> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("f") || Input.GetMouseButtonDown(0) && !CursorLocked && !eventSys.IsPointerOverGameObject()) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			CursorLocked = true;
		}
		else if(Input.GetKeyDown("f") || Input.GetKeyDown("escape") && CursorLocked){
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			CursorLocked = false;
		}

		if (Input.GetKeyDown (KeyCode.Alpha1) && sight != null) {
			if(!sight.activeInHierarchy){
				sight.SetActive(true);
			}
			else{
				sight.SetActive(false);
			}
		}
	if(Input.GetMouseButtonDown(0)){
			if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) * 20, out hit)){
				if(hit.transform.gameObject.tag == "Lock"){
					LockScript lockScript = hit.transform.gameObject.GetComponent<LockScript>();
					lockScript.LockHit();
				}

			}
		}
	}
}
