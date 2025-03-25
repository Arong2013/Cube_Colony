using System.Collections.Generic;
using UnityEngine;

public class Enemy : FaceUnit
{
    [SerializeField] private List<BehaviorSequenceSO> behaviorSequencesSO;
    private List<BehaviorSequence> behaviorSequences = new List<BehaviorSequence>();

    private Transform target; // 플레이어 타겟
    private float detectionRange = 5.0f;

    public override void Init(CubieFace cubieFace)
    {
        base.Init(cubieFace);
        behaviorSequencesSO.ForEach(sequence => behaviorSequences.Add(sequence.CreateBehaviorSequence(this)));
        target = GameObject.FindWithTag("Player")?.transform; // 예제 코드
    }
    public BehaviorState Execute()
    {
        foreach (var seq in behaviorSequences)
        {
            var behaviorState = seq.Execute();
            if (behaviorState == BehaviorState.SUCCESS || behaviorState == BehaviorState.RUNNING)
            {
                return behaviorState;
            }
        }
        return BehaviorState.FAILURE;
    }

    // A* 경로 탐색을 위한 큐브 페이스 리스트
    public List<CubieFace> GetAstarCubeFace(FaceUnit targetUnit)
    {
        var targetFace = targetUnit.ParentFace;
		return new List<CubieFace>();
    }

    // 감지된 FaceUnit 중 같은 면에 있는 애들만 반환
    public List<FaceUnit> CheckEnemy()
    {
        List<FaceUnit> detectedEnemies = new List<FaceUnit>();
        // 지정된 detectionRange 내의 모든 Collider 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (var col in colliders)
        {
            FaceUnit FaceUnit = col.GetComponent<FaceUnit>();

            // FaceUnit가 존재하고, 현재 Enemy와 같은 면(Face)에 있을 경우 추가
            if (FaceUnit != null && FaceUnit.CubeFaceType == this.CubeFaceType)
            {
                detectedEnemies.Add(FaceUnit);
            }
        }

        return detectedEnemies;
    }
}
