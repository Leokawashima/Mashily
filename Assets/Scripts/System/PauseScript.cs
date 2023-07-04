using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PauseScript : MonoBehaviour
{
	[SerializeField]	
	private GameObject pauseUIPrefab;
	private GameObject pauseUIInstance;

	[SerializeField]
	private MultifunctionFollowingCamera cam;

	private Slider sliderSensitivityY;
	private Slider sliderSensitivityX;
	private Toggle padtoggle;

	public bool padmode = false;

	private static float sensiY;
	private static float sensiX;

	private bool releaseKey = true;

	void Update()
	{
		string padName = padmode ? Name.Input.PausePad : Name.Input.Pause;
		if(Input.GetAxisRaw(padName) == 0f) 
			releaseKey = true;

		if (pauseUIInstance != null)
			padmode = padtoggle.isOn;

		if (releaseKey)
		if(Input.GetAxisRaw(padName) > 0f)
		{
			releaseKey = false;

			if(pauseUIInstance == null)
			{
				pauseUIInstance = GameObject.Instantiate(pauseUIPrefab) as GameObject;

				GameObject slidY = GameObject.Find("SliderY");
				GameObject slidX = GameObject.Find("SliderX");
				GameObject tog = GameObject.Find("PadChange");

				sliderSensitivityY = slidY.GetComponent<Slider>();
				sliderSensitivityY.value = sensiY == 0f ? cam.inputSpeedY : sensiY;
				sliderSensitivityX = slidX.GetComponent<Slider>();
				sliderSensitivityX.value = sensiX == 0f ? cam.inputSpeedX : sensiX;
				padtoggle = tog.GetComponent<Toggle>();
				padtoggle.isOn = padmode;
				Time.timeScale = 0f;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.Confined;
			}
			else
			{
				cam.inputSpeedY = sliderSensitivityY.value;
				cam.inputSpeedX = sliderSensitivityX.value;
				sensiY = cam.inputSpeedY;
				sensiX = cam.inputSpeedX;
				padmode = padtoggle.isOn;
				Destroy(pauseUIInstance);
				Time.timeScale = 1f;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}
}
