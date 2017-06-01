﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace RTS
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Unit : MonoBehaviour, IUnit
    {
        [System.Serializable]
        public class Settings
        {
            public float range;
            public float attackRangeTolerance = .1f;
            public int damage;
            public Material idleMaterial;
            public Material selectedMaterial;
        }
        public Settings settings;

        public Transform selectionIndicator;
        public MeshRenderer meshRenderer;
        public UnitAnimationHandler animationHandler;


        public event System.Action OnDestroyed;

        NavMeshAgent navMeshAgent;
        IHittable hitTarget;




        public bool Selectable { get { return true; } }
        public bool Highlightable { get { return true; } }
        public bool CanTarget { get { return true; } }
        public bool Targetable { get { return true; } }
        public bool Hittable { get { return true; } }
        public GameObject Owner { get { return gameObject; } }
        public Vector3 position { get { return transform.position; } }
        public bool IsInRange
        {
            get
            {
                return hitTarget != null &&
                    (Vector3.Distance(hitTarget.position, transform.position) - settings.attackRangeTolerance) < settings.range;
            }
        }

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            meshRenderer = meshRenderer != null? meshRenderer: GetComponent<MeshRenderer>();
            meshRenderer.material = settings.idleMaterial;

            animationHandler.GetComponent<UnitAnimationHandler>();
            animationHandler.OnHitFrame += HitCurrentTarget;
        }
        void Update()
        {
            animationHandler.SetAttacking(IsInRange);
        }
        public void OnDestroy()
        {
            if (OnDestroyed != null)
                OnDestroyed();
        }



        public void Deselect()
        {
            selectionIndicator.gameObject.SetActive(false);
        }
        public void Select()
        {
            selectionIndicator.gameObject.SetActive(true);
        }
        
        public void HighlightOn()
        {
            meshRenderer.material = settings.selectedMaterial;
        }
        public void HighlightOff()
        {
            meshRenderer.material = settings.idleMaterial; 
        }

        public void TargetBy(ITargetReceiver targetReceiver)
        {
            throw new System.NotImplementedException();
        }

        public void SetTarget(ITargetable target, Vector3 position)
        {
            if (target != null)
            {
                navMeshAgent.stoppingDistance = 0;
                navMeshAgent.destination = position;
                var hittable = target as IHittable;
                setHitTarget(hittable);
                if (hittable != null)
                {
                    navMeshAgent.stoppingDistance = settings.range;
                }
            }
            else
                setHitTarget(null);
        }

        public void HitCurrentTarget()
        {
            if(hitTarget!= null)
                hitTarget.Hit(settings.damage);
        }

        public void Hit(int damage)
        {
            throw new System.NotImplementedException();
        }



        void clearHitTarget()
        {
            setHitTarget(null);
        }
        void setHitTarget(IHittable target)
        {
            if (hitTarget != null)
                hitTarget.OnDestroyed -= clearHitTarget;
            if (target != null)
                target.OnDestroyed += clearHitTarget;
            hitTarget = target;
        }
    }
}