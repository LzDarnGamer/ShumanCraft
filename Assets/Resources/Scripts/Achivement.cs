﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achivement 
{
    
    public string achName;
    public string description;
    public bool isLocked = true;
    public Sprite icon;
    public int[] requirement;
    public bool isDone;
    public Achivement(string achName, string description, int[] requirement, Sprite icon) {
        this.requirement = requirement;
        this.achName = achName;
        this.description = description;
        this.icon = icon;
        isDone = false;
    }
    

}