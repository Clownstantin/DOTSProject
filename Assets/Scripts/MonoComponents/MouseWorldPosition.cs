using UnityEngine;

namespace Core
{
	public class MouseWorldPosition : MonoBehaviour
	{
		private Camera _mainCamera;

		public static MouseWorldPosition Instance { get; private set; }

		public Camera MainCamera => _mainCamera;

		private void Awake()
		{
			_mainCamera = Camera.main;
			Instance = this;
		}

		public Vector3 GetPosition()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
			Plane plane = new(Vector3.up, Vector3.zero);

			if(plane.Raycast(ray, out float distance))
				return ray.GetPoint(distance);

			return default;
		}
	}
}