using System.Threading;
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
        private static readonly List<ComponentBase> EmptyList = new List<ComponentBase>(); 
        private List<ComponentBase> children;

        protected float Depth { get; private set; }

        protected ComponentBase Parent { get; private set; }

        private bool visible = true;

        public bool Visible
        {
            get { return visible && (Parent == null || Parent.Visible); }
            set { visible = value; }
        }

        protected ComponentBase(params ComponentBase[] components)
        {
            Visible = true;

            if (components.Length > 0)
            {
                children = new List<ComponentBase>(components);
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
                foreach (var child in Children)
                {
                    child.Parent = null;
                }

                children.Clear();
            }
        }

        public IList<ComponentBase> Children
        {
            get { return children ?? EmptyList; }
        }

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            foreach (var child in Children)
            {
                child.LoadContent(content, graphics);
            }

            LoadContentCore(content, graphics);
        }

        protected virtual void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            foreach (var child in Children)
            {
                child.Update(gameTime, inputState);
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
            if (!Visible) return;

            OnBeforeDraw(spriteBatch, graphics);

            foreach (var child in Children)
            {
                child.Draw(spriteBatch, graphics);
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
    }
}