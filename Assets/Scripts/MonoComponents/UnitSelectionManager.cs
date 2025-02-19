using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Core
{
	public class UnitSelectionManager : MonoBehaviour
	{
		private const float MinSelectionSize = 50f;

		[Header("Single selection")]
		[SerializeField] private LayerMask _colidesWith;
		[SerializeField] private LayerMask _belongsTo;

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

				float selectionSize = selectionRect.size.magnitude;
				queryBuilder.Reset();

				if(selectionSize >= MinSelectionSize)
				{
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
				}
				else
				{
					query = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
					CollisionWorld collisionWorld = query.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
					Vector3 mousePos = MouseWorldPosition.Instance.GetPosition();

					CollisionFilter collisionFilter = new()
					{
						BelongsTo = (uint)_belongsTo.value,
						CollidesWith = (uint)_colidesWith.value,
					};

					RaycastInput input = new()
					{
						Start = main.transform.position,
						End = mousePos,
						Filter = collisionFilter
					};

					if(collisionWorld.CastRay(input, out Unity.Physics.RaycastHit closestHit))
						entityManager.SetComponentEnabled<Selected>(closestHit.Entity, true);
				}

				OnSelectionEnd?.Invoke(this, EventArgs.Empty);
			}
			else if(Input.GetMouseButtonDown(1))
			{
				Vector3 mousePos = MouseWorldPosition.Instance.GetPosition();
				EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
				EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);
				NativeArray<UnitMover> moverArray = query.ToComponentDataArray<UnitMover>(Allocator.Temp);
				NativeArray<float3> movePositions = GenerateCirclePositionArray(mousePos, moverArray.Length);

				for(int i = 0; i < moverArray.Length; i++)
				{
					UnitMover unitMover = moverArray[i];
					unitMover.targetPosition = movePositions[i];
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

		private NativeArray<float3> GenerateCirclePositionArray(float3 targetPos, int posCount)
		{
			NativeArray<float3> posArray = new(posCount, Allocator.Temp);
			if(posCount <= 0)
				return posArray;

			posArray[0] = targetPos;
			if(posCount > 1)
			{
				float ringSize = 2.2f;
				int ring = 0;
				int posIndex = 1;
				int minRingCount = 3;
				int scaleValue = 2;

				while(posIndex < posCount)
				{
					int ringPosCount = minRingCount + ring * scaleValue;

					for(int i = 0; i < ringPosCount; i++)
					{
						float angle = i * (math.PI2 / ringPosCount);
						float3 ringRotation = math.rotate(quaternion.RotateY(angle), new(ringSize * (ring + 1), 0, 0));
						float3 ringPos = targetPos + ringRotation;

						posArray[posIndex] = ringPos;
						posIndex++;

						if(posIndex >= posCount)
							break;
					}

					ring++;
				}
			}

			return posArray;
		}
	}
}