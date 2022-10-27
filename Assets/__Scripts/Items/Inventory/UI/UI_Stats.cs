using UnityEngine;
using System;
using System.Collections;
using TMPro;

namespace KK.Items
{
    public class UI_Stats : MonoBehaviour
    {
        [Serializable]
        class StatValueField
        {
            public PlayerStats.StatType statType;
            public TextMeshProUGUI valueField;
        }

        [SerializeField] UI_Inventory ui_inventory;
        [SerializeField] StatValueField[] statTextFields;


        PlayerStats stats;

        IEnumerator Start()
        {
            while (stats == null)
            {
                if (ui_inventory.inventory != null)
                    ui_inventory.inventory.TryGetComponent(out stats);
                yield return new WaitForSeconds(0.1f);
            }

            if (stats == null)
                Debug.LogError("Stats not found on the player");

            DisplayStatInfo();

            yield return null;
        }

        public void DisplayStatInfo()
        {
            foreach (var field in statTextFields)
            {
                field.valueField.text = stats.GetValue(field.statType).ToString();
            }
        }
    }
}
