using System;
using System.Linq;
using EventHorizonRider.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EventHorizonRider.Core
{
    /// <summary>
    /// Composite pattern of game components.
    /// </summary>
    internal abstract class ComponentBase
    {
        private List<ComponentBase> children;

        public float Depth { get; private set; }

        protected ComponentBase Parent { get; private set; }

        private bool visible = true;

        public bool Visible
        {
            get { return visible && (Parent == null || Parent.Visible); }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    OnVisibleChanged();
                    ForEach<ComponentBase>(child => child.OnVisibleChanged());
                }
            }
        }

        private bool updating = true;

        public bool Updating
        {
            get { return updating && (Parent == null || Parent.Updating); }
            set
            {
                if (updating != value)
                {
                    updating = value;
                    OnUpdatingChanged();
                    ForEach<ComponentBase>(child => child.OnUpdatingChanged());
                }
            }
        }

        protected ComponentBase(params ComponentBase[] components)
        {
            Visible = true;

            if (components.Length > 0)
            {
                children = new List<ComponentBase>(components);

                foreach (var child in children)
                {
                    child.Parent = this;
                }
            }

            var depthStep = 1f/(components.Length + 2);

            for (int i = 0; i < components.Length; i++)
            {
                children[i].Depth = (i + 1)*depthStep;
            }
        }

        protected void AddChild(ComponentBase component, float depth)
        {
            component.Depth = depth;
            component.Parent = this;

            if (children == null)
            {
                children = new List<ComponentBase>();    
            }

            children.Add(component);
        }

        protected void RemoveChild(ComponentBase component)
        {
            if (children != null)
            {
                component.Parent = null;
                children.Remove(component);
            }
        }

        protected void ClearChildren()
        {
            if (children != null)
            {
                ForEach<ComponentBase>(child => child.Parent = null);
                children.Clear();
            }
        }

        public IEnumerable<ComponentBase> Children
        {
            get { return children ?? Enumerable.Empty<ComponentBase>(); }
        }

        public bool ChildrenIsEmpty { get { return children == null || children.Count == 0; } }

        public int ChildrenCount { get { return children == null ? 0 : children.Count; } }

        public void ForEach<T>(Action<T> action) where T : ComponentBase
        {
            if (children == null) return;

            var count = children.Count;

            for (int i = 0; i < count; i++)
            {
                action(children[i] as T);
            }
        }

        public void ForEachReverse<T>(Action<T> action) where T : ComponentBase
        {
            if (children == null) return;

            var lastIndex = children.Count - 1;

            for (int i = lastIndex; i >= 0; i--)
            {
                action(children[i] as T);
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
            if (!Updating) return;

            ForEach<ComponentBase>(child => child.Update(gameTime, inputState));

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
            if (!Visible) return;

            OnBeforeDraw(spriteBatch, graphics);

            ForEach<ComponentBase>(child => child.Draw(spriteBatch, graphics));

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
}