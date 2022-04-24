using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DSVis.Tools {
    class Arrow : Shape {
        //绑定X1端点数值
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        //DependencyProperty wpf编程和界面打交道，一个属性的值的变化会影响到多个其他对象 该属性类的值的来源并不单一
        //PropertyMetadata 依赖属性的基本元数据
        //FrameworkPropertyMetadata后的affect参数 属性值改变后是否重新运行布局运算
        public double X1 {
            get => (double)GetValue(X1Property);
            set => SetValue(X1Property, value);
        }

        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double X2 {
            get => (double)GetValue(X2Property);
            set => SetValue(X2Property, value);
        }
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double Y1 {
            get => (double)GetValue(Y1Property);
            set => SetValue(Y1Property, value);
        }

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double Y2 {
            get => (double)GetValue(Y2Property);
            set => SetValue(Y2Property, value);
        }

        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double HeadWidth {
            get => (double)GetValue(HeadWidthProperty);
            set => SetValue(HeadWidthProperty, value);
        }

        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double HeadHeight {
            get => (double)GetValue(HeadHeightProperty);
            set => SetValue(HeadHeightProperty, value);
        }

        protected override Geometry DefiningGeometry {
            get {
                //通过保存字节流绘制数据，效率高
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                //打开一个StreamGeometryContext以描述内容
                using (StreamGeometryContext context = geometry.Open()) {
                    double theta = Math.Atan2(Y1 - Y2, X1 - X2);
                    double sint = Math.Sin(theta);
                    double cost = Math.Cos(theta);
                    Point start = new Point(X1, this.Y1);
                    Point end = new Point(X2, this.Y2);
                    Point left = new Point(X2 + (HeadWidth * cost - HeadHeight * sint), Y2 + (HeadWidth * sint + HeadHeight * cost));
                    Point right = new Point(X2 + (HeadWidth * cost + HeadHeight * sint), Y2 - (HeadHeight * cost - HeadWidth * sint));

                    context.BeginFigure(start, true, false);
                    context.LineTo(end, true, true);
                    context.LineTo(left, true, true);
                    context.BeginFigure(end, true, false);
                    context.LineTo(right, true, true);
                    context.Close();
                }

                //结束并不再绘制
                geometry.Freeze();

                return geometry;
            }
        }
    }
}
