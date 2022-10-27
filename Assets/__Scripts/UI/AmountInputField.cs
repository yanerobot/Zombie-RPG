using UnityEngine;
using System;
using TMPro;
using System.Text;

namespace KK
{
    public class AmountInputField : MonoBehaviour
    {
        public TMP_InputField inputField;
        public TextMeshProUGUI textField;
        public TextMeshProUGUI maxAmountField;
        int totalAmount;
        int currentAmount = 1;

        public bool IsInteracting { get; private set; }

        void Awake()
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
            inputField.onSelect.AddListener(_ => IsInteracting = true);
            inputField.onDeselect.AddListener(_ => IsInteracting = false);
            gameObject.SetActive(false);
        }


        void OnDestroy()
        {
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onSelect.RemoveAllListeners();
            inputField.onDeselect.RemoveAllListeners();
        }

        public void Init(int amount)
        {
            if (amount < 2)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            totalAmount = amount;
            currentAmount = 1;
            maxAmountField.text = "/" + totalAmount;
            UpdateInputField();
        }


        #region buttonCallbacks
        public void Add()
        {
            if (currentAmount + 1 > totalAmount)
                return;

            currentAmount += 1;
            UpdateInputField();
        }

        public void Remove()
        {
            if (currentAmount - 1 < 1)
                return;

            currentAmount -= 1;
            UpdateInputField();
        }

        public void SetMax()
        {
            currentAmount = totalAmount;
            UpdateInputField();
        }

        public void SetMin()
        {
            currentAmount = 1;
            UpdateInputField();
        }
        #endregion

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public int GetInputValue()
        {
            var result = currentAmount;
            int.TryParse(maxAmountField.text.Substring(1), out var maxAmountValue);
            maxAmountField.text = $"/{maxAmountValue - currentAmount}";
            if (currentAmount > maxAmountValue - currentAmount)
            {
                currentAmount = maxAmountValue - currentAmount;
                UpdateInputField();
            }

            return result;
        }

        public void OnDropdownClose()
        {
            currentAmount = 1;
            gameObject.SetActive(false);
        }

        void OnSelect(string input)
        {
            IsInteracting = true;
        }

        void OnDeselect(string input)
        {
            IsInteracting = false;
        }
        void OnValueChanged(string input)
        {
            if (input == "")
                input = "1";

            if (!int.TryParse(input, out var converted))
                return;

            if (converted > totalAmount)
            {
                int.TryParse(input.Substring(1), out converted);
            }

            converted = Mathf.Clamp(converted, 1, totalAmount);

            currentAmount = converted;
            UpdateInputField();
        }

        void UpdateInputField()
        {
            inputField.text = currentAmount.ToString();
        }
    }
}
