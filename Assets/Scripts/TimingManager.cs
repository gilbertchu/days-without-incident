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
	public Text daysValue;
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
		daysValue.text = GetDaysDebug ().ToString ();
	}

	double GetDaysDebug(){
		//Debug value of get days
		TimeSpan elapsed = DateTime.UtcNow - startTime;
		double days = Math.Round (elapsed.TotalDays, 5);
		return days;
	}

	void SetTime(int days = 0){
		//Set the starting time with initial days value
		startTime = DateTime.UtcNow;
		if (days != 0)
			startTime = startTime.AddDays (Convert.ToDouble (-days));
		Save ();
	}

	int GetDays(){
		//Return number of days elapsed since start time
		TimeSpan elapsed = DateTime.UtcNow - startTime;
		return elapsed.Days;
	}

	void Save(){
		//Save data to file
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/persistentData.dat");
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
		resetWindow.SetActive (true);
	}

	public void ConfirmReset(bool confirm){
		//Confirm or cancel reset
		if (confirm) {
			int days;
			int.TryParse (resetDays.text, out days);
			Debug.Log ("Days: " + days.ToString ());
			SetTime (days);
		}
		resetWindow.SetActive (false);
	}
}

[Serializable]
class GameData
{
	public DateTime startTime;
}