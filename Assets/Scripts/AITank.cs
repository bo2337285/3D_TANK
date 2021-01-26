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
    public bool isHurt = false;

    #endregion
    #region 钩子函数
    protected override void Start () {
        base.Start ();
        Init ();
    }
    void Update () {
        // AIAction ();
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
    public override void BeHurt (Battle battle) {
        isHurt = true;
        base.BeHurt (battle);
    }
    // 挨打的特殊处理
    IEnumerator HurtAction () {
        // 找打我的列表
        List<Battle> battleList = BattleManager.Instance.suffererBattleDict[gameObject.GetInstanceID ()];
        if (battleList.Count > 0) {
            // 找最近打我的
            GameObject giver = GameManager.Instance.GetUnitById (battleList[0].giverId);
            if (giver != null) {
                target = giver.transform;
                // Debug.Log ("疼!", giver);
            }
        }
        yield return new WaitForEndOfFrame ();
        isHurt = false;
    }
    private void InitFSM () {
        FSMControler controler = FSMManager.Instance.CreateFSMControler (gameObject.GetInstanceID ().ToString (), gameObject);
        #region 创建状态
        FSMState idle = new FSMState ("idle");
        FSMState follow = new FSMState ("follow", null, () => {
            nav.SetDestination (target.position);
        });
        FSMState patrol = new FSMState ("patrol", null,
            () => {
                SearchEnemy ();
                Patrol ();
            }
        );
        FSMState attack = new FSMState ("attack",
            null,
            () => {
                nav.ResetPath ();
                Attack ();
            }
        );
        FSMState hurt = new FSMState ("hurt", null,
            () => {
                StartCoroutine (HurtAction ());
            });
        #endregion
        #region  建立状态切换条件
        #region idle
        // idle -> follow
        idle.RegisterChangeEvt (follow, (object[] o) => {
            return target != null;
        }, gameObject);
        // idle -> patrol
        idle.RegisterChangeEvt (patrol, (object[] o) => {
            return target == null;
        }, gameObject);
        // idle -> hurt
        idle.RegisterChangeEvt (hurt, (object[] o) => {
            return isHurt;
        }, gameObject);
        #endregion
        #region follow
        // follow -> patrol
        follow.RegisterChangeEvt (patrol, (object[] o) => {
            return target == null;
        }, gameObject);
        // follow -> attack
        follow.RegisterChangeEvt (attack, (object[] o) => {
            return CanAttack ();
        }, gameObject);
        // follow -> hurt
        follow.RegisterChangeEvt (hurt, (object[] o) => {
            //TODO 有目标的情况下被打,得根据仇恨系统处理
            return isHurt;
        }, gameObject);
        #endregion
        #region  patrol
        // patrol -> follow
        patrol.RegisterChangeEvt (follow, (object[] o) => {
            return target != null;
        }, gameObject);
        // patrol -> attack
        patrol.RegisterChangeEvt (attack, (object[] o) => {
            return CanAttack ();
        }, gameObject);
        // patrol -> hurt
        patrol.RegisterChangeEvt (hurt, (object[] o) => {
            return isHurt;
        }, gameObject);
        #endregion
        #region  attack
        // attack -> patrol
        attack.RegisterChangeEvt (patrol, (object[] o) => {
            return target == null;
        }, gameObject);
        // attack -> follow
        attack.RegisterChangeEvt (follow, (object[] o) => {
            return !CanAttack ();
        }, gameObject);
        // attack -> hurt
        attack.RegisterChangeEvt (hurt, (object[] o) => {
            return isHurt;
        }, gameObject);
        #endregion
        #region  hurt
        hurt.RegisterChangeEvt (idle, (object[] o) => {
            return !isHurt;
        }, gameObject);
        #endregion
        #endregion
        #region 添加状态到controler
        controler.AddState (idle);
        controler.AddState (follow);
        controler.AddState (patrol);
        controler.AddState (attack);
        controler.AddState (hurt);
        controler.StartState (idle);
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
        unitSetting = ResourceManager.Instance.enemyInfo;
        InitFSM ();
    }
    private void Patrol () {
        // Debug.Log (nav.isOnNavMesh);
        if (target != null) return;
        Vector3 dstPoint;
        // 检查是否有巡逻点,没有则创建
        if (!(patrolPointList.Count > 0)) {
            GetPatrolPoint ();
        }
        // 到达目标点,则切换下个目标点
        if (!nav.pathPending && nav.remainingDistance <= nav.stoppingDistance + 0.5f) {
            dstPoint = patrolPointList[currPatrolPointIdx];
            currPatrolPointIdx = currPatrolPointIdx < patrolPointList.Count - 1 ? currPatrolPointIdx + 1 : 0;
            // Debug.Log ("changePoint", gameObject);
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
        if (target != null) {
            NavMeshHit hit;
            bool hasObstacles = target != null? nav.Raycast (target.position, out hit) : true;
            if (!hasObstacles && nav.remainingDistance <= attackDist) {
                nav.ResetPath ();
                Attack ();
            } else {
                nav.SetDestination (target.position);
            }
        } else {
            // 索敌
            SearchEnemy ();
            // 巡逻
            Patrol ();
        }
    }
    private void SearchEnemy () {
        GameObject _targetObj = PhysicsUtils.SearchTargetSector (transform, searchDist, searchAngle, searchAccuracy, searchSpeed, CanAttackFilter);
        if (_targetObj != null) {
            target = _targetObj.transform;
        }
    }
    private bool CanAttack () {
        NavMeshHit hit;
        bool hasObstacles = target != null? nav.Raycast (target.position, out hit) : true;
        return !hasObstacles && nav.remainingDistance <= attackDist;
    }
    #endregion
}