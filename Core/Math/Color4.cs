namespace Core.Math
{
	public struct Color4
	{
		public float r;
		public float g;
		public float b;
		public float a;

		public float grayScale => 0.299f * this.r + 0.587f * this.g + 0.114f * this.b;

		public Color4 gamma => new Color4( MathUtils.LinearToGammaSpace( this.r ), MathUtils.LinearToGammaSpace( this.g ), MathUtils.LinearToGammaSpace( this.b ), this.a );

		public Color4 linear => new Color4( MathUtils.GammaToLinearSpace( this.r ), MathUtils.GammaToLinearSpace( this.g ), MathUtils.GammaToLinearSpace( this.b ), this.a );

		public float maxColorComponent => MathUtils.Max( MathUtils.Max( this.r, this.g ), this.b );

		private static readonly Color4 CLEAR = new Color4( 0f, 0f, 0f, 0f );
		private static readonly Color4 CYAN = new Color4( 0f, 1f, 1f, 1f );
		private static readonly Color4 GRAY = new Color4( 0.5f, 0.5f, 0.5f, 1f );
		private static readonly Color4 GREY = new Color4( 0.5f, 0.5f, 0.5f, 1f );
		private static readonly Color4 MAGENTA = new Color4( 1f, 0f, 1f, 1f );
		private static readonly Color4 RED = new Color4( 1f, 0f, 0f, 1f );
		private static readonly Color4 WHITE = new Color4( 1f, 1f, 1f, 1f );
		private static readonly Color4 BLACK = new Color4( 0f, 0f, 0f, 1f );
		private static readonly Color4 BLUE = new Color4( 0f, 0f, 1f, 1f );
		private static readonly Color4 GREEN = new Color4( 0f, 1f, 0f, 1f );
		private static readonly Color4 YELLOW = new Color4( 1f, 0.92f, 0.016f, 1f );

		public static Color4 clear => CLEAR;

		public static Color4 cyan => CYAN;

		public static Color4 gray => GRAY;

		public static Color4 grey => GREY;

		public static Color4 magenta => MAGENTA;

		public static Color4 red => RED;

		public static Color4 white => WHITE;

		public static Color4 black => BLACK;

		public static Color4 blue => BLUE;

		public static Color4 green => GREEN;

		public static Color4 yellow => YELLOW;

		public Color4( float r, float g, float b )
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 1;
		}

		public Color4( float r, float g, float b, float a )
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public static Color4 Lerp( Color4 a, Color4 b, float t )
		{
			t = MathUtils.Clamp01( t );
			return new Color4( a.r + t * ( b.r - a.r ), a.g + t * ( b.g - a.g ), a.b + t * ( b.b - a.b ), a.a + t * ( b.a - a.a ) );
		}

		public static Color4 LerpUnclamped( Color4 a, Color4 b, float t )
		{
			return new Color4( a.r + t * ( b.r - a.r ), a.g + t * ( b.g - a.g ), a.b + t * ( b.b - a.b ), a.a + t * ( b.a - a.a ) );
		}


		public static Color4 HsvtoRgb( float h, float s, float v, bool hdr )
		{
			Color4 white = new Color4( 1, 1, 1, 1 );

			if ( s == 0 )
			{
				white.r = v;
				white.g = v;
				white.b = v;
				return white;
			}

			if ( v == 0 )
			{
				white.r = 0;
				white.g = 0;
				white.b = 0;
				return white;
			}

			white.r = 0;
			white.g = 0;
			white.b = 0;
			var num = s;
			var num2 = v;
			var f = h * 6;
			var num4 = MathUtils.Floor( f );
			var num5 = f - num4;
			var num6 = num2 * ( 1 - num );
			var num7 = num2 * ( 1 - ( num * num5 ) );
			var num8 = num2 * ( 1 - ( num * ( 1 - num5 ) ) );
			var num9 = num4;

			var flag = num9 + 1;
			if ( flag == 0 )
			{
				white.r = num2;
				white.g = num6;
				white.b = num7;
			}
			else if ( flag == 1 )
			{
				white.r = num2;
				white.g = num8;
				white.b = num6;
			}
			else if ( flag == 2 )
			{
				white.r = num7;
				white.g = num2;
				white.b = num6;
			}
			else if ( flag == 3 )
			{
				white.r = num6;
				white.g = num2;
				white.b = num8;
			}
			else if ( flag == 4 )
			{
				white.r = num6;
				white.g = num7;
				white.b = num2;
			}
			else if ( flag == 5 )
			{
				white.r = num8;
				white.g = num6;
				white.b = num2;
			}
			else if ( flag == 6 )
			{
				white.r = num2;
				white.g = num6;
				white.b = num7;
			}
			else if ( flag == 7 )
			{
				white.r = num2;
				white.g = num8;
				white.b = num6;
			}

			if ( !hdr )
			{
				white.r = MathUtils.Clamp( white.r, 0, 1 );
				white.g = MathUtils.Clamp( white.g, 0, 1 );
				white.b = MathUtils.Clamp( white.b, 0, 1 );
			}
			return white;
		}

		public static void RgbtoHsvHelper( float offset, float dominantcolor, float colorone, float colortwo, out float h, out float s, out float v )
		{
			v = dominantcolor;
			h = 0;
			s = 0;
			if ( v != 0f )
			{
				float num = colorone > colortwo ? colortwo : colorone;

				var num2 = v - num;

				if ( num2 != 0f )
				{
					s = num2 / v;
					h = offset + ( colorone - colortwo ) / num2;
				}
				else
				{
					s = 0;
					h = offset + ( colorone - colortwo );
				}

				h = h / 6;
				if ( h < 0 )
					h = h + 1;
			}
		}

		public static void RgbtoHsv( Color4 rgbColor, out float h, out float s, out float v )
		{
			if ( rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r )
				RgbtoHsvHelper( 4, rgbColor.b, rgbColor.r, rgbColor.g, out h, out s, out v );
			else if ( rgbColor.g > rgbColor.r )
				RgbtoHsvHelper( 2, rgbColor.g, rgbColor.b, rgbColor.r, out h, out s, out v );
			else
				RgbtoHsvHelper( 0, rgbColor.r, rgbColor.g, rgbColor.b, out h, out s, out v );
		}

		public static Color4 operator *( Color4 color, float scale )
		{
			return new Color4( color.r * scale, color.g * scale, color.b * scale, color.a * scale );
		}

		public static Color4 operator *( Color4 a, Color4 b )
		{
			return new Color4( a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a );
		}

		public static Color4 operator /( Color4 color, float scale )
		{
			return new Color4( color.r / scale, color.g / scale, color.b / scale, color.a / scale );
		}

		public static Color4 operator /( Color4 a, Color4 b )
		{
			return new Color4( a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a );
		}

		public static Color4 operator +( Color4 a, Color4 b )
		{
			return new Color4( a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a );
		}

		public static Color4 operator -( Color4 a, Color4 b )
		{
			return new Color4( a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a );
		}

		public override bool Equals( object obj )
		{
			return obj is Color4 && ( ( Color4 ) obj ) == this;
		}

		public override int GetHashCode()
		{
			return this.r.GetHashCode() ^ this.g.GetHashCode() ^ this.b.GetHashCode() ^ this.a.GetHashCode();
		}

		public static bool operator ==( Color4 p1, Color4 p2 )
		{
			return p1.r == p2.r && p1.g == p2.g && p1.b == p2.b && p1.a == p2.a;
		}

		public static bool operator !=( Color4 p1, Color4 p2 )
		{
			return p1.r != p2.r || p1.g != p2.g || p1.b != p2.b || p1.a != p2.a;
		}

		public override string ToString()
		{
			return $"({this.r}, {this.g}, {this.b},{this.a})";
		}
	}
}