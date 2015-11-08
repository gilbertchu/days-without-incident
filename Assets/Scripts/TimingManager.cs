using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class TimingManager : MonoBehaviour {
	
	public GameObject resetWindow;
	public InputField resetField;
	public Text resetDays;
	public Toggle useToday;
	public Text daysValue;
	public Toggle useDebugValue;
	DateTime startTime;

	void Start () {
		//Load starting time or prompt for new starting time
		if (!Load ()) {
			Debug.Log("No data found");
			DisplayResetWindow();
		}
	}

	void Update() {
		//Update days value
		daysValue.text = (useDebugValue.isOn) ? GetDaysDebug ().ToString () : GetDays ().ToString ();
	}

	double GetDaysDebug(){
		//Debug value of get days
		TimeSpan elapsed = DateTime.Now - startTime;
		double days = Math.Round (elapsed.TotalDays, 5);
		return days;
	}

	void SetTime(int days = 0, bool today = false){
		//Set the starting time with initial days value
		startTime = (today) ? DateTime.Today : DateTime.Now;
		Debug.Log (startTime.ToString ());
		if (days != 0)
			startTime = startTime.AddDays (Convert.ToDouble (-days));
		Save ();
	}

	int GetDays(){
		//Return number of days elapsed since start time
		TimeSpan elapsed = DateTime.Now - startTime;
		return elapsed.Days;
	}

	void Save(){
		//Save data to file
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerData.dat");
		GameData data = new GameData ();
		data.startTime = startTime;
		bf.Serialize (file, data);
		file.Close ();
	}

	bool Load(){
		//Load data from file
		bool hasData = File.Exists (Application.persistentDataPath + "/playerData.dat");
		if (hasData) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerData.dat", FileMode.Open);
			GameData data = (GameData)bf.Deserialize (file);
			file.Close ();
			startTime = data.startTime;
		} else {
			SetTime();
		}
		return hasData;
	}

	public void DisplayResetWindow(){
		//Pop up reset time window
		resetField.text = "0";
		useToday.isOn = true;
		resetWindow.SetActive (true);
	}

	public void ConfirmReset(bool confirm){
		//Confirm or cancel reset
		if (confirm) {
			int days;
			int.TryParse (resetDays.text, out days);
			Debug.Log ("Days: " + days.ToString ());
			SetTime (days, useToday.isOn);
		}
		resetWindow.SetActive (false);
	}
}

[Serializable]
class GameData
{
	public DateTime startTime;
}