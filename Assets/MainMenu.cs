using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static readonly string[] ItemSprites = { "menu_start", "menu_options", "menu_exit" };
    private const float ReferenceHeight = 1024f;
    private const float TextLeftX = -160f;
    private const float ArrowGap = 20f;

    private static readonly Color NormalColor = Color.white;
    private static readonly Color SelectedColor = new Color(1f, 0.85f, 0.2f);

    private GameObject _canvasObject;
    private Image[] _itemImages;
    private RectTransform _arrowRect;
    private int _selectedIndex;
    private bool _isOpen = true;

    private void Awake()
    {
        Time.timeScale = 0f;
        Build();
        UpdateSelection();
    }

    private void Update()
    {
        if (!_isOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            _selectedIndex = (_selectedIndex - 1 + ItemSprites.Length) % ItemSprites.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            _selectedIndex = (_selectedIndex + 1) % ItemSprites.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            Confirm();
        }
    }

    private void Confirm()
    {
        if (_selectedIndex == 0)
        {
            _isOpen = false;
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
        else if (_selectedIndex == 2)
        {
            Application.Quit();
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < _itemImages.Length; i++)
        {
            _itemImages[i].color = i == _selectedIndex ? SelectedColor : NormalColor;
        }

        var target = _itemImages[_selectedIndex].rectTransform;
        _arrowRect.anchoredPosition = new Vector2(TextLeftX - ArrowGap, target.anchoredPosition.y);
    }

    private void Build()
    {
        _canvasObject = new GameObject("MainMenuCanvas");
        _canvasObject.transform.SetParent(transform, false);
        var canvas = _canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = _canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1536f, 1024f);
        scaler.matchWidthOrHeight = 1f;

        // Matching by height keeps the canvas at exactly 1024 reference-units tall on
        // any screen, so the vertical layout below is always correct. The background
        // images are centered at their native aspect ratio (never stretched) instead
        // of forced to fill the width, so on wider-than-1.5 screens they simply show
        // black bars at the sides rather than distorting.
        float referenceHeight = ReferenceHeight;

        CreateFullScreenBackground(Color.black);

        var topSprite = Resources.Load<Sprite>("splash_top");
        var bottomSprite = Resources.Load<Sprite>("splash_bottom");
        var arrowSprite = Resources.Load<Sprite>("splash_arrow");

        CreateCenteredImage("Top", topSprite, new Vector2(0.5f, 1f), topSprite.rect.height);
        CreateCenteredImage("Bottom", bottomSprite, new Vector2(0.5f, 0f), bottomSprite.rect.height);

        // Mountain peaks sit above the bottom image, flush with its left/right edges
        // (not the screen edges), clear of the old baked menu text that used to
        // occupy the center column.
        float bottomHalfWidth = bottomSprite.rect.width / 2f;
        var peakLeftSprite = Resources.Load<Sprite>("peak_left");
        var peakRightSprite = Resources.Load<Sprite>("peak_right");
        CreateCornerImage("PeakLeft", peakLeftSprite, -bottomHalfWidth, 1f, bottomSprite.rect.height);
        CreateCornerImage("PeakRight", peakRightSprite, bottomHalfWidth, 0f, bottomSprite.rect.height);

        // Lay the menu items out in the gap between the top and bottom images.
        // Spacing is measured edge-to-edge (not center-to-center) so the visual
        // gap stays even even though each cropped word has a different height.
        const float ItemGap = 25f;

        float gapTopY = topSprite.rect.height;
        float gapBottomY = referenceHeight - bottomSprite.rect.height;

        var itemSprites = new Sprite[ItemSprites.Length];
        float totalContentHeight = 0f;
        for (int i = 0; i < ItemSprites.Length; i++)
        {
            itemSprites[i] = Resources.Load<Sprite>(ItemSprites[i]);
            totalContentHeight += itemSprites[i].rect.height;
        }
        totalContentHeight += ItemGap * (ItemSprites.Length - 1);

        float cursorTopY = gapTopY + (gapBottomY - gapTopY - totalContentHeight) / 2f;

        _itemImages = new Image[ItemSprites.Length];

        for (int i = 0; i < ItemSprites.Length; i++)
        {
            var itemObject = new GameObject("Item_" + ItemSprites[i]);
            itemObject.transform.SetParent(_canvasObject.transform, false);

            var image = itemObject.AddComponent<Image>();
            image.sprite = itemSprites[i];
            image.SetNativeSize();

            float itemHeight = itemSprites[i].rect.height;
            float centerTopY = cursorTopY + itemHeight / 2f;
            float centerY = referenceHeight / 2f - centerTopY;

            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(TextLeftX, centerY);

            _itemImages[i] = image;
            cursorTopY += itemHeight + ItemGap;
        }

        var arrowObject = new GameObject("Arrow");
        arrowObject.transform.SetParent(_canvasObject.transform, false);
        var arrowImage = arrowObject.AddComponent<Image>();
        arrowImage.sprite = arrowSprite;
        arrowImage.preserveAspect = true;

        _arrowRect = arrowImage.rectTransform;
        _arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        _arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        _arrowRect.pivot = new Vector2(1f, 0.5f);
        _arrowRect.sizeDelta = new Vector2(40f, 30f);
    }

    private void CreateFullScreenBackground(Color color)
    {
        var obj = new GameObject("Backdrop");
        obj.transform.SetParent(_canvasObject.transform, false);

        var image = obj.AddComponent<Image>();
        image.color = color;

        var rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void CreateCenteredImage(string name, Sprite sprite, Vector2 anchor, float height)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(_canvasObject.transform, false);

        var image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;

        float aspect = sprite.rect.width / sprite.rect.height;

        var rect = image.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.sizeDelta = new Vector2(height * aspect, height);
        rect.anchoredPosition = Vector2.zero;
    }

    private void CreateCornerImage(string name, Sprite sprite, float anchoredX, float pivotX, float bottomImageHeight)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(_canvasObject.transform, false);

        var image = obj.AddComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();

        var rect = image.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(pivotX, 0f);
        rect.anchoredPosition = new Vector2(anchoredX, bottomImageHeight);
    }
}
