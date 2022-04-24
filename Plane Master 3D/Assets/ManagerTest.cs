using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTest : MonoBehaviour
{
    /*[Header("Plane Parts")]
    [SerializeField] List<GameObject> pPref = new List<GameObject>();

    [SerializeField] List<GameObject> objScene = new List<GameObject>();

    [Space]
    [SerializeField] GameObject dropZone;
    [SerializeField] List<Transform> dzTransform = new List<Transform>();
    [Space]
    [SerializeField] GameObject container;
    [SerializeField] List<Transform> cTransform = new List<Transform>();*/

    [SerializeField] List<Transform> realNormalPos = new List<Transform>();

    [SerializeField] List<Transform> truckNormalPos = new List<Transform>();

    [SerializeField] PlaneMain ps;

    GameObject addObj;

    public void DoScript(List<Transform> groundPositions)
    {
        ChangeNormalPosition();
        //ps.enabled = true;
        StartCoroutine(MoveBreakablesToGroundDestinations(groundPositions));
    }

    void ChangeNormalPosition()
    {
        StartCoroutine(ChangePos());
    }

    IEnumerator MoveBreakablesToGroundDestinations(List<Transform> groundPositions)
    {
        while(Vector3.Distance(ps.breakables[0].transform.position, groundPositions[0].position) > 0.1f)
        {
            for (int i = 0; i < ps.breakables.Count; i++)
            {
                ps.breakables[i].transform.position = Vector3.Lerp(ps.breakables[i].transform.position, groundPositions[i].position, 0.1f);
                ps.breakables[i].transform.rotation = Quaternion.Lerp(ps.breakables[i].transform.rotation, groundPositions[i].rotation, 0.1f);
            }
            yield return null;
        }
        //Parts moved to the ground
        yield break;
    }

    private IEnumerator ChangePos()
    {
        for (int i = 0; i < realNormalPos.Count; i++)
        {
            //ps.breakables[i].normalPosition = truckNormalPos[i];
        }
        yield return new WaitForSeconds(1);
        for (int i = 0; i < realNormalPos.Count; i++)
        {
            //ps.breakables[i].normalPosition = realNormalPos[i];
        }

        /*if (addObj.transform.Find("BrokenPos") != null)
        {
            GameObject bp = pPref[i].transform.Find("BrokenPos").gameObject;
            bp.transform.position = dzChild.transform.position;
            ps.breakables[i].brokenPosition = bp.transform;
        }
        else Debug.Log("It doesn't have BrokenPos");*/
    }

        /*void FindBrokenPieces()
        {
            for(int i = 0; i < pPref.Count; i++)
            {
                //If shit goes wrong GameObject addObj

                //addObj = GameObject.Find(pPref[i].name);

                if (addObj.layer == 10)
                {
                    objScene.Add(addObj);

                    addObj.SetActive(false);

                    if (pPref[i].name == addObj.name)
                    {
                        //pPref[i] = Instantiate(pPref[i]);

                        GameObject child = pPref[i].transform.GetChild(0).gameObject;

                        ps.breakables[i].obj = child.transform;

                        if (addObj.transform.Find("NormalPos") != null)
                        {
                            //GameObject np = addObj.transform.Find("NormalPos").gameObject;
                            //print(np.transform.position);
                            //pPref[i].transform.position = np.transform.position;
                            //ps.breakables[i].normalPosition = pPref[i].transform;
                            //print(pPref[i].transform);
                        }
                        else Debug.Log("It doesn't have NormalPos");
                        // NEED TO WORK ON THE BROKEN POSITION IN THE SCENE AND MAKE IT WORK WITH THE MATERIALS, ALSO ADD MONEY. WILL FINISH TOMORROW.

                        Transform dzChild = dropZone.transform.GetChild(i).transform;

                        //print(dzChild);

                        dzTransform.Add(dzChild);

                        if (addObj.transform.Find("BrokenPos") != null)
                        {
                            GameObject bp = pPref[i].transform.Find("BrokenPos").gameObject;
                            bp.transform.position = dzChild.transform.position;
                            ps.breakables[i].brokenPosition = bp.transform;
                        }
                        else Debug.Log("It doesn't have BrokenPos");
                    }
                }
            }
        }*/
    }
