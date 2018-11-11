using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Core
{
    //http://xboxforums.create.msdn.com/forums/t/34356.aspx
    //public class Ray2D
    //{
    //    private Vector2 startPos;
    //    private Vector2 endPos;

    //    public Ray2D(Vector2 startPos, Vector2 endPos)
    //    {
    //        this.startPos = startPos;
    //        this.endPos = endPos;
    //    }

    //    /// <summary> 
    //    /// Determine if the ray intersects the rectangle 
    //    /// </summary> 
    //    /// <param name="rectangle">Rectangle to check</param> 
    //    /// <returns></returns> 
    //    public Vector2 Intersects(Rectangle rectangle)
    //    {
    //        Point p0 = new Point((int)startPos.X, (int)startPos.Y);
    //        Point p1 = new Point((int)endPos.X, (int)endPos.Y);

    //        foreach (Point testPoint in BresenhamLine(p0, p1))
    //        {
    //            if (rectangle.Contains(testPoint))
    //                return new Vector2((float)testPoint.X, (float)testPoint.Y);
    //        }

    //        return Vector2.Zero;
    //    }

    //    // Swap the values of A and B 

    //    private void Swap<T>(ref T a, ref T b)
    //    {
    //        T c = a;
    //        a = b;
    //        b = c;
    //    }

    //    // Returns the list of points from p0 to p1  

    //    private List<Point> BresenhamLine(Point p0, Point p1)
    //    {
    //        return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
    //    }

    //    // Returns the list of points from (x0, y0) to (x1, y1) 

    //    private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
    //    {
    //        // Optimization: it would be preferable to calculate in 
    //        // advance the size of "result" and to use a fixed-size array 
    //        // instead of a list. 

    //        List<Point> result = new List<Point>();

    //        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
    //        if (steep)
    //        {
    //            Swap(ref x0, ref y0);
    //            Swap(ref x1, ref y1);
    //        }
    //        if (x0 > x1)
    //        {
    //            Swap(ref x0, ref x1);
    //            Swap(ref y0, ref y1);
    //        }

    //        int deltax = x1 - x0;
    //        int deltay = Math.Abs(y1 - y0);
    //        int error = 0;
    //        int ystep;
    //        int y = y0;
    //        if (y0 < y1) ystep = 1; else ystep = -1;
    //        for (int x = x0; x <= x1; x++)
    //        {
    //            if (steep) result.Add(new Point(y, x));
    //            else result.Add(new Point(x, y));
    //            error += deltay;
    //            if (2 * error >= deltax)
    //            {
    //                y += ystep;
    //                error -= deltax;
    //            }
    //        }

    //        return result;
    //    }
    //    //http://xboxforums.create.msdn.com/forums/t/34356.aspx
    //}
    public class MathUtils
    {

        public static vec2 DecomposeRotation(float r)
        {
            //Turn a rotation into a vector (for chars mostly)
            float r2 = r - (float)Math.PI * 0.5f;
            vec2 dxy = new vec2(
                (float)Math.Cos(r2),
                (float)Math.Sin(r2)
                );

            return dxy;
        }


        public static float GetRotationFromLine(float x, float y, float x2, float y2)
        {
            //https://stackoverflow.com/questions/270138/how-do-i-draw-lines-using-xna
            //this returns the angle between two points in radians 
            float adj = x - x2;
            float opp = y - y2;
            float tan = opp / adj;
            float res = MathHelper.ToDegrees((float)Math.Atan2(opp, adj));
            res = (res - 180) % 360;
            if (res < 0) { res += 360; }
            res = MathHelper.ToRadians(res);
            return res;
        }

        //http://xboxforums.create.msdn.com/forums/t/34356.aspx
        public static float? RayIntersect(Box2f box, Ray ray)
        {
            float num = 0f;
            float maxValue = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if ((ray.Position.X < box.Min.x) || (ray.Position.X > box.Max.x))
                {
                    return null;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.X;
                float num8 = (box.Min.x - ray.Position.X) * num11;
                float num7 = (box.Max.x - ray.Position.X) * num11;
                if (num8 > num7)
                {
                    float num14 = num8;
                    num8 = num7;
                    num7 = num14;
                }
                num = MathHelper.Max(num8, num);
                maxValue = MathHelper.Min(num7, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if ((ray.Position.Y < box.Min.y) || (ray.Position.Y > box.Max.y))
                {
                    return null;
                }
            }
            else
            {
                float num10 = 1f / ray.Direction.Y;
                float num6 = (box.Min.y - ray.Position.Y) * num10;
                float num5 = (box.Max.y - ray.Position.Y) * num10;
                if (num6 > num5)
                {
                    float num13 = num6;
                    num6 = num5;
                    num5 = num13;
                }
                num = MathHelper.Max(num6, num);
                maxValue = MathHelper.Min(num5, maxValue);
                if (num > maxValue)
                {
                    return null;
                }
            }

            return new float?(num);
        }
    }
    public class Plane2f
    {
        public Plane2f() { }
        public float D;
        public vec2 N;
        public Plane2f(vec2 n, vec2 pt)
        {
            D = -n.Dot(pt);
            N = n;
        }
        public float IntersectLine(vec2 p1, vec2 p2)
        {
            float t = -(N.Dot(p1) + D) / ((p2 - p1).Dot(N));
            return t;
        }
    }
    public class vec2EqualityComparer : IEqualityComparer<vec2>
    {
        public bool Equals(vec2 x, vec2 y)
        {
            return x.x == y.x && x.y == y.y;
        }

        public int GetHashCode(vec2 x)
        {
            return x.x.GetHashCode() + x.y.GetHashCode();
        }
    }
    class mat2
    {
        //0 1
        //2 3
        public static int m00 = 0;
        public static int m01 = 1;
        public static int m10 = 2;
        public static int m11 = 3;
        public float[] m = new float[4];

        public static vec2 operator *(mat2 a, vec2 b)
        {
            vec2 r = new vec2(0, 0);

            r.x = a.m[0] * b.x + a.m[1] * b.y;
            r.y = a.m[2] * b.x + a.m[3] * b.y;
            return r;
        }
        public static mat2 GetRot(float theta)
        {
            mat2 ret = new mat2();

            //This is an incorrect rotation function - sin 10 shouldn't be negative.
            ret.m[m00] = (float)Math.Cos(theta);
            ret.m[m10] = (float)Math.Sin(theta);
            ret.m[m01] = -(float)Math.Sin(theta);
            ret.m[m11] = (float)Math.Cos(theta);

            return ret;
        }

    }
    public class ivec2EqualityComparer : IEqualityComparer<ivec2>
    {
        public bool Equals(ivec2 x, ivec2 y)
        {
            return x.x == y.x && x.y == y.y;
        }

        public int GetHashCode(ivec2 x)
        {
            return x.x.GetHashCode() + x.y.GetHashCode();
        }
    }
    public struct vec2
    {
        public vec2(Point p)
        {
            x = (float)p.X;
            y = (float)p.Y;
        }
        public float x, y;
        public void Construct(float a, float b) { x = a; y = b; }
        //public vec2() { }
        public vec2(vec2 dxy) { x = dxy.x; y = dxy.y; }
        public vec2(float dx, float dy) { x = dx; y = dy; }
        public vec2(Vector2 v) { x = v.X; y = v.Y; }//From XNA's Vector2
        public float Len() { return (float)Math.Sqrt((x * x) + (y * y)); }

        public vec2 Perp()
        {
            //Perpendicular
            return new vec2(y, -x);
        }
        public void Normalize()
        {
            float l = Len();
            if (l != 0)
            {
                x /= l;
                y /= l;
            }
            else
            {
                x = 0; y = 0;
            }

        }
        public vec2 Normalized()
        {
            vec2 v = new vec2(this);
            v.Normalize();
            return v;

        }
        public float Len2() { return Dot(this, this); }
        public Vector2 toXNA() { return new Vector2(x, y); }


        static public implicit operator vec2(float f)
        {
            return new vec2(f, f);
        }
        //public static vec2 operator =(vec2 a, float f)
        //{
        //    return new vec2(f, f);
        //}
        public static float Dot(vec2 a, vec2 b)
        {
            return (a.x * b.x) + (a.y * b.y);
        }
        public float Dot(vec2 b)
        {
            return (x * b.x) + (y * b.y);
        }
        public static vec2 operator -(vec2 d)
        {
            return new vec2(-d.x, -d.y);
        }
        public static vec2 operator +(vec2 a, vec2 b)
        {
            return new vec2(a.x + b.x, a.y + b.y);
        }
        public static vec2 operator -(vec2 a, vec2 b)
        {
            return new vec2(a.x - b.x, a.y - b.y);
        }
        public static vec2 operator *(vec2 a, float b)
        {
            return new vec2(a.x * b, a.y * b);
        }
        public static vec2 operator *(vec2 a, vec2 b)
        {
            return new vec2(a.x * b.x, a.y * b.y);
        }
        public static vec2 operator /(vec2 a, float b)
        {
            return new vec2(a.x / b, a.y / b);
        }
        public static vec2 operator -(vec2 a, float f)
        {
            return new vec2(a.x - f, a.y - f);
        }
        public static vec2 Minv(vec2 a, vec2 b)
        {
            vec2 ret = new vec2();
            ret.x = (float)Math.Min(a.x, b.x);
            ret.y = (float)Math.Min(a.y, b.y);

            return ret;
        }
        public static vec2 Maxv(vec2 a, vec2 b)
        {
            vec2 ret = new vec2();
            ret.x = (float)Math.Max(a.x, b.x);
            ret.y = (float)Math.Max(a.y, b.y);
            return ret;
        }

    }

    public struct ivec2
    {
        public ivec2(int dx, int dy) { x = dx; y = dy; }
        public int x { get; set; }
        public int y { get; set; }
        static public implicit operator ivec2(int f)
        {
            return new ivec2(f, f);
        }
        public static ivec2 operator -(ivec2 d)
        {
            return new ivec2(-d.x, -d.y);
        }
        public static ivec2 operator +(ivec2 a, ivec2 b)
        {
            return new ivec2(a.x + b.x, a.y + b.y);
        }
        public static ivec2 operator -(ivec2 a, ivec2 b)
        {
            return new ivec2(a.x - b.x, a.y - b.y);
        }
        public static ivec2 operator *(ivec2 a, int b)
        {
            return new ivec2(a.x * b, a.y * b);
        }
        public static ivec2 operator *(ivec2 a, ivec2 b)
        {
            return new ivec2(a.x * b.x, a.y * b.y);
        }
        public static ivec2 operator /(ivec2 a, int b)
        {
            return new ivec2(a.x / b, a.y / b);
        }
        public static ivec2 operator -(ivec2 a, int f)
        {
            return new ivec2(a.x - f, a.y - f);
        }
    }
    public struct vec4
    {
        public float x, y, z, w;

        public vec4(vec4 dxy) { x = dxy.x; y = dxy.y; z = dxy.z; w = dxy.w; }
        public vec4(float dx, float dy, float dz, float dw) { x = dx; y = dy; z = dz; w = dw; }
        public vec4(Vector4 v) { x = v.X; y = v.Y; z = v.Z; w = v.W; }//From XNA's Vector2

        public static vec4 Clamp(vec4 v, float a, float b)
        {
            vec4 ret = new vec4();
            ret.x = Globals.Clamp(v.x, a, b);
            ret.y = Globals.Clamp(v.y, a, b);
            ret.z = Globals.Clamp(v.z, a, b);
            ret.w = Globals.Clamp(v.w, a, b);
            return ret;
        }
        public void Clamp(float a, float b)
        {
            this = Clamp(this, a, b);
        }
        public void SetMinLightValue(float val)
        {
            //Make sure there's enough light for this.
            //Val = the minimum amount of light.
            //This isn't perfect
            float tot = x + y + z;
            if (tot < val)
            {
                float add = (2 - tot) / val;
                x += add;
                y += add;
                z += add;
                x = Globals.Clamp(x, 0, 1);
                y = Globals.Clamp(y, 0, 1);
                z = Globals.Clamp(z, 0, 1);
            }

        }

        public Vector4 toXNA() { return new Vector4(x, y, z, w); }
        public Color toXNAColor() { return new Color(toXNA()); }
        public static vec4 operator -(vec4 d)
        {
            return new vec4(-d.x, -d.y, -d.z, -d.w);
        }

        public static vec4 operator +(vec4 a, vec4 b)
        {
            return new vec4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }
        public static vec4 operator -(vec4 a, vec4 b)
        {
            return new vec4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }
        public static vec4 operator *(vec4 a, vec4 b)
        {
            return new vec4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }
        public static vec4 operator *(vec4 a, float b)
        {
            return new vec4(a.x * b, a.y * b, a.z * b, a.w * b);
        }
        public static vec4 operator /(vec4 a, float b)
        {
            return new vec4(a.x / b, a.y / b, a.z / b, a.w / b);
        }
        public static vec4 Minv(vec4 a, vec4 b)
        {
            vec4 ret = new vec4();
            ret.x = (float)Math.Min(a.x, b.x);
            ret.y = (float)Math.Min(a.y, b.y);
            ret.z = (float)Math.Min(a.z, b.z);
            ret.w = (float)Math.Min(a.w, b.w);
            return ret;
        }
        public static vec4 Maxv(vec4 a, vec4 b)
        {
            vec4 ret = new vec4();
            ret.x = (float)Math.Max(a.x, b.x);
            ret.y = (float)Math.Max(a.y, b.y);
            ret.z = (float)Math.Max(a.z, b.z);
            ret.w = (float)Math.Max(a.w, b.w);
            return ret;
        }

    }
    public struct RaycastHit
    {
        public bool _bHit;    // Whether the ray intersected the box.
        public bool _p1Contained;
        public bool _p2Contained;
        public float _t; // - Time to hit [0,1]
                         //  public void* _pPickData; // picked object (BvhObject3*)
        public vec2 _vNormal; //The normal of the plane the raycast hit.
                              //Do not include ray data for optimization.

        //public RaycastHit()
        //{
        //    reset();
        //}
        public bool trySetClosestHit(ref float closest_t)
        {
            //Easy way of iterating a closest hit.
            if (_bHit && (_t < closest_t))
            {
                closest_t = _t;
                return true;
            }
            return false;
        }
        public void reset()
        {
            _bHit = false;
            _p1Contained = false;
            _p2Contained = false;
            _t = float.MaxValue;
            //  _pPickData = NULL;
        }
        public void copyFrom(RaycastHit bh)
        {
            _bHit = bh._bHit;
            _p1Contained = bh._p1Contained;
            _p2Contained = bh._p2Contained;
            _t = bh._t;
        }
    }
    public struct ProjectedRay
    {
        public vec2 Origin;
        public vec2 Dir;
        public float _t;
        public vec2 _vNormal;

        // Found the following two cool optimizations on WIlliams et. al (U. Utah)
        public vec2 InvDir;
        public int[] Sign;

        public bool IsOpt { get; private set; }    // - return true if  we optimized this

        public float Length;// Max length

        public vec2 Begin() { return Origin; }
        public vec2 End() { return Origin + Dir; }

        public ProjectedRay(vec2 origin, vec2 dir)
        {
            Sign = new int[2];
            Origin = origin;
            Dir = dir;

            IsOpt = false;
            Length = float.MaxValue;//Must be maxvalue
            _t = float.MaxValue;
            _vNormal = new vec2(0, 0);

            //opt()
            //            //**New - optimization
            //http://people.csail.mit.edu/amy/papers/box-jgt.pdf
            //Don't set to zero. We need infinity (or large value) here.
            InvDir.x = 1.0f / Dir.x;
            InvDir.y = 1.0f / Dir.y;

            Sign[0] = (InvDir.x < 0) ? 1 : 0;
            Sign[1] = (InvDir.y < 0) ? 1 : 0;

            IsOpt = true;
        }
        //public void opt()
        //{



        //}
        public bool isHit()
        {
            return _t >= 0.0f && _t <= 1.0f;
        }
        public vec2 HitPoint()
        {
            vec2 ret = Begin() + (End() - Begin()) * _t;
            return ret;
        }
    }

    public struct Box2f
    {
        public vec2 Min;
        public vec2 Max;

        public float Width() { return Max.x - Min.x; }
        public float Height() { return Max.y - Min.y; }

        public vec2 TopRight() { return new vec2(Max.x, Min.y); }
        public vec2 BotRight(){ return new vec2(Max.x, Max.y); }
        public vec2 BotLeft(){ return new vec2(Min.x, Max.y); }
        public vec2 TopLeft(){ return new vec2(Min.x, Min.y); }

        public void Construct(vec2 min, vec2 max)
        {
            Min = min; Max = max;
        }
        public Box2f(float x, float y, float w, float h)
        {
            Min = new vec2(x, y);
            Max = new vec2(w, h) + Min;
        }
        public Box2f(vec2 min, vec2 max)
        {
            Min = min;
            Max = max;
        }
        public vec2 Center()
        {
            return Min + (Max - Min) * 0.5f;
        }
        public static Box2f FlipBoxH(Box2f b, float w)
        {
            //Flip the box inside of a larger box (w)
            Box2f ret = new Box2f();
            ret.Min.x = w - b.Max.x;
            ret.Max.x = w - b.Min.x;

            ret.Min.y = b.Min.y;
            ret.Max.y = b.Max.y;
            return ret;
        }
        public static Box2f FlipBoxV(Box2f b, float h)
        {
            //Flip the box inside of a larger box (h)
            Box2f ret = new Box2f();
            ret.Min.y = h - b.Max.y;
            ret.Max.y = h - b.Min.y;

            ret.Min.x = b.Min.x;
            ret.Max.x = b.Max.x;
            return ret;
        }
        public Rectangle ToXNARect()
        {
            Rectangle r = new Rectangle();

            r.X = (int)(Min.x);
            r.Y = (int)(Min.y);
            r.Width = (int)(Max.x - Min.x);
            r.Height = (int)(Max.y - Min.y);

            return r;
        }

        public static Box2f GetIntersectionBox_Inclusive(Box2f a, Box2f b)
        {
            Box2f ret = new Box2f();

            ret.Min.x = Single.MaxValue;
            ret.Min.y = Single.MaxValue;
            ret.Max.x = -Single.MaxValue;
            ret.Max.y = -Single.MaxValue;


            if (a.Min.x >= b.Min.x && a.Min.x <= b.Max.x)
            {
                ret.Min.x = Math.Min(ret.Min.x, a.Min.x);
            }
            if (a.Max.x <= b.Max.x && a.Max.x >= b.Min.x)
            {
                ret.Max.x = Math.Max(ret.Max.x, a.Max.x);
            }
            if (a.Min.y >= b.Min.y && a.Min.y <= b.Max.y)
            {
                ret.Min.y = Math.Min(ret.Min.y, a.Min.y);
            }
            if (a.Max.y <= b.Max.y && a.Max.y >= b.Min.y)
            {
                ret.Max.y = Math.Max(ret.Max.y, a.Max.y);
            }

            if (b.Min.x >= a.Min.x && b.Min.x <= a.Max.x)
            {
                ret.Min.x = Math.Min(ret.Min.x, b.Min.x);
            }
            if (b.Max.x <= a.Max.x && b.Max.x >= a.Min.x)
            {
                ret.Max.x = Math.Max(ret.Max.x, b.Max.x);
            }
            if (b.Min.y >= a.Min.y && b.Min.y <= a.Max.y)
            {
                ret.Min.y = Math.Min(ret.Min.y, b.Min.y);
            }
            if (b.Max.y <= a.Max.y && b.Max.y >= a.Min.y)
            {
                ret.Max.y = Math.Max(ret.Max.y, b.Max.y);
            }
            return ret;
        }

        public void GenResetExtents()
        {
            Min = new vec2(Single.MaxValue, Single.MaxValue);
            Max = new vec2(-Single.MaxValue, -Single.MaxValue);
        }
        public void ExpandByPoint(vec2 v)
        {
            Min = vec2.Minv(Min, v);
            Max = vec2.Maxv(Max, v);
        }
        public bool BoxIntersect_EasyOut_Inclusive(Box2f cc)
        {
            return cc.Min.x <= Max.x && cc.Min.y <= Max.y && Min.x <= cc.Max.x && Min.y <= cc.Max.y;
        }
        public bool ContainsPointInclusive(vec2 point)
        {
            if (point.x < Min.x)
                return false;
            if (point.y < Min.y)
                return false;
            if (point.x > Max.x)
                return false;
            if (point.y > Max.y)
                return false;
            return true;
        }
        private vec2 bounds(int x)
        {
            if (x == 0) return Min;
            return Max;
        }
        public bool RayIntersect(ProjectedRay ray, ref RaycastHit bh)
        {
            if (ray.IsOpt == false)
            {
                //Error.
                System.Diagnostics.Debugger.Break();
            }

            float txmin, txmax, tymin, tymax;
            bool bHit;

            txmin = (bounds(ray.Sign[0]).x - ray.Origin.x) * ray.InvDir.x;
            txmax = (bounds(1 - ray.Sign[0]).x - ray.Origin.x) * ray.InvDir.x;

            tymin = (bounds(ray.Sign[1]).y - ray.Origin.y) * ray.InvDir.y;
            tymax = (bounds(1 - ray.Sign[1]).y - ray.Origin.y) * ray.InvDir.y;

            if ((txmin > tymax) || (tymin > txmax))
            {
               // if (bh != null)
               // {
                    bh._bHit = false;
               // }
                return false;
            }
            if (tymin > txmin)
            {
                txmin = tymin;
            }
            if (tymax < txmax)
            {
                txmax = tymax;
            }

            bHit = ((txmin >= 0.0f) && (txmax <= ray.Length));

            //**Note changed 20151105 - this is not [0,1] this is the lenth along the line in which 
            // the ray enters and exits the cube, so any value less than the maximum is valid

           // if (bh != null)
           // {
                bh._bHit = bHit;
                bh._t = txmin;
           // }

            return bHit;
        }
    }



}
