using System;
using EventHorizonRider.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Physics
{
    /// <summary>
    /// Collision detection helper methods mostly from:
    /// https://github.com/CartBlanche/MonoGame-Samples/blob/master/TransformedCollisionSample/Game1.cs
    /// </summary>
    internal class CollisionDetection
    {
        public static CollisionInfo GetCollisionInfo(Texture2D texture, byte alphaThreshold = 255)
        {
            var data = TextureProcessor.GetAlphaData(texture);

            var bounds = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (data[x, y] >= alphaThreshold)
                    {
                        bounds.X = Math.Min(bounds.X, x - 1);
                        bounds.Y = Math.Min(bounds.Y, y - 1);
                        bounds.Width = Math.Max(bounds.Width, ((x + 1) - bounds.X) + 1);
                        bounds.Height = Math.Max(bounds.Height, ((y + 1) - bounds.Y) + 1);
                    }
                }
            }

            var croppedData = TextureProcessor.GetCroppedData(data, bounds);

            return new CollisionInfo(croppedData, new Vector2(bounds.X, bounds.Y));
        }

        public static bool Collides(ISpriteInfo sprite1, ISpriteInfo sprite2, byte tolerance = 255)
        {
            var bounds1 = GetBoundingRectangle(sprite1);
            var bounds2 = GetBoundingRectangle(sprite2);

            if (bounds1.Intersects(bounds2))
            {
                if (IntersectPixels(
                        GetTransform(sprite1),
                        sprite1.CollisionInfo.PixelData.Width,
                        sprite1.CollisionInfo.PixelData.Height,
                        sprite1.CollisionInfo.PixelData.Data,
                        GetTransform(sprite2),
                        sprite2.CollisionInfo.PixelData.Width,
                        sprite2.CollisionInfo.PixelData.Height,
                        sprite2.CollisionInfo.PixelData.Data,
                        tolerance))
                {
                    return true;
                }
            }

            return false;
        }

        private static Matrix GetTransform(ISpriteInfo spriteInfo)
        {
            return
                Matrix.CreateTranslation(new Vector3(-(spriteInfo.Origin - spriteInfo.CollisionInfo.Offset), 0.0f)) *
                Matrix.CreateScale(new Vector3(spriteInfo.Scale, 1.0f)) *
                Matrix.CreateRotationZ(spriteInfo.Rotation) *
                Matrix.CreateTranslation(new Vector3(spriteInfo.Position, 0.0f));
        }

        private static Rectangle GetBoundingRectangle(ISpriteInfo spriteInfo)
        {
            return CalculateBoundingRectangle(
                spriteInfo.CollisionInfo.Bounds,
                GetTransform(spriteInfo));
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <param name="tolerance">Tolerance of alpha channel before considering it a collision</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        private static bool IntersectPixels(
            Matrix transformA, 
            int widthA, 
            int heightA, 
            byte[] dataA,
            Matrix transformB, 
            int widthB, 
            int heightB, 
            byte[] dataB, 
            byte tolerance)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAtoB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAtoB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAtoB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAtoB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Low at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        byte colorA = dataA[xA + yA * widthA];
                        byte colorB = dataB[xB + yB * widthB];

                        // If both pixels are completely opaque,
                        if (colorA >= tolerance && colorB >= tolerance)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        private static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            var leftTop = new Vector2(rectangle.Left, rectangle.Top);
            var rightTop = new Vector2(rectangle.Right, rectangle.Top);
            var leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            var rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
