using UnityEngine;

namespace Core
{
	public class UnitSelectionUI : MonoBehaviour
	{
		[SerializeField] private RectTransform _selectionAreaRT;
		[SerializeField] private Canvas _canvas;

		private void Start()
		{
			_selectionAreaRT.gameObject.SetActive(false);
			UnitSelectionManager.Instance.OnSelectionStart += OnSelectionStart;
			UnitSelectionManager.Instance.OnSelectionEnd += OnSelectionEnd;
		}

		private void Update()
		{
			if(_selectionAreaRT.gameObject.activeSelf)
				UpdateSelectionView();
		}

		private void OnSelectionStart(object sender, System.EventArgs e)
		{
			_selectionAreaRT.gameObject.SetActive(true);
			UpdateSelectionView();
		}

		private void OnSelectionEnd(object sender, System.EventArgs e) => _selectionAreaRT.gameObject.SetActive(false);

		private void UpdateSelectionView()
		{
			Rect selectionRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
			float canvasScale = _canvas.transform.localScale.x;

			_selectionAreaRT.anchoredPosition = new Vector2(selectionRect.x, selectionRect.y) / canvasScale;
			_selectionAreaRT.sizeDelta = new Vector2(selectionRect.width, selectionRect.height) / canvasScale;
		}
	}
}
