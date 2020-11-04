using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEditorInternal;

public class Rotate : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotationSpeed;
    private Vector3 newTargetPos;
    [SerializeField] float minDistance = 5f;
    [SerializeField] float maxDistance = 15f;
    private Boolean dead;
    NavMeshAgent navMeshAgent;
    public GameObject gun;
    public GameObject RotateHead;
    public GameObject head;
    [SerializeField] float headRotationSpeed;

    public GameObject RotateRarm;
    private Quaternion PrevRarmRot;
    [SerializeField] float RarmRotationSpeed;

    private bool canShoot = false;

    private void Start()
    {
        PrevRarmRot = RotateRarm.transform.localRotation;
        navMeshAgent = GetComponent<NavMeshAgent>();
        
    }
    void Update()
    {

        dead = GetComponentInChildren<Death>().dead;
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= maxDistance && !dead) // chase player
        {
            // desired look directions for all components
            newTargetPos = target.position;
            Vector3 direction = newTargetPos - transform.position;
            Vector3 headDirection = newTargetPos - head.transform.position;
            Vector3 armDirection = newTargetPos - RotateRarm.transform.position;

            //R.ARM - moves with body
            Quaternion RArmRotation = Quaternion.LookRotation(armDirection);
            RotateRarm.transform.rotation = Quaternion.Lerp(RotateRarm.transform.rotation, RArmRotation, RarmRotationSpeed * Time.deltaTime);
            StartCoroutine(Wait());

            //HEAD - moves with body
            Quaternion HeadRotation = Quaternion.LookRotation(headDirection);
            RotateHead.transform.rotation = Quaternion.Lerp(RotateHead.transform.rotation, HeadRotation, headRotationSpeed * Time.deltaTime);

            //BODY
            direction.y = 0; //no vertical rotation
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

            RaycastHit hit;
            var rayDirection = target.position - head.transform.position;
            if (Physics.Raycast(head.transform.position, rayDirection, out hit, maxDistance))
            {
                if (hit.transform.gameObject.tag == "Player") // can see player
                {
                    if (canShoot)
                    {
                        gun.GetComponent<Enemyshoot>().canSeePlayer = true;
                    }
                    
                    if (distance >= minDistance)
                    {
                        navMeshAgent.SetDestination(target.position);
                    }
                    else
                    {
                        navMeshAgent.SetDestination(transform.position);
                    }

                }
                else // hit something else
                {
                    gun.GetComponent<Enemyshoot>().canSeePlayer = false;
                    navMeshAgent.SetDestination(target.position);
                    canShoot = false;
                }
            }
            else // no hit
            {
                gun.GetComponent<Enemyshoot>().canSeePlayer = false;
                navMeshAgent.SetDestination(target.position);
                canShoot = false;
            }
        }
        else
        {
            // NMA is deleted
            gun.GetComponent<Enemyshoot>().canSeePlayer = false;
            canShoot = false;
        }
        if (distance > maxDistance && !dead)
        {
            LowerRArm();
            gun.GetComponent<Enemyshoot>().canSeePlayer = false;
            navMeshAgent.SetDestination(transform.position);
            canShoot = false;
        }

    }
    IEnumerator Wait() //wait to shoot
    {
        yield return new WaitForSeconds(1);       
        canShoot = true;       
    }
    private void LowerRArm()
    {
        //LOWER R.ARM
        Quaternion RArmRotation = PrevRarmRot;
        RotateRarm.transform.localRotation = Quaternion.Lerp(RotateRarm.transform.localRotation, RArmRotation, RarmRotationSpeed * Time.deltaTime);
    }

}
