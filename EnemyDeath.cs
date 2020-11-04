// Created by Ribbet400

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class Death : MonoBehaviour
{
    public float explosionRadius = 50f;
    public float explosionForce = 500f;
    public Boolean dead = false;
    public Boolean bulletHit = false;
    public Vector3 hitPoint = new Vector3(0f,0f,0f);
    private Boolean repeat = true;
    [SerializeField] AudioSource dyingSound;

    // Start is called before the first frame update
    void Start()
    {
        setCollider(false);
        setRigidbody(true);
    }
    private void Update()
    {
        if (repeat)
        {
            Explode(bulletHit, dead, hitPoint);
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "Player")
        {
            death();
        }
    }
    public void death()
    {
        GetComponentInParent<NavMeshAgent>().enabled = false;
        GetComponent<Animator>().enabled = false;
        Destroy(gameObject.GetComponent<Rigidbody>());
        setCollider(true);
        setRigidbody(false);
        dyingSound.Play(0);
        dead = true;
        Destroy(gameObject, 10f);
        Destroy(transform.parent.gameObject, 10f);//saves processing power
        
    
    }
    private void Explode(Boolean bulletHit, Boolean dead, Vector3 point)//called by bullet
    {
        if (bulletHit && dead)
        {
            Collider[] colliders = Physics.OverlapSphere(point, explosionRadius);
            foreach (Collider closeObjects in colliders)
            {
                Rigidbody rigidbody = closeObjects.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(explosionForce, point, explosionRadius);
                }
            }
            repeat = false;
        }
            
    }
    void setCollider(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider1 in colliders)
        {
            collider1.enabled = state;
        }
        GetComponent<Collider>().enabled = !state;
    }

    void setRigidbody(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
        GetComponent<Rigidbody>().isKinematic = !state;
    }
}
