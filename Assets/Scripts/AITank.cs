using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AITank : Unit {

    #region  属性
    [HideInInspector]
    public Transform target;
    public float searchSpeed;
    public float searchDist;
    public float searchAngle;
    public float searchAccuracy = 1f; //检测精度
    public float attackDist;
    public float moveSpeed;
    public float roateSpeed;
    TankWeapon weapon;
    public float shootCD;
    bool canShoot = true;
    NavMeshAgent nav;
    public List<Vector3> patrolPointList;
    public int randomPatrolPointCount = 2;
    public int currPatrolPointIdx = 0;
    public UnitSetting unitSetting;
    #endregion
    #region 钩子函数
    void Start () {
        Init ();
    }
    void Update () {
        AIAction ();
    }
    private void OnDrawGizmos () {
        // 沿轴旋转 Quaternion.AngleAxis (angle, transform.up)*dir, 一定要左乘
        GizmosUtils.DrawWireSemicircle (transform.position, transform.forward, searchDist, searchAngle, transform.up);
        // Gizmos.DrawMesh (GizmosUtils.SemicircleMesh (searchDist, searchAngle, transform.up));
        if (nav != null) {
            Vector3[] corners = nav.path.corners;
            if (corners.Length > 0) {
                for (var i = 0; i < corners.Length; i++) {
                    Gizmos.DrawLine (i == 0 ? corners[0] : corners[i - 1], corners[i]);
                }
            }
        }
    }
    #endregion
    #region 函数
    private void InitFSM () {
        FSMControler controler = FSMManager.Instance.CreateFSMControler (unitSetting.unitName, gameObject);
        #region 创建状态
        FSMState idle = new FSMState ("idle", () => {
            Debug.Log ("idle");
        });
        FSMState follow = new FSMState ("follow", () => {
            Debug.Log ("follow");
        }, () => {
            nav.SetDestination (target.position);
        });
        FSMState patrol = new FSMState ("patrol", () => {
            Debug.Log ("patrol");
        }, () => {
            SearchEnemy ();
            Patrol ();
        });
        FSMState attack = new FSMState ("attack", () => {
            Debug.Log ("attack");
        }, () => {
            nav.ResetPath ();
            Attack ();
        });
        #endregion
        #region  建立状态切换条件
        // idle -> follow
        idle.RegisterChangeEvt (follow, (object[] o) => {
            return target != null;
        });
        // idle -> patrol
        idle.RegisterChangeEvt (patrol, (object[] o) => {
            return target == null;
        });
        // follow -> patrol
        // follow -> attack
        // patrol -> follow
        // patrol -> attack
        // attack -> patrol
        // attack -> follow
        // attack -> idle
        #endregion
        #region 添加状态到controler
        #endregion
    }
    private void Attack () {
        // 差值旋转
        Vector3 dir = target.position - transform.position;
        Quaternion wantedRotation = Quaternion.LookRotation (dir);
        transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, roateSpeed * Time.deltaTime);
        if (canShoot) {
            StartCoroutine (Shoot ());
        }
    }
    IEnumerator Shoot () {
        weapon.Shoot ();
        canShoot = false;
        yield return new WaitForSeconds (shootCD);
        canShoot = true;
    }
    public void Init (Transform _target = null) {
        if (_target != null) {
            target = _target;
        }
        weapon = GetComponent<TankWeapon> ();
        nav = GetComponent<NavMeshAgent> ();
        nav.speed = moveSpeed;
        nav.stoppingDistance = attackDist;
        // InitFSM ();
    }
    private void Patrol () {
        if (target != null) return;
        Vector3 dstPoint;
        // 检查是否有巡逻点,没有则创建
        if (!(patrolPointList.Count > 0)) {
            GetPatrolPoint ();
        }
        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance + 0.5f) {
            dstPoint = patrolPointList[currPatrolPointIdx];
            currPatrolPointIdx = currPatrolPointIdx < patrolPointList.Count - 1 ? currPatrolPointIdx + 1 : 0;
            Debug.Log ("changePoint", gameObject);
            nav.SetDestination (dstPoint);
        }
    }
    private void GetPatrolPoint () {
        // 遍历randomPatrolPointCount次
        for (var i = 0; i < randomPatrolPointCount; i++) {
            if (i == 0) {
                patrolPointList.Add (transform.position);
            } else {
                patrolPointList.Add (PhysicsUtils.GetRandomPointInMap (new Vector2 (20f, 60f), 20f, patrolPointList[i - 1]));
            }
        }
    }
    private void AIAction () {
        Patrol ();

        // if (target != null) {
        //     NavMeshHit hit;
        //     bool hasObstacles = target != null? nav.Raycast (target.position, out hit) : true;
        //     if (!hasObstacles && nav.remainingDistance <= attackDist) {
        //         nav.ResetPath ();
        //         Attack ();
        //     } else {
        //         nav.SetDestination (target.position);
        //     }
        // } else {
        //     // 索敌
        //     SearchEnemy ();
        //     // 巡逻
        //     Patrol ();
        // }
    }
    // 索敌
    private void SearchEnemy () {
        GameObject _targetObj = PhysicsUtils.SearchTargetSector (transform, searchDist, searchAngle, searchAccuracy, searchSpeed, CanAttackFilter);
        if (_targetObj != null) {
            target = _targetObj.transform;
        }
    }
    #endregion
}