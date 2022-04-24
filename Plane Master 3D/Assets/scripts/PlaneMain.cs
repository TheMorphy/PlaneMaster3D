using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMain : MonoBehaviour
{
    DroppingZone dz;
    [SerializeField]
    public List<Breakable> breakables = new List<Breakable>();
    
    private void OnEnable()
    {
        dz = GetComponent<DroppingZone>();
        AssignPlainMains();
    }

    void AssignPlainMains()
    {
        foreach (Breakable b in breakables)
        {
            b.planeMain = this;
        }
    }

    void OnBreakableRepaired()
    {
        bool allRepaired = true;
        foreach(Breakable b in breakables)
        {
            if(!b.isRepaired)
            {
                allRepaired = false;
            }
        }
        if(allRepaired)
        {
            //TakeOff
            //Get The money 
            //...
        }
    }
    
    

        
}
