using System;

namespace Core.Math
{
	public struct MathfInternal
	{
		public static volatile float FloatMinNormal = 1.17549435E-38f;
		public static volatile float FloatMinDenormal = 1.401298E-45f;
		public static readonly bool IS_FLUSH_TO_ZERO_ENABLED = FloatMinDenormal == 0f;
	}

	public static class MathUtils
	{
		/// <summary>
		///     Tolerance value.
		/// </summary>
		public static readonly float EPSILON = ( !MathfInternal.IS_FLUSH_TO_ZERO_ENABLED ) ? MathfInternal.FloatMinDenormal : MathfInternal.FloatMinNormal;

		public static readonly float MAX_VALUE = float.MaxValue;
		public static readonly float MIN_VALUE = float.MinValue;

		/// <summary>
		///     Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
		/// </summary>
		public const float PI = 3.1415926535897932384626433832795f;

		/// <summary>
		///     Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, multiplied by
		///     2.
		/// </summary>
		public const float PI2 = 6.283185307179586476925286766559f;

		/// <summary>
		///     Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, multiplied by
		///     4.
		/// </summary>
		public const float PI4 = 12.566370614359172953850573533118f;

		/// <summary>
		///     Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 2.
		/// </summary>
		public const float PI_HALF = 1.5707963267948966192313216916398f;

		/// <summary>
		///     Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 4.
		/// </summary>
		public const float PI_QUARTER = 0.78539816339744830961566084581988f;

		/// <summary>
		///     Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π.
		/// </summary>
		public const float PI_DELTA = 0.31830988618379067153776752674503f;

		/// <summary>
		///     Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π,
		///     divided by 2.
		/// </summary>
		public const float PI_HALF_DELTA = 0.63661977236758134307553505349004f;

		/// <summary>
		///     Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π,
		///     divided by 4.
		/// </summary>
		public const float PI_QUARTER_DELTA = 1.2732395447351626861510701069801f;

		public const float DEG_TO_RAD = PI / 180f;

		public const float RAD_TO_DEG = 180f / PI;

		/// <summary>
		///   <para>A representation of positive infinity (Read Only).</para>
		/// </summary>
		public const float INFINITY = float.PositiveInfinity;

		/// <summary>
		///   <para>A representation of negative infinity (Read Only).</para>
		/// </summary>
		public const float NEGATIVE_INFINITY = float.NegativeInfinity;

		/// <summary>
		///   <para>Returns the sine of angle f in radians.</para>
		/// </summary>
		/// <param name="f">The argument as a radian.</param>
		/// <returns>
		///   <para>The return value between -1 and +1.</para>
		/// </returns>
		public static float Sin( float f )
		{
			return ( float )System.Math.Sin( f );
		}

		/// <summary>
		///   <para>Returns the cosine of angle f in radians.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Cos( float f )
		{
			return ( float )System.Math.Cos( f );
		}

		/// <summary>
		///   <para>Returns the tangent of angle f in radians.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Tan( float f )
		{
			return ( float )System.Math.Tan( f );
		}

		/// <summary>
		///   <para>Returns the arc-sine of f - the angle in radians whose sine is f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Asin( float f )
		{
			return ( float )System.Math.Asin( f );
		}

		/// <summary>
		///   <para>Returns the arc-cosine of f - the angle in radians whose cosine is f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Acos( float f )
		{
			return ( float )System.Math.Acos( f );
		}

		/// <summary>
		///   <para>Returns the arc-tangent of f - the angle in radians whose tangent is f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Atan( float f )
		{
			return ( float )System.Math.Atan( f );
		}

		/// <summary>
		///   <para>Returns the angle in radians whose Tan is y/x.</para>
		/// </summary>
		/// <param name="y"></param>
		/// <param name="x"></param>
		public static float Atan2( float y, float x )
		{
			return ( float )System.Math.Atan2( y, x );
		}

		/// <summary>
		///   <para>Returns square root of f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Sqrt( float f )
		{
			return ( float )System.Math.Sqrt( f );
		}

		/// <summary>
		///   <para>Returns the absolute value of f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Abs( float f )
		{
			return ( float )System.Math.Abs( f );
		}

		/// <summary>
		///   <para>Returns the absolute value of value.</para>
		/// </summary>
		/// <param name="value"></param>
		public static int Abs( int value )
		{
			return System.Math.Abs( value );
		}

		/// <summary>
		///   <para>Returns the smallest of two or more values.</para>
		/// </summary>
		public static float Min( float a, float b )
		{
			return ( a >= b ) ? b : a;
		}

