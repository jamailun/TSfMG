using UnityEngine;
using UnityEngine.UI;

public class StatisticsWindowUI : WindowsUI {

	[SerializeField] private LayoutGroup container;
	[SerializeField] private InGameStatLine linePrefab;

	public override void OnHide() { }

	public override void OnShow() {
		// Clear
		container.transform.DestroyChildren();
		// fill
		var p = GlobalGameManager.Instance.GetPlayer();
		if(p == null) {
			Debug.LogError("pas de joueur...");
			return;
		}
		
		var c = p.Character;
		if(c == null) {
			Debug.LogError("pas de character...");
			return;
		}

		var stats = c.CurrentStatistics;
		foreach(var k in stats.Keys) {
			var line = Instantiate(linePrefab, container.transform);
			line.Set(k, stats[k]);
		}
	}
}