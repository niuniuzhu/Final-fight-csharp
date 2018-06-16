namespace Core.FMath
{
	public partial struct Fix64
	{
		public static readonly Fix64 DEG_TO_RAD = ( Fix64 )0.01745329251994329576923690768489f;
		public static readonly Fix64 RAD_TO_DEG = ( Fix64 )57.295779513082320876798154814105f;

		public static Fix64 Min( Fix64 a, Fix64 b )
		{
			return a >= b ? b : a;
		}

		public static Fix64 Min( params Fix64[] values )
		{
			int num = values.Length;
			Fix64 result;
			if ( num == 0 )
			{
				result = Zero;
			}
			else
			{
				Fix64 num2 = values[0];
				for ( int i = 1; i < num; i++ )
				{
					if ( values[i] < num2 )
					{
						num2 = values[i];
					}
				}
				result = num2;
			}
			return result;
		}

		public static Fix64 Max( Fix64 a, Fix64 b )
		{
			return a <= b ? b : a;
		}

		public static Fix64 Max( params Fix64[] values )
		{
			int num = values.Length;
			Fix64 result;
			if ( num == 0 )
			{
				result = Zero;
			}
			else
			{
				Fix64 num2 = values[0];
				for ( int i = 1; i < num; i++ )
				{
					if ( values[i] > num2 )
					{
						num2 = values[i];
					}
				}
				result = num2;
			}
			return result;
		}

		public static Fix64 Clamp( Fix64 value, Fix64 min, Fix64 max )
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

		public static Fix64 Clamp01( Fix64 value )
		{
			Fix64 result;
			if ( value < Zero )
			{
				result = Zero;
			}
			else if ( value > One )
			{
				result = One;
			}
			else
			{
				result = value;
			}
			return result;
		}

		public static Fix64 Lerp( Fix64 a, Fix64 b, Fix64 t )
		{
			return a + ( b - a ) * Clamp01( t );
		}

		public static Fix64 LerpUnclamped( Fix64 a, Fix64 b, Fix64 t )
		{
			return a + ( b - a ) * t;
		}

		public static Fix64 SmoothDamp( Fix64 current, Fix64 target, ref Fix64 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed, Fix64 deltaTime )
		{
			smoothTime = Max( Epsilon, smoothTime );
			Fix64 num = Two / smoothTime;
			Fix64 num2 = num * deltaTime;
			Fix64 num3 = One / ( One + num2 + ( Fix64 )0.48f * num2 * num2 + ( Fix64 )0.235f * num2 * num2 * num2 );
			Fix64 num4 = current - target;
			Fix64 num5 = target;
			Fix64 num6 = maxSpeed * smoothTime;
			num4 = Clamp( num4, -num6, num6 );
			target = current - num4;
			Fix64 num7 = ( currentVelocity + num * num4 ) * deltaTime;
			currentVelocity = ( currentVelocity - num * num7 ) * num3;
			Fix64 num8 = target + ( num4 + num7 ) * num3;
			if ( num5 - current > Zero == num8 > num5 )
			{
				num8 = num5;
				currentVelocity = ( num8 - num5 ) / deltaTime;
			}
			return num8;
		}

		public static Fix64 SmoothDampAngle( Fix64 current, Fix64 target, ref Fix64 currentVelocity, Fix64 smoothTime, Fix64 maxSpeed, Fix64 deltaTime )
		{
			target = current + DeltaAngle( current, target );
			return SmoothDamp( current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime );
		}

		public static Fix64 Repeat( Fix64 t, Fix64 length )
		{
			return Clamp( t - Floor( t / length ) * length, Zero, length );
		}

		public static Fix64 PingPong( Fix64 t, Fix64 length )
		{
			t = Repeat( t, length * Two );
			return length - Abs( t - length );
		}

		public static Fix64 InverseLerp( Fix64 a, Fix64 b, Fix64 value )
		{
			return a != b ? Clamp01( ( value - a ) / ( b - a ) ) : Zero;
		}

		public static Fix64 DeltaAngle( Fix64 current, Fix64 target )
		{
			Fix64 num = Repeat( target - current, ( Fix64 )360 );
			if ( num > ( Fix64 )180 )
			{
				num -= ( Fix64 )360;
			}
			return num;
		}

		public static Fix64 DegToRad( Fix64 deg )
		{
			return deg * DEG_TO_RAD;
		}

		public static Fix64 RadToDeg( Fix64 rad )
		{
			return rad * RAD_TO_DEG;
		}

		public static void Swap<T>( ref T a, ref T b )
		{
			T temp = a;
			a = b;
			b = temp;
		}
	}
}