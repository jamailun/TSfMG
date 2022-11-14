using UnityEngine;

public abstract class ItemType : ScriptableObject {

	[SerializeField] private new string name;
	public string Name => name;

	[SerializeField] private string _description;
	public string Description => _description;

	[SerializeField] private int _tier;
	public int Tier => _tier;


}