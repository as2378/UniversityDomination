using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour {

    public enum ResourceType {Beer, Knowledge};
	[SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount = 2;

    
    public ResourceType GetResourceType() {
        return resourceType;
    }

    public void SetResourceType(ResourceType resourceType) {
        this.resourceType = resourceType;
    }

    public int GetAmount() {
        return amount;
    }

    public void SetAmount(int amount) {
        this.amount = amount;
    }
}
