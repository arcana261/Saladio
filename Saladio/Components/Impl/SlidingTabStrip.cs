using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

namespace Saladio.Components.Impl
{
    public class SlidingTabStrip : LinearLayout
    {
        #region constants
        private const int DEFAULT_BOTTOM_BORDER_THICKNESS = 2;
        private const byte DEFAULT_BOTTOM_BORDER_COLOR_ALPHA = 0x26;
        private const int SELECTED_INDICATOR_THICKNESS_DIPS = 8;
        private int[] INDICATOR_COLORS = { 0x19A319, 0x0000FC };
        private int[] DIVIDER_COLORS = { 0xC5C5C5 };

        private const int DEFAULT_DEVIDER_THICKNESS_DIPS = 1;
        private const float DEFAULT_DEVIDER_HEIGHT = 0.5f;
        #endregion

        #region bottom border
        private int bottomBorderThickness;
        private Paint bottomBorderPaint;
        private int defaultBottomBorderColor;
        #endregion

        #region indicator
        private int selectedIndicatorThickness;
        private Paint selectedIndicatorPaint;
        #endregion

        #region divider
        private float dividerHeight;
        private Paint dividerPaint;
        #endregion

        #region selected position and offset
        private int selectedPosition;
        private float selectedOffset;
        #endregion

        #region tab colorizer
        private SlidingTabScrollView.TabColorizer customTabColorizer;
        private SimpleTabColorizer defaultTabColorizer;
        #endregion

        #region constructors
        public SlidingTabStrip(Context context) : this(context, null)
        { }

        public SlidingTabStrip(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetWillNotDraw(false);

            float density = Resources.DisplayMetrics.Density;

            TypedValue outValue = new TypedValue();
            context.Theme.ResolveAttribute(Android.Resource.Attribute.ColorForeground, outValue, true);
            int themeForeground = outValue.Data;
            defaultBottomBorderColor = SetColorAlpha(themeForeground, DEFAULT_BOTTOM_BORDER_COLOR_ALPHA);

            defaultTabColorizer = new SimpleTabColorizer();
            defaultTabColorizer.IndicatorColors = INDICATOR_COLORS;
            defaultTabColorizer.DividerColors = DIVIDER_COLORS;

            bottomBorderThickness = (int)(DEFAULT_BOTTOM_BORDER_THICKNESS * density);
            bottomBorderPaint = new Paint();
            bottomBorderPaint.Color = GetColorFromInteger(0xC5C5C5);

            selectedIndicatorThickness = (int)(SELECTED_INDICATOR_THICKNESS_DIPS * density);
            selectedIndicatorPaint = new Paint();

            dividerHeight = DEFAULT_DEVIDER_HEIGHT;
            dividerPaint = new Paint();
            dividerPaint.StrokeWidth = (int)(DEFAULT_DEVIDER_THICKNESS_DIPS * density);
        }
        #endregion

        public SlidingTabScrollView.TabColorizer CustomTabColorizer
        {
            get { return customTabColorizer; }
            set
            {
                customTabColorizer = value;
                Invalidate();
            }
        }

        public int[] SelectedIndicatorColors
        {
            get
            {
                if (customTabColorizer != null)
                {
                    throw new InvalidOperationException("custom colorizer is present");
                }

                return defaultTabColorizer.IndicatorColors;
            }

            set
            {
                customTabColorizer = null;
                defaultTabColorizer.IndicatorColors = INDICATOR_COLORS;
                Invalidate();
            }
        }

        public int[] DividerColors
        {
            get
            {
                if (customTabColorizer != null)
                {
                    throw new InvalidOperationException("custom colorizer is present");
                }

                return defaultTabColorizer.DividerColors;
            }

            set
            {
                customTabColorizer = null;
                defaultTabColorizer.DividerColors = value;
                Invalidate();
            }
        }

        protected SlidingTabScrollView.TabColorizer GetTabColorizerInternal()
        {
            return customTabColorizer != null ? customTabColorizer : defaultTabColorizer;
        }

        private Color GetColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        private int SetColorAlpha(int color, byte alpha)
        {
            return Color.Argb(alpha, Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        public void OnViewerPagerPageChanged(int position, float positionOffset)
        {
            selectedPosition = position;
            selectedOffset = positionOffset;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            int height = Height;
            int tabCount = ChildCount;
            int dividerHeightPx = (int)(Math.Min(Math.Max(dividerHeight, 0.0f), 1.0f) * height);
            SlidingTabScrollView.TabColorizer tabColorizer = GetTabColorizerInternal();

            if (tabCount > 0)
            {
                View selectedTitle = GetChildAt(selectedPosition);
                int left = selectedTitle.Left;
                int right = selectedTitle.Right;
                int color = tabColorizer.GetIndicatorColor(selectedPosition);

                if (selectedOffset > 0.0f && (selectedPosition + 1) < tabCount)
                {
                    int nextColor = tabColorizer.GetIndicatorColor(selectedPosition + 1);
                    if (color != nextColor)
                    {
                        color = blendColor(nextColor, color, selectedOffset);
                    }

                    View nextTitle = GetChildAt(selectedPosition + 1);
                    left = (int)((nextTitle.Left * selectedOffset) + (left * (1.0f - selectedOffset)));
                    right = (int)((nextTitle.Right * selectedOffset) + (right * (1.0f - selectedOffset)));
                }

                selectedIndicatorPaint.Color = GetColorFromInteger(color);
                canvas.DrawRect(left, height - selectedIndicatorThickness, right, height, selectedIndicatorPaint);

                int seperatorTop = (height - dividerHeightPx) / 2;
                for (int i = 0; i < tabCount; i++)
                {
                    View child = GetChildAt(i);
                    dividerPaint.Color = GetColorFromInteger(tabColorizer.GetDividerColor(i));
                    canvas.DrawLine(child.Right, seperatorTop, child.Right, seperatorTop + dividerHeightPx, dividerPaint);
                }

                canvas.DrawRect(0, height - bottomBorderThickness, Width, height, bottomBorderPaint);
            }
        }

        private int blendColor(int color1, int color2, float ratio)
        {
            float inverseRatio = 1.0f - ratio;
            float r = (Color.GetRedComponent(color1) * ratio) + (Color.GetRedComponent(color2) * inverseRatio);
            float g = (Color.GetGreenComponent(color1) * ratio) + (Color.GetGreenComponent(color2) * inverseRatio);
            float b = (Color.GetBlueComponent(color1) * ratio) + (Color.GetBlueComponent(color2) * inverseRatio);

            return Color.Rgb((int)r, (int)g, (int)b);
        }

        private class SimpleTabColorizer : SlidingTabScrollView.TabColorizer
        {
            public int GetIndicatorColor(int position)
            {
                return IndicatorColors[position % IndicatorColors.Length];
            }

            public int GetDividerColor(int position)
            {
                return DividerColors[position % DividerColors.Length];
            }

            public int[] IndicatorColors { get; set; }

            public int[] DividerColors { get; set; }
        }
    }
}