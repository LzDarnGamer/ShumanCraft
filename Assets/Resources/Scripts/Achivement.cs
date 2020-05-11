using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achivement 
{
    
    public string achName;
    public string description;
    public bool isLocked = true;
    public Sprite icon;
    public Dictionary<int, int> requirement;
    public Achivement(string achName, string description, Dictionary<int, int> requirement, Sprite icon) {
        this.requirement = requirement;
        this.achName = achName;
        this.description = description;
        this.icon = icon;
    }
    

}
