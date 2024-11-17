using UnityEngine;

namespace Core
{
	public class MouseWorldPosition : MonoBehaviour
	{
		private Camera _main;

		public static MouseWorldPosition Instance { get; private set; }

		private void Awake()
		{
			_main = Camera.main;
			Instance = this;
		}

		public Vector3 GetPosition()
		{
			Ray ray = _main.ScreenPointToRay(Input.mousePosition);
			Plane plane = new(Vector3.up, Vector3.zero);

			if(plane.Raycast(ray, out float distance))
				return ray.GetPoint(distance);

			return default;
		}
	}
}