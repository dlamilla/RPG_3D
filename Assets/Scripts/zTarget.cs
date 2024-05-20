using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class zTarget : MonoBehaviour
{
    public float viewScope;
    public Transform cam;
    public Transform t;
    public List<Transform> impacts;
    public List<Transform> targetL;
    public List<Transform> targetR;
    private void Awake()
    {
        impacts = new List<Transform>();
        targetL = new List<Transform>();
        targetR = new List<Transform>();
    }

    public Transform FirtsTarger()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewScope);
        impacts.Clear();
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Target")
            {
                if (!impacts.Contains(hitCollider.transform))
                {
                    Vector3 dir = (hitCollider.transform.position - cam.position).normalized;
                    float f = Vector3.Dot(dir, cam.forward);
                    if (f > 0)
                    {
                        impacts.Add(hitCollider.transform);
                    }
                }
            }
        }
        impacts = impacts.OrderBy(i => Vector3.Distance(cam.position, i.position)).ToList();
        if (impacts.Count == 0)
        {
            impacts.Add(null);
        }
        t = impacts[0];
        return t;

    }

    private void UpdateImpacts()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewScope);
        impacts.Clear();
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Target")
            {
                if (!impacts.Contains(hitCollider.transform))
                {
                    impacts.Add(hitCollider.transform);
                }
            }
                
        }
        impacts = impacts.OrderBy(i => Vector3.Distance(cam.position, i.position)).ToList();
        if (impacts.Count == 0)
        {
            impacts.Add(null);
            t = impacts[0];
        }
    }

    public Transform NextToLeft()
    {
        UpdateImpacts();
        if (impacts.Count > 1)
        {
            targetL.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewScope);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.tag == "Target")
                {
                    if (!targetL.Contains(hitCollider.transform))
                    {
                        targetL.Add(hitCollider.transform);
                    }
                }
                    
            }
        }

        targetL = targetL.OrderBy(i => 
        {
            Vector3 dir = (i.position - cam.position).normalized;
            float f = Vector3.Dot(dir, cam.right);
            return f;
        }).ToList();

        if (targetL[0] == t)
        {
            MaxRight();
        }
        else
        {
            Transform previous = null;
            foreach (Transform e in targetL)
            {
                if (e == t)
                {
                    break;
                }
                previous = e;
            }
            t = previous;
        }
        return t;
    }

    public Transform NextToRight()
    {
        UpdateImpacts();
        if (impacts.Count > 1)
        {
            targetR.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewScope);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.tag == "Target")
                {
                    if (!targetR.Contains(hitCollider.transform))
                    {
                        targetR.Add(hitCollider.transform);
                    }
                }
                    
            }
        }

        targetR = targetR.OrderByDescending(i =>
        {
            Vector3 dir = (i.position - cam.position).normalized;
            float f = Vector3.Dot(dir, cam.right);
            return f;
        }).ToList();

        if (targetR[0] == t)
        {
            MaxLeft(); 
        }
        else
        {
            Transform previous = null;
            foreach (Transform e in targetR)
            {
                if (e == t)
                {
                    break;
                }
                previous = e;
            }
            t = previous;
        }
        return t;
    }

    public void MaxLeft()
    {
        targetL.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewScope);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Target")
            {
                if (!targetL.Contains(hitCollider.transform))
                {
                    targetL.Add(hitCollider.transform);
                }
            }
                
        }

        targetL = targetL.OrderBy(i =>
        {
            Vector3 dir = (i.position - cam.position).normalized;
            float f = Vector3.Dot(dir, cam.right);
            return f;
        }).ToList();

        targetL.Remove(t);
        t = targetL[0];
    }

    public void MaxRight()
    {
        targetR.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewScope);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Target")
            {
                if (!targetR.Contains(hitCollider.transform))
                {
                    targetR.Add(hitCollider.transform);
                }
            }
                
        }

        targetR = targetR.OrderByDescending(i =>
        {
            Vector3 dir = (i.position - cam.position).normalized;
            float f = Vector3.Dot(dir, cam.right);
            return f;
        }).ToList();

        targetR.Remove(t);
        t = targetR[0];
    }
}
