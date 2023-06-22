using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UtilLoader21341.Util
{
    public static class UtilTools
    {
        public static Button CreateButton(Transform parent, Sprite Image, Vector2 scale, Vector2 position)
        {
            var image = CreateImage(parent, Image, scale, position);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            return button;
        }

        public static Image CreateImage(Transform parent, Sprite Image, Vector2 scale, Vector2 position)
        {
            var gameObject = new GameObject("Image");
            var image = gameObject.AddComponent<Image>();
            image.transform.SetParent(parent);
            new Texture2D(2, 2); //??
            image.sprite = Image;
            image.rectTransform.sizeDelta = new Vector2(Image.texture.width, Image.texture.height);
            gameObject.SetActive(true);
            gameObject.transform.localScale = scale;
            gameObject.transform.localPosition = position;
            return image;
        }
    }

    public class ButtonColor : EventTrigger
    {
        public static Color OnEnterColor;

        public Color DefaultColor = new Color(1f, 1f, 1f);

        public Image Image;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            Image.color = OnEnterColor;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            Image.color = DefaultColor;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            Image.color = DefaultColor;
        }
    }

    public static class UIPanelTool
    {
        public static T GetUIPanel<T>(UIPanelType type) where T : UIPanel
        {
            return UI.UIController.Instance.GetUIPanel(type) as T;
        }

        public static UIEnemyCharacterListPanel GetEnemyCharacterListPanel()
        {
            return GetUIPanel<UIEnemyCharacterListPanel>(UIPanelType.CharacterList);
        }

        public static UILibrarianCharacterListPanel GetLibrarianCharacterListPanel()
        {
            return GetUIPanel<UILibrarianCharacterListPanel>(UIPanelType.CharacterList_Right);
        }
    }
}