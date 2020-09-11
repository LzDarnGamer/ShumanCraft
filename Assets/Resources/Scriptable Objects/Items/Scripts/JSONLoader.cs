using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONLoader : MonoBehaviour
{
    [SerializeField] public TextAsset LootTableJSON;

    public static Dictionary<string, LootTable> lootTables { private set; get; }
    void Start() {
        lootTables = new Dictionary<string, LootTable>();

        LootTables json = JsonUtility.FromJson<LootTables>(LootTableJSON.text);

        foreach (LootTable loot in json.lootTables) {
            lootTables.Add(loot.tablename, loot);
        }
    }
}
