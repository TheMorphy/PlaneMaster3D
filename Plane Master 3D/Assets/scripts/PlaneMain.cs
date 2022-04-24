using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMain : MonoBehaviour
{
    DroppingZone dz;
    [SerializeField]
    public List<Breakable> breakables = new List<Breakable>();
    [SerializeField]
    float planeHealth;
    [SerializeField]
    float planeMaxHealth;
    [SerializeField]
    float smoothLerp;
    float repairPercent;
    [SerializeField]
    List<ParticleSystem> fire = new List<ParticleSystem>();
    [SerializeField]
    float fireStopPercentage;
    private void OnEnable()
    {
        dz = GetComponent<DroppingZone>();
        OnAddItem();
    }
    float curPercentage()
    {
        int cur = 0;
        int max = 0;
        foreach(UpgradeCondition u in dz.conditions)
        {
            cur += u.count;
            max += u.countNeeded;
        }
        //print(cur);
        return ((float)cur / max) * 100;
    }
    private void Update()
    {
        //print(curPercentage());
    }
    public void OnAddItem()
    {
        repairPercent = curPercentage();
        planeHealth = repairPercent;

        for (int i = 0; i < breakables.Count; i++)
        {

            if (breakables[i].health < 1)
            {
                if (repairPercent < breakables[i].neededRepairPercent)
                {
                    if (i == 0)
                    {
                        breakables[i].health = repairPercent / breakables[i].neededRepairPercent;
                        breakables[i].obj.GetComponent<Outline>().enabled = true;
                        //dz.conditions[0].itemDestination.position = breakables[i].obj.position;
                        //dz.conditions[1].itemDestination.position = breakables[i].obj.position;
                    }
                    else if (repairPercent > breakables[i - 1].neededRepairPercent)
                    {
                        breakables[i].health = (repairPercent - breakables[i - 1].neededRepairPercent) / (breakables[i].neededRepairPercent - breakables[i - 1].neededRepairPercent);
                        breakables[i].obj.GetComponent<Outline>().enabled = true;
                        //dz.conditions[0].itemDestination.position = breakables[i].obj.position;
                        //dz.conditions[1].itemDestination.position = breakables[i].obj.position;
                    }
                    else
                        breakables[i].obj.GetComponent<Outline>().enabled = false;
                }
                else
                {
                    breakables[i].health = 1;
                    breakables[i].obj.GetComponent<Outline>().enabled = false;
                }
                    Vector3 pos = breakables[i].normalPosition.position * breakables[i].health + breakables[i].brokenPosition.position * (1 - breakables[i].health);
                    Quaternion rot = Quaternion.Euler(breakables[i].normalPosition.rotation.eulerAngles * breakables[i].health + breakables[i].brokenPosition.rotation.eulerAngles * (1 - breakables[i].health));

                    if (breakables[i].obj.position != pos || breakables[i].obj.rotation != rot)
                    {
                        if (breakables[i].alignCoroutine != null)
                        {
                            StopCoroutine(breakables[i].alignCoroutine);
                            breakables[i].alignCoroutine = StartCoroutine(alignTransforms(breakables[i].obj, pos, rot));
                        }
                        else
                        {
                            breakables[i].alignCoroutine = StartCoroutine(alignTransforms(breakables[i].obj, pos, rot));
                        }
                    }
                    //breakables[i].obj.position = breakables[i].normalPosition.position * breakables[i].health + breakables[i].brokenPosition.position * (1 - breakables[i].health);
                    //breakables[i].obj.rotation = Quaternion.Euler(breakables[i].normalPosition.rotation.eulerAngles * breakables[i].health + breakables[i].brokenPosition.rotation.eulerAngles * (1 - breakables[i].health));

                }
            }

        if(repairPercent >= fireStopPercentage)
        {
            foreach(ParticleSystem p in fire)
            {
                p.Stop();
            }
        }
        
    }

        IEnumerator alignTransforms(Transform t, Vector3 p, Quaternion r)
        {
            while(t.position != p || t.rotation != r)
            {
                t.position = Vector3.Lerp(t.position, p, smoothLerp);
                t.rotation = Quaternion.Lerp(t.rotation, r, smoothLerp);
                yield return null;
            }
        }
}
