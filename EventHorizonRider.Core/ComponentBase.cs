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
 
        protected ComponentBase(params ComponentBase[] components)
        {
            if (components.Length > 0)
            {
                children = new List<ComponentBase>(components);
            }
        }

        protected void AddChild(ComponentBase component)
        {
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
                children.Remove(component);
            }
        }

        protected void ClearChildren()
        {
            if (children != null)
            {
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

        protected virtual void OnBeforeDraw(SpriteBatch spriteBatch)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            OnBeforeDraw(spriteBatch);

            foreach (var child in Children)
            {
                child.Draw(spriteBatch);
            }

            DrawCore(spriteBatch);

            OnAfterDraw(spriteBatch);
        }

        protected virtual void OnAfterDraw(SpriteBatch spriteBatch)
        {
        }

        protected virtual void DrawCore(SpriteBatch spriteBatch)
        {
        }
    }
}