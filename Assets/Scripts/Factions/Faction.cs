using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction
{
    //non-monobehavior class that is constructed from a FactionBase and stored by the FactionManager for only relevant data.
    private FactionBase _base;
    public FactionBase FactionBase => _base; //public getter so _base cannot be modified externally (preventing possible data loss)
    public CaptureChance CapChance;
    public Faction()
    {
        //blank constructor
        _base = null;
    }
    public Faction(FactionBase fBase)
    {
        _base = fBase;
        CapChance = fBase.CaptureChance;

    }
    public void AssignFactionBase(FactionBase factionBase) //if a blank constructor is called and later needs to be used
    {
        if (_base == null)
        {
            _base = factionBase;
            return;
        }

        Debug.LogError("Attempted to reassign an existing faction's base. This is not allowed.");
    }

    //Dynamic faction fields
    
}