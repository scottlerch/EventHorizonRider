using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EventHorizonRider.Core;

/// <summary>
/// Composite pattern of game components.
/// </summary>
internal abstract class ComponentBase
{
    private List<ComponentBase> _children;
    private bool _visible = true;
    private bool _updating = true;

    protected ComponentBase(params ComponentBase[] components)
    {
        Visible = true;

        if (components.Length > 0)
        {
            _children = [.. components];

            foreach (var child in _children)
            {
                child.Parent = this;
            }
        }

        var depthStep = 1f / (components.Length + 2);

        for (var i = 0; i < components.Length; i++)
        {
            _children[i].Depth = (i + 1) * depthStep;
        }
    }

    public float Depth { get; private set; }

    public bool Visible
    {
        get => _visible && (Parent == null || Parent.Visible);
        set
        {
            if (_visible != value)
            {
                _visible = value;
                OnVisibleChanged();
                ForEach<ComponentBase>(child => child.OnVisibleChanged());
            }
        }
    }

    public bool Updating
    {
        get => _updating && (Parent == null || Parent.Updating);
        set
        {
            if (_updating != value)
            {
                _updating = value;
                OnUpdatingChanged();
                ForEach<ComponentBase>(child => child.OnUpdatingChanged());
            }
        }
    }

    public bool ChildrenIsEmpty => _children == null || _children.Count == 0;

    public int ChildrenCount => _children == null ? 0 : _children.Count;

    protected ComponentBase Parent { get; private set; }

    protected void AddChild(ComponentBase component, float depth)
    {
        component.Depth = depth;
        component.Parent = this;

        _children ??= [];

        _children.Add(component);
    }

    protected void RemoveChild(ComponentBase component)
    {
        if (_children != null)
        {
            component.Parent = null;
            _children.Remove(component);
        }
    }

    protected void ClearChildren()
    {
        if (_children != null)
        {
            ForEach<ComponentBase>(child => child.Parent = null);
            _children.Clear();
        }
    }

    public IReadOnlyList<ComponentBase> Children => _children ?? (IReadOnlyList<ComponentBase>)Array.Empty<ComponentBase>();

    public void ForEach<T>(Action<T> action) where T : ComponentBase
    {
        if (_children == null)
        {
            return;
        }

        var count = _children.Count;

        for (var i = 0; i < count; i++)
        {
            action(_children[i] as T);
        }
    }

    public void ForEachReverse<T>(Action<T> action) where T : ComponentBase
    {
        if (_children == null)
        {
            return;
        }

        var lastIndex = _children.Count - 1;

        for (var i = lastIndex; i >= 0; i--)
        {
            action(_children[i] as T);
        }
    }

    public void LoadContent(ContentManager content, GraphicsDevice graphics)
    {
        ForEach<ComponentBase>(child => child.LoadContent(content, graphics));

        LoadContentCore(content, graphics);
    }

    protected virtual void LoadContentCore(ContentManager content, GraphicsDevice graphics)
    {
    }

    public void Update(GameTime gameTime, InputState inputState)
    {
        if (!Updating)
        {
            return;
        }

        // Hand-rolled loop instead of ForEach to avoid allocating a closure/delegate
        // every frame on this hot path (Update runs for the whole component tree at 60fps).
        if (_children != null)
        {
            var count = _children.Count;
            for (var i = 0; i < count; i++)
            {
                _children[i].Update(gameTime, inputState);
            }
        }

        UpdateCore(gameTime, inputState);
    }

    protected virtual void UpdateCore(GameTime gameTime, InputState inputState)
    {
    }

    protected virtual void OnBeforeDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
        if (!Visible)
        {
            return;
        }

        OnBeforeDraw(spriteBatch, graphics);

        // Hand-rolled loop instead of ForEach to avoid allocating a closure/delegate
        // every frame on this hot path (Draw runs for the whole component tree at 60fps).
        if (_children != null)
        {
            var count = _children.Count;
            for (var i = 0; i < count; i++)
            {
                _children[i].Draw(spriteBatch, graphics);
            }
        }

        DrawCore(spriteBatch);

        OnAfterDraw(spriteBatch, graphics);
    }

    protected virtual void OnAfterDraw(SpriteBatch spriteBatch, GraphicsDevice graphics)
    {
    }

    protected virtual void DrawCore(SpriteBatch spriteBatch)
    {
    }

    protected virtual void OnUpdatingChanged()
    {
    }

    protected virtual void OnVisibleChanged()
    {
    }
}
