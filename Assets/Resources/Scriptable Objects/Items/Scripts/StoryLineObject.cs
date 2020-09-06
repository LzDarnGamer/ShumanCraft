using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StoryLine Object", menuName = "Inventory System/Items/StoryLine")]
public class StoryLineObject : ItemObject {
    
    private void Awake() {
        type = ItemType.Story;
    }

}
