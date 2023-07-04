using UnityEngine;
using UnityEngine.UI;

public class UIFollowTarget : MonoBehaviour
{
	private RectTransform rectT;
	[SerializeField] 
	private Transform target;
	[SerializeField]
	private Vector2 offset;

	void Awake()
	{
		rectT = GetComponent<RectTransform>();
	}

	void Update()
	{
		rectT.position = 
			RectTransformUtility.WorldToScreenPoint(Camera.main, target.position)
			+ offset;
	}
}