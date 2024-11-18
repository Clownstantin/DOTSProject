using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Core
{
	public class UnitSelectionManager : MonoBehaviour
	{
		private Vector2 _startMousePos;

		public static UnitSelectionManager Instance { get; private set; }

		public event EventHandler OnSelectionStart;
		public event EventHandler OnSelectionEnd;

		private void Awake() => Instance = this;

		private void Update()
		{
			if(Input.GetMouseButtonDown(0))
			{
				_startMousePos = Input.mousePosition;
				OnSelectionStart?.Invoke(this, EventArgs.Empty);
			}
			else if(Input.GetMouseButtonUp(0))
			{
				Camera main = MouseWorldPosition.Instance.MainCamera;
				Rect selectionRect = GetSelectionAreaRect();
				EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
				EntityQueryBuilder queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>();
				EntityQuery query = queryBuilder.Build(entityManager);

				NativeArray<Entity> entityArray = query.ToEntityArray(Allocator.Temp);
				for(int i = 0; i < entityArray.Length; i++)
					entityManager.SetComponentEnabled<Selected>(entityArray[i], false);

				queryBuilder.Reset();
				query = queryBuilder.WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
				entityArray = query.ToEntityArray(Allocator.Temp);

				NativeArray<LocalTransform> localTRArray = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
				for(int i = 0; i < localTRArray.Length; i++)
				{
					LocalTransform localTR = localTRArray[i];
					Vector3 unitScreenPos = main.WorldToScreenPoint(localTR.Position);

					if(selectionRect.Contains(unitScreenPos))
						entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
				}

				OnSelectionEnd?.Invoke(this, EventArgs.Empty);
			}
			else if(Input.GetMouseButtonDown(1))
			{
				Vector3 mousePos = MouseWorldPosition.Instance.GetPosition();
				EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
				EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);
				NativeArray<UnitMover> moverArray = query.ToComponentDataArray<UnitMover>(Allocator.Temp);

				for(int i = 0; i < moverArray.Length; i++)
				{
					UnitMover unitMover = moverArray[i];
					unitMover.targetPosition = mousePos;
					moverArray[i] = unitMover;
				}

				query.CopyFromComponentDataArray(moverArray);
			}
		}

		public Rect GetSelectionAreaRect()
		{
			Vector2 mousePos = Input.mousePosition;
			Vector2 bottomLeftCorner = new(Mathf.Min(_startMousePos.x, mousePos.x), Mathf.Min(_startMousePos.y, mousePos.y));
			Vector2 topRightCorner = new(Mathf.Max(_startMousePos.x, mousePos.x), Mathf.Max(_startMousePos.y, mousePos.y));

			return new Rect(bottomLeftCorner.x, bottomLeftCorner.y, topRightCorner.x - bottomLeftCorner.x, topRightCorner.y - bottomLeftCorner.y);
		}
	}
}