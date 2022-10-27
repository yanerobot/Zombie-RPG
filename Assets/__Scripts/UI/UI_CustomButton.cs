using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KK
{
    public class UI_CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Graphic targetGraphic;
        Image image;
        enum EventType { Click, Enter, Exit, Up }
        enum Modifier
        {
            Color,
            ImageSwitch,
            Outline
        }

        [SerializeField] bool selectable;
        [SerializeField] Modifier modifier;
        [SerializeField] Outline outline;
        [SerializeField, HideInInspector] Color normalColor, enteredColor, clickedColor, disabledColor, selectedColor;
        [SerializeField, HideInInspector] Sprite normalImage, enteredImage, clickedImage, disabledImage, selectedImage;
        [SerializeField] float duration;

        public UnityEvent onClick;

        Color previousColor;
        Sprite previousImage;

        bool isDisabled, isSelected;

        void Awake()
        {
            image = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SetGraphics(EventType.Click);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetGraphics(EventType.Enter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetGraphics(EventType.Exit);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SetGraphics(EventType.Click);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetGraphics(EventType.Up);
            onClick?.Invoke();
        }

        void SetGraphics(EventType type)
        {
            if (isDisabled)
                return;

            switch (type)
            {
                case EventType.Click:
                    if (selectable)
                    {
                        ApplyGraphics(selectedColor, selectedImage);
                        isSelected = true;
                    }
                    else
                        ApplyGraphics(clickedColor, clickedImage);
                    break;
                case EventType.Enter:
                    ApplyGraphics(enteredColor, enteredImage);
                    break;
                case EventType.Exit:
                    if (isSelected)
                        ApplyGraphics(selectedColor, selectedImage);
                    else
                        ApplyGraphics(normalColor, normalImage);
                    break;
                case EventType.Up:
                    if (isSelected)
                        ApplyGraphics(selectedColor, selectedImage);
                    else
                        ApplyGraphics(previousColor, previousImage);
                    break;
            }

            void ApplyGraphics(Color color, Sprite sprite)
            {
                switch (modifier)
                {
                    case Modifier.Color:
                        image.DOComplete();
                        previousColor = image.color;
                        image.DOColor(color, duration);
                        break;
                    case Modifier.ImageSwitch:
                        previousImage = image.sprite;
                        image.sprite = sprite;
                        break;
                    case Modifier.Outline:
                        outline.DOComplete();
                        previousColor = outline.effectColor;
                        outline.DOColor(color, duration);
                        break;
                }
            }
        }
        #region Editor
#if UNITY_EDITOR


        [CustomEditor(typeof(UI_CustomButton), true)]
        [CanEditMultipleObjects]
        public class UI_CustomButtonEditor : Editor
        {
            SerializedProperty selectable, modifier, outline, script;
            SerializedProperty normal, clicked, entered, disabled, selected;

            void OnEnable()
            {
                selectable = serializedObject.FindProperty("selectable");
                modifier = serializedObject.FindProperty("modifier");
                script = serializedObject.FindProperty("m_Script");
                outline = serializedObject.FindProperty("outline");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.ObjectField(script);

                GUILayout.Label("Custom Button", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                modifier.enumValueIndex = (int)(Modifier)EditorGUILayout.EnumPopup(modifier.displayName, (Modifier)modifier.enumValueIndex);
                selectable.boolValue = EditorGUILayout.Toggle(selectable.displayName, selectable.boolValue);

                switch ((Modifier)modifier.enumValueIndex)
                {
                    case Modifier.ImageSwitch:
                        normal = serializedObject.FindProperty("normalImage");
                        EditorGUILayout.ObjectField(normal);
                        entered = serializedObject.FindProperty("enteredImage");
                        EditorGUILayout.ObjectField(entered);
                        clicked = serializedObject.FindProperty("clickedImage");
                        EditorGUILayout.ObjectField(clicked);
                        disabled = serializedObject.FindProperty("disabledImage");
                        EditorGUILayout.ObjectField(disabled);

                        if (selectable.boolValue)
                        {
                            selected = serializedObject.FindProperty("selectedImage");
                            EditorGUILayout.ObjectField(selected);
                        }
                        break;
                    default:
                        if ((Modifier)modifier.enumValueIndex == Modifier.Outline)
                        {
                            EditorGUILayout.ObjectField(outline);
                        }
                        normal = serializedObject.FindProperty("normalColor");
                        normal.colorValue = EditorGUILayout.ColorField(normal.displayName, normal.colorValue);
                        entered = serializedObject.FindProperty("enteredColor");
                        entered.colorValue = EditorGUILayout.ColorField(entered.displayName, entered.colorValue);
                        clicked = serializedObject.FindProperty("clickedColor");
                        clicked.colorValue = EditorGUILayout.ColorField(clicked.displayName, clicked.colorValue);
                        disabled = serializedObject.FindProperty("disabledColor");
                        disabled.colorValue = EditorGUILayout.ColorField(disabled.displayName, disabled.colorValue);

                        if (selectable.boolValue)
                        {
                            selected = serializedObject.FindProperty("selectedColor");
                            selected.colorValue = EditorGUILayout.ColorField(selected.displayName, selected.colorValue);
                        }
                        break;
                }

                EditorGUILayout.Space();

                GUILayout.Label("Image", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                DrawPropertiesExcluding(serializedObject, script.name, selectable.name, modifier.name, outline.name);

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
        #endregion
    }
}