		/// <summary>
		///   <para>Returns the smallest of two or more values.</para>
		/// </summary>
		public static float Min( params float[] values )
		{
			int num = values.Length;
			if ( num == 0 )
			{
				return 0f;
			}
			float num2 = values[0];
			for ( int i = 1; i < num; i++ )
			{
				if ( values[i] < num2 )
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		/// <summary>
		///   <para>Returns the smallest of two or more values.</para>
		/// </summary>
		public static int Min( int a, int b )
		{
			return ( a >= b ) ? b : a;
		}

		/// <summary>
		///   <para>Returns the smallest of two or more values.</para>
		/// </summary>
		public static int Min( params int[] values )
		{
			int num = values.Length;
			if ( num == 0 )
			{
				return 0;
			}
			int num2 = values[0];
			for ( int i = 1; i < num; i++ )
			{
				if ( values[i] < num2 )
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		/// <summary>
		///   <para>Returns largest of two or more values.</para>
		/// </summary>
		public static float Max( float a, float b )
		{
			return ( a <= b ) ? b : a;
		}

		/// <summary>
		///   <para>Returns largest of two or more values.</para>
		/// </summary>
		public static float Max( params float[] values )
		{
			int num = values.Length;
			if ( num == 0 )
			{
				return 0f;
			}
			float num2 = values[0];
			for ( int i = 1; i < num; i++ )
			{
				if ( values[i] > num2 )
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		/// <summary>
		///   <para>Returns the largest of two or more values.</para>
		/// </summary>
		public static int Max( int a, int b )
		{
			return ( a <= b ) ? b : a;
		}

		/// <summary>
		///   <para>Returns the largest of two or more values.</para>
		/// </summary>
		public static int Max( params int[] values )
		{
			int num = values.Length;
			if ( num == 0 )
			{
				return  0;
			}
			int num2 = values[0];
			for ( int i = 1; i < num; i++ )
			{
				if ( values[i] > num2 )
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		/// <summary>
		///   <para>Returns f raised to power p.</para>
		/// </summary>
		/// <param name="f"></param>
		/// <param name="p"></param>
		public static float Pow( float f, float p )
		{
			return ( float )System.Math.Pow( f, p );
		}

		/// <summary>
		///   <para>Returns e raised to the specified power.</para>
		/// </summary>
		/// <param name="power"></param>
		public static float Exp( float power )
		{
			return ( float )System.Math.Exp( power );
		}

		/// <summary>
		///   <para>Returns the logarithm of a specified number in a specified base.</para>
		/// </summary>
		/// <param name="f"></param>
		/// <param name="p"></param>
		public static float Log( float f, float p )
		{
			return ( float )System.Math.Log( f, p );
		}

		/// <summary>
		///   <para>Returns the natural (base e) logarithm of a specified number.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Log( float f )
		{
			return ( float )System.Math.Log( f );
		}

		/// <summary>
		///   <para>Returns the base 10 logarithm of a specified number.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Log10( float f )
		{
			return ( float )System.Math.Log10( f );
		}

		/// <summary>
		///   <para>Returns the smallest integer greater to or equal to f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Ceil( float f )
		{
			return ( float )System.Math.Ceiling( f );
		}

		/// <summary>
		///   <para>Returns the largest integer smaller to or equal to f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Floor( float f )
		{
			return ( float )System.Math.Floor( f );
		}

		/// <summary>
		///   <para>Returns f rounded to the nearest integer.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Round( float f )
		{
			return ( float )System.Math.Round( f );
		}

		/// <summary>
		///   <para>Returns the smallest integer greater to or equal to f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Ceiling( float f )
		{
			return ( float )System.Math.Ceiling( f );
		}

		/// <summary>
		///   <para>Returns the smallest integer greater to or equal to f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static int CeilToInt( float f )
		{
			return ( int )System.Math.Ceiling( f );
		}

		/// <summary>
		///   <para>Returns the largest integer smaller to or equal to f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static int FloorToInt( float f )
		{
			return ( int )System.Math.Floor( f );
		}

		/// <summary>
		///   <para>Returns f rounded to the nearest integer.</para>
		/// </summary>
		/// <param name="f"></param>
		public static int RoundToInt( float f )
		{
			return ( int )System.Math.Round( f );
		}

		/// <summary>
		///   <para>Returns the sign of f.</para>
		/// </summary>
		/// <param name="f"></param>
		public static float Sign( float f )
		{
			return ( f < 0f ) ? -1f : 1f;
		}

		/// <summary>
		///   <para>Clamps a value between a minimum float and maximum float value.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public static float Clamp( float value, float min, float max )
		{
			if ( value < min )
			{
				value = min;
			}
			else if ( value > max )
			{
				value = max;
			}
			return value;
		}

		/// <summary>
		///   <para>Clamps value between min and max and returns value.</para>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		public static int Clamp( int value, int min, int max )
		{
			if ( value < min )
			{
				value = min;
			}
			else if ( value > max )
			{
				value = max;
			}
			return value;
		}

		/// <summary>
		///   <para>Clamps value between 0 and 1 and returns value.</para>
		/// </summary>
		/// <param name="value"></param>
		public static float Clamp01( float value )
		{
			float result;
			if ( value < 0f )
			{
				result = 0f;
			}
			else if ( value > 1f )
			{
				result = 1f;
			}
			else
			{
				result = value;
			}
			return result;
		}

		/// <summary>
		///   <para>Linearly interpolates between a and b by t.</para>
		/// </summary>
		/// <param name="a">The start value.</param>
		/// <param name="b">The end value.</param>
		/// <param name="t">The interpolation value between the two floats.</param>
		/// <returns>
		///   <para>The interpolated float result between the two float values.</para>
		/// </returns>
		public static float Lerp( float a, float b, float t )
		{
			return a + ( b - a ) * Clamp01( t );
		}

		/// <summary>
		///   <para>Linearly interpolates between a and b by t with no limit to t.</para>
		/// </summary>
		/// <param name="a">The start value.</param>
		/// <param name="b">The end value.</param>
		/// <param name="t">The interpolation between the two floats.</param>
		/// <returns>
		///   <para>The float value as a result from the linear interpolation.</para>
		/// </returns>
		public static float LerpUnclamped( float a, float b, float t )
		{
			return a + ( b - a ) * t;
		}

		/// <summary>
		///   <para>Same as Lerp but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static float LerpAngle( float a, float b, float t )
		{
			float num = Repeat( b - a, 360f );
			if ( num > 180f )
			{
				num -= 360f;
			}
			return a + num * Clamp01( t );
		}

		/// <summary>
		///   <para>Moves a value current towards target.</para>
		/// </summary>
		/// <param name="current">The current value.</param>
		/// <param name="target">The value to move towards.</param>
		/// <param name="maxDelta">The maximum change that should be applied to the value.</param>
		public static float MoveTowards( float current, float target, float maxDelta )
		{
			float result;
			if ( Abs( target - current ) <= maxDelta )
			{
				result = target;
			}
			else
			{
				result = current + Sign( target - current ) * maxDelta;
			}
			return result;
		}

		/// <summary>
		///   <para>Same as MoveTowards but makes sure the values interpolate correctly when they wrap around 360 degrees.</para>
		/// </summary>
		/// <param name="current"></param>
		/// <param name="target"></param>
		/// <param name="maxDelta"></param>
		public static float MoveTowardsAngle( float current, float target, float maxDelta )
		{
			float num = DeltaAngle( current, target );
			float result;
			if ( -maxDelta < num && num < maxDelta )
			{
				result = target;
			}
			else
			{
				target = current + num;
				result = MoveTowards( current, target, maxDelta );
			}
			return result;
		}

		public static Vec3 FromToDirection( Vec3 from, Vec3 to, float t, Vec3 forward )
		{
			Quat q1 = Quat.FromToRotation( forward, from );
			Quat q2 = Quat.FromToRotation( forward, to );
			Quat q3 = Quat.Lerp( q1, q2, t );
			return q3 * forward;
		}

		/// <summary>
		///   <para>Interpolates between min and max with smoothing at the limits.</para>
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="t"></param>
		public static float SmoothStep( float from, float to, float t )
		{
			t = Clamp01( t );
			t = -2f * t * t * t + 3f * t * t;
			return to * t + from * ( 1f - t );
		}

		public static float Gamma( float value, float absmax, float gamma )
		{
			bool flag = value < 0f;
			float num = Abs( value );
			float result;
			if ( num > absmax )
				result = ( ( !flag ) ? num : ( -num ) );
			else
			{
				float num2 = Pow( num / absmax, gamma ) * absmax;
				result = ( ( !flag ) ? num2 : ( -num2 ) );
			}
			return result;
		}

		/// <summary>
		///   <para>Compares two floating point values and returns true if they are similar.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static bool Approximately( float a, float b )
		{
			return Abs( b - a ) < Max( 1E-06f * Max( Abs( a ), Abs( b ) ), EPSILON * 8f );
		}

		public static float SmoothDamp( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime )
		{
			smoothTime = Max( 0.0001f, smoothTime );
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / ( 1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2 );
			float num4 = current - target;
			float num5 = target;
			float num6 = maxSpeed * smoothTime;
			num4 = Clamp( num4, -num6, num6 );
			target = current - num4;
			float num7 = ( currentVelocity + num * num4 ) * deltaTime;
			currentVelocity = ( currentVelocity - num * num7 ) * num3;
			float num8 = target + ( num4 + num7 ) * num3;
			if ( num5 - current > 0f == num8 > num5 )
			{
				num8 = num5;
				currentVelocity = ( num8 - num5 ) / deltaTime;
			}
			return num8;
		}

		public static float SmoothDampAngle( float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime )
		{
			target = current + DeltaAngle( current, target );
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		/// <summary>
		///   <para>Loops the value t, so that it is never larger than length and never smaller than 0.</para>
		/// </summary>
		/// <param name="t"></param>
		/// <param name="length"></param>
		public static float Repeat( float t, float length )
		{
			return Clamp( t - Floor( t / length ) * length, 0f, length );
		}

		/// <summary>
		///   <para>PingPongs the value t, so that it is never larger than length and never smaller than 0.</para>
		/// </summary>
		/// <param name="t"></param>
		/// <param name="length"></param>
		public static float PingPong( float t, float length )
		{
			t = Repeat( t, length * 2f );
			return length - Abs( t - length );
		}

		/// <summary>
		///   <para>Calculates the linear parameter t that produces the interpolant value within the range [a, b].</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="value"></param>
		public static float InverseLerp( float a, float b, float value )
		{
			return a != b ? Clamp01( ( value - a ) / ( b - a ) ) : 0f;
		}

		/// <summary>
		///   <para>Calculates the shortest difference between two given angles given in degrees.</para>
		/// </summary>
		/// <param name="current"></param>
		/// <param name="target"></param>
		public static float DeltaAngle( float current, float target )
		{
			float num = Repeat( target - current, 360f );
			if ( num > 180f )
			{
				num -= 360f;
			}
			return num;
		}

		public static bool LineIntersection( Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4, ref Vec2 result )
		{
			float num = p2.x - p1.x;
			float num2 = p2.y - p1.y;
			float num3 = p4.x - p3.x;
			float num4 = p4.y - p3.y;
			float num5 = num * num4 - num2 * num3;
			bool result2;
			if ( num5 == 0f )
			{
				result2 = false;
			}
			else
			{
				float num6 = p3.x - p1.x;
				float num7 = p3.y - p1.y;
				float num8 = ( num6 * num4 - num7 * num3 ) / num5;
				result = new Vec2( p1.x + num8 * num, p1.y + num8 * num2 );
				result2 = true;
			}
			return result2;
		}

		public static bool LineSegmentIntersection( Vec2 p1, Vec2 p2, Vec2 p3, Vec2 p4, ref Vec2 result )
		{
			float num = p2.x - p1.x;
			float num2 = p2.y - p1.y;
			float num3 = p4.x - p3.x;
			float num4 = p4.y - p3.y;
			float num5 = num * num4 - num2 * num3;
			bool result2;
			if ( num5 == 0f )
			{
				result2 = false;
			}
			else
			{
				float num6 = p3.x - p1.x;
				float num7 = p3.y - p1.y;
				float num8 = ( num6 * num4 - num7 * num3 ) / num5;
				if ( num8 < 0f || num8 > 1f )
				{
					result2 = false;
				}
				else
				{
					float num9 = ( num6 * num2 - num7 * num ) / num5;
					if ( num9 < 0f || num9 > 1f )
					{
						result2 = false;
					}
					else
					{
						result = new Vec2( p1.x + num8 * num, p1.y + num8 * num2 );
						result2 = true;
					}
				}
			}
			return result2;
		}

		public static long RandomToLong( Random r )
		{
			byte[] array = new byte[8];
			r.NextBytes( array );
			return ( long )( BitConverter.ToUInt64( array, 0 ) & 9223372036854775807uL );
		}

		public static float DegToRad( float deg )
		{
			return deg * DEG_TO_RAD;
		}

		public static float RadToDeg( float rad )
		{
			return rad * RAD_TO_DEG;
		}

		public static float LinearToGammaSpace( float value )
		{
			return Pow( value, 1f / 2.2f );
		}

		public static float GammaToLinearSpace( float value )
		{
			return Pow( value, 2.2f );
		}

		public static void Swap<T>( ref T a, ref T b )
		{
			T temp = a;
			a = b;
			b = temp;
		}

		public static int[] RandomSequence( int[] input )
		{
			Random rnd = new Random();
			int count = input.Length;
			int[] output = new int[count];
			int end = count - 1;
			for ( int i = 0; i < count; i++ )
			{
				int num = rnd.Next( 0, end + 1 );
				output[i] = input[num];
				input[num] = input[end];
				end--;
			}
			return output;
		}
	}
}