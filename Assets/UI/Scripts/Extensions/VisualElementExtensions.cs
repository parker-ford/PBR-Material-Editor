using UnityEngine;
using UnityEngine.UIElements;

namespace MaterialUI
{
    public static class VisualElementExtensions
    {
        //Referenced from: https://github.com/adammyhre/Unity-Inventory-System/blob/master/Assets/_Project/Scripts/Inventory/Extensions/VisualElementExtensions.cs
        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            VisualElement child = new VisualElement();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        public static T CreateChild<T>(this VisualElement parent, params string[] classes) where T : VisualElement, new()
        {
            var child = new T();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement
        {
            parent.Add(child);
            return child;
        }

        public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (string className in classes)
            {
                if (!string.IsNullOrEmpty(className))
                {
                    visualElement.AddToClassList(className);
                }
            }
            return visualElement;
        }
    }

}