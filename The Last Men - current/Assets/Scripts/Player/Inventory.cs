using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class Inventory : MonoBehaviour {

    public int ArtifactID = 666;    //just for reference since we dont have a proper item db yet
    public GameObject UIInventory;

    private List<Item> inventory = new List<Item>();
    private SortedDictionary<int, Item> itemDictionary = new SortedDictionary<int, Item>();
    private bool inventoryActive = false;

    //TODO: all item desription should be available in an excel sheet
    //only add item by id
    //When the excel sheet is setup we dont want to use List<Item> itemDitionary anymore
    void Start()
    {
        itemDictionary.Add(ArtifactID, new Item("Artifact", ArtifactID));
    }

    public struct Item{
        public string name;
        public int id;

        public Item(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }

    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Inventory") == true)
        {
            UIInventory.SetActive(!inventoryActive);
            inventoryActive = !inventoryActive;
        }
    }

    //TODO: change this to add id
    public void AddArtifact()
    {
        inventory.Add(itemDictionary[ArtifactID]);
        GameObject newItem = Instantiate(Resources.Load("UIItemPanel", typeof(GameObject)) as GameObject) as GameObject;
        newItem.transform.SetParent(UIInventory.transform);
        newItem.transform.GetChild(0).GetComponent<Text>().text = "Artifact";
    }

    public void SetUIInventory(GameObject UIInventory)
    {
        this.UIInventory = UIInventory;
    }
}
