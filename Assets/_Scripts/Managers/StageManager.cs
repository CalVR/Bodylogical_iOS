﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collections.Hybrid.Generic;

public class StageManager : MonoBehaviour {

    public static StageManager Instance;
    public GameObject stage;
    public GameObject controlPanel;
    public GameObject[] props;

    public Transform[] positionList;

    public static string[] name_array = { "Bob", "Alice", "Cecelia", "Donald", "Emily" };
    public static string[] health_condition = {"Good", "In Danger","Average","Not Good", "Good"};
    public static string[] sex_array = {"male","female", "female","make", "female"};
    private LinkedListDictionary<Transform, bool> posAvailableMap;
    private Color futureBlue;
    private Color colorWhite;
    private Vector3 cp_initial_localPos;
    private bool isAnimating;

    private int propsIterator = 0;

    private void Awake()
    {
        if (Instance == null){
            Instance = this;
        }

        cp_initial_localPos = new Vector3(controlPanel.transform.localPosition.x, controlPanel.transform.localPosition.y, controlPanel.transform.localPosition.z);
        isAnimating = false;
    }

    public Transform GetAvailablePosInWorld(){

        foreach (Transform trans in positionList){
            if (posAvailableMap[trans]){
                posAvailableMap[trans] = false;
                return trans;
            }
        }
        return null;
    }

    public void toggleProps()
    {
        if (propsIterator == 0){
            HumanManager.Instance.getSelectedHuman().SetActive(false);
        }
        else{
            props[propsIterator - 1].SetActive(false);
        }

        propsIterator += 1;

        if(propsIterator > 3){
            propsIterator = 0;

            HumanManager.Instance.getSelectedHuman().SetActive(true);
        }
        else{
            props[propsIterator - 1].SetActive(true);
        }
    }


    public void BuildStage(){
        for (int i = 0; i < positionList.Length; i++){
            string profile_name = "Profile " + i;
            string model_name = name_array[i];
            string health = health_condition[i];

            HumanManager.Instance.CreateArchitype(profile_name, Name: model_name, health_cond: health);
        }
    }

    public void UpdateStageTransform(){

        if (!stage.activeSelf){
            stage.SetActive(true);
        }

        if (!stage.GetComponent<MeshRenderer>().enabled){
            stage.GetComponent<MeshRenderer>().enabled = true;
        }

        if (CursorManager.Instance.cursor.GetCurrentFocusedObj() != null){

            GameObject obj = CursorManager.Instance.cursor.GetCurrentFocusedObj();

            // if this is a plane
            if (obj.GetComponent<PlaneInteract>()!=null){
                Vector3 cursorPos = CursorManager.Instance.cursor.GetCursorPosition();
                Vector3 stageCenter = stage.transform.GetChild(0).position;
                Vector3 diff = stage.transform.position - stageCenter;
                stage.transform.position = cursorPos + diff;
                AdjustStageRotation(PlaneManager.Instance.GetMainPlane());
            }
        }

        Color lerpedColor = Color.Lerp(colorWhite, futureBlue, Mathf.PingPong(Time.time, 1));

        stage.GetComponent<MeshRenderer>().material.color = lerpedColor;
    }

    public void DisableStage(){
        stage.SetActive(false);
    }

    public void SettleStage(){
        stage.GetComponent<MeshRenderer>().enabled = false;
    }

    public void EnableControlPanel(){
        if(!isAnimating){
            StartCoroutine(FadeUpCP());
        }
    }

    public void DisableControlPanel(){
        controlPanel.SetActive(false);
    }


    IEnumerator FadeUpCP(){

        controlPanel.SetActive(true);

        float animation_time = 2.5f;
        float time_passed = 0;
        isAnimating = true;

        controlPanel.transform.localPosition = new Vector3(controlPanel.transform.localPosition.x, -10f, controlPanel.transform.localPosition.z);

        while(time_passed < animation_time){
        
            controlPanel.transform.localPosition = Vector3.Lerp(controlPanel.transform.localPosition, cp_initial_localPos, 0.03f);

            time_passed += Time.deltaTime;
            yield return null;
        }

        controlPanel.transform.localPosition = cp_initial_localPos;

        isAnimating = false;

        yield return null;
    }

    // Use this for initialization
    void Start () {

        posAvailableMap = new LinkedListDictionary<Transform, bool>();

        foreach (Transform trans in positionList){
            posAvailableMap.Add(trans, true);
        }

        futureBlue = stage.transform.GetComponent<MeshRenderer>().material.color;
        colorWhite = new Color(0, 1, 1, 0.42f);

        DisableControlPanel();
        DisableStage();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void AdjustStageRotation(GameObject plane){
        stage.transform.rotation = plane.transform.rotation;

        while(Vector3.Dot((stage.transform.position-Camera.main.transform.position), stage.transform.forward) < 0){
            stage.transform.Rotate(0, 90, 0);
        }
        
    }
}
