using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SafeArea : VisualElement
{
    [UxmlAttribute] public bool ApplyTop = true;
    [UxmlAttribute] public bool ApplyRight = true;
    [UxmlAttribute] public bool ApplyBottom = true;
    [UxmlAttribute] public bool ApplyLeft = true;

    [UxmlAttribute] public Length TopPadding;
    [UxmlAttribute] public Length RightPadding;
    [UxmlAttribute] public Length BottomPadding;
    [UxmlAttribute] public Length LeftPadding;

    IVisualElementScheduledItem _poll;
    const long PollInterval = 1000;

    public SafeArea()
    {
        style.flexGrow = 1f;
        RegisterCallback<AttachToPanelEvent>(OnAttach);
        RegisterCallback<DetachFromPanelEvent>(OnDetach);
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    void OnAttach(AttachToPanelEvent evt)
    {
        Apply();
        _poll = schedule.Execute(Apply).Every(PollInterval);
    }

    void OnDetach(DetachFromPanelEvent evt)
    {
        _poll?.Pause();
        _poll = null;
    }

    void OnGeometryChanged(GeometryChangedEvent evt)
    {
        Apply();
    }

    void Apply()
    {
        var root = panel?.visualTree;
        if (root == null)
            return;

        var panelWidth = root.resolvedStyle.width;
        if (panelWidth <= 0f || Screen.width <= 0)
            return;
        
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var unitsPerPixel = panelWidth / screenWidth;
        var safe = SafeAreaProvider.Current;
        var top = (screenHeight - safe.yMax) * unitsPerPixel;
        var right = (screenWidth - safe.xMax) * unitsPerPixel;
        var bottom = safe.yMin * unitsPerPixel;
        var left = safe.xMin * unitsPerPixel;

        var reference = resolvedStyle.width;
        style.paddingTop = (ApplyTop ? top : 0f) + Resolve(TopPadding, screenHeight);
        style.paddingRight = (ApplyRight ? right : 0f) + Resolve(RightPadding, reference);
        style.paddingBottom = (ApplyBottom ? bottom : 0f) + Resolve(BottomPadding, reference);
        style.paddingLeft = (ApplyLeft ? left : 0f) + Resolve(LeftPadding, reference);
    }

    static float Resolve(Length length, float reference)
    {
        if (length.unit == LengthUnit.Percent)
            return length.value * .01f * reference;
        return length.value;
    }
}