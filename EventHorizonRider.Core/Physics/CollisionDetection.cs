using System;
using Microsoft.Xna.Framework;

namespace EventHorizonRider.Core.Physics
{
    /// <summary>
    /// Collision detection helper methods mostly from:
    /// https://github.com/CartBlanche/MonoGame-Samples/blob/master/TransformedCollisionSample/Game1.cs
    /// </summary>
    internal class CollisionDetection
    {
        public static bool Collides(ISpriteInfo sprite1, ISpriteInfo sprite2, byte tolerance = 255)
        {
            var transform1 = GetTransform(sprite1);
            var transform2 = GetTransform(sprite2);

            var bounds1 = GetBoundingRectangle(sprite1);
            var bounds2 = GetBoundingRectangle(sprite2);

            if (bounds1.Intersects(bounds2))
            {
                if (IntersectPixels(
                        transform1,
                        sprite1.Texture.Width,
                        sprite1.Texture.Height,
                        sprite1.TextureAlphaData,
                        transform2,
                        sprite2.Texture.Width,
                        sprite2.Texture.Height,
                        sprite2.TextureAlphaData,
                        tolerance))
                {
                    return true;
                }
            }

            return false;
        }

        public static Matrix GetTransform(ISpriteInfo spriteInfo)
        {
            return
                Matrix.CreateTranslation(new Vector3(-spriteInfo.Origin, 0.0f)) *
                Matrix.CreateScale(new Vector3(spriteInfo.Scale, 1.0f)) *
                Matrix.CreateRotationZ(spriteInfo.Rotation) *
                Matrix.CreateTranslation(new Vector3(spriteInfo.Position, 0.0f));
        }

        public static Rectangle GetBoundingRectangle(ISpriteInfo spriteInfo)
        {
            return CalculateBoundingRectangle(
                spriteInfo.Texture.Bounds,
                GetTransform(spriteInfo));
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <param name="tolerance">Tolerance of alpha channel before considering it a collision</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(Rectangle rectangleA, byte[] dataA, Rectangle rectangleB, byte[] dataB, byte tolerance)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    byte colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    byte colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA >= tolerance && colorB >= tolerance)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
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
        public static bool IntersectPixels(
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
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
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
