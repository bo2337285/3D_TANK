using UnityEngine;
using UnityEngine.AI;

public static class PhysicsUtils {
    public delegate bool SearchFilter (RaycastHit hit);
    public static Vector3 getMousePos () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        Physics.Raycast (ray, out hit, 1000, LayerMask.GetMask ("Ground"));
        return hit.point;
    }
    public static Vector3 GetRandomPoint (Vector2 randomRange, Vector3 samplePoint = default (Vector3)) {
        float mRadius = Random.Range (randomRange.x, randomRange.y);
        float fRadius = Random.Range (0, mRadius);
        float fAngle = Random.Range (0, 3.14f);
        Vector3 point = samplePoint;
        point.x += Mathf.Sin (fAngle) * fRadius * (Random.value > 0.5f? 1: -1);
        point.z += Mathf.Cos (fAngle) * fRadius * (Random.value > 0.5f? 1: -1);
        point.y = 0;
        return point;
    }

    public static Vector3 GetRandomPointInMap (Vector2 randomRange, float pointDistance, Vector3 samplePoint) {
        NavMeshHit hit;
        Vector3 point = GetRandomPoint (randomRange, samplePoint);
        int loopCount = 10;
        while (
            loopCount > 0 &&
            ((samplePoint - point).sqrMagnitude < pointDistance * pointDistance ||
                !NavMesh.SamplePosition (point, out hit, 10, 1))
        ) {
            point = GetRandomPoint (randomRange, samplePoint);
            loopCount--;
        }
        return point;
    }

    /// <summary>
    /// 扇形搜索目标(实现思路是在扇形区域内发多条射线扫描)
    /// </summary>
    /// <param name="transform"> 搜索发起transform</param>
    /// <param name="searchDist">搜索半径</param>
    /// <param name="searchAngle">搜索角度</param>
    /// <param name="searchAccuracy">搜索精度(指同时发几条线)</param>
    /// <param name="searchSpeed">搜索角度变化速度(角度值/秒)</param>
    /// <param name="filter">搜到collider之后,调用者对搜索目标的进一步过滤</param>
    public static GameObject SearchTargetSector (Transform transform, float searchDist, float searchAngle, float searchAccuracy, float searchSpeed, SearchFilter filter) {
        float subAngle = searchAngle / searchAccuracy; //每条射线需要检测的角度范围
        for (int i = 0; i < searchAccuracy; i++) {
            GameObject _target = SearchAround (
                transform,
                Quaternion.Euler (0, -searchAngle / 2 + i * subAngle + Mathf.Repeat (searchSpeed * Time.time, subAngle), 0),
                searchDist,
                filter
            );
            if (_target != null) {
                return _target;
            }
        }
        return null;
    }
    /// <summary>
    /// 按角度搜索目标
    /// </summary>
    public static GameObject SearchAround (Transform transform, Quaternion eulerAnger, float searchDist, SearchFilter filter) {
        // 画出扫描线
        Debug.DrawRay (transform.position, eulerAnger * transform.forward.normalized * searchDist, Color.red);
        RaycastHit hit;
        if (
            Physics.Raycast (transform.position, eulerAnger * transform.forward, out hit, searchDist) &&
            filter (hit) // 外部对扫到的对象进一步过滤
        ) {
            return hit.collider.gameObject;
        } else {
            return null;
        }
    }
}