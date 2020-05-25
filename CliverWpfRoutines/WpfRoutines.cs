using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Media.Imaging;

namespace Cliver.Wpf
{
    static public class Routines
    {
        public static BitmapImage Convert(System.Drawing.Bitmap bitmap, bool disposeBitmap = true)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            if (disposeBitmap)
                bitmap.Dispose();
            return image;
        }

        public static void AddFadeEffect(this System.Windows.Window window, double durationMss)
        {
            window.IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs e) =>
            {
                if (!window.IsVisible)
                    return;
                DoubleAnimation da = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                da.FillBehavior = FillBehavior.Stop;
                da.Completed += delegate
                {
                    window.Background.BeginAnimation(UIElement.OpacityProperty, null);
                };
                window.Background.BeginAnimation(UIElement.OpacityProperty, da);
            };

            window.Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                //if (!window.IsVisible)
                //    return;
                //e.Cancel = true;
                //DoubleAnimation da = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                //da.FillBehavior = FillBehavior.Stop;
                //da.Completed += delegate
                //{
                //    window.Background.Opacity = 0;
                //    window.Visibility = Visibility.Hidden;
                //    window.Close();
                //};
                //window.Background.BeginAnimation(UIElement.OpacityProperty, da);
            };
        }

        public static void TrimWindowSize(this System.Windows.Window window, double screen_factor = 0.8)
        {
            System.Drawing.Size s = Win.SystemInfo.GetPrimaryScreenSize(false);
            int v = (int)((float)s.Width * screen_factor);
            if (window.Width > v)
                window.Width = v;
            v = (int)((float)s.Height * screen_factor);
            if (window.Height > v)
                window.Height = v;
        }

        public static bool IsValid(this DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;
            
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child))
                    return false; 
            }

            return true;
        }

        public static bool IsValid2(this DependencyObject parent)
        {
            // Validate all the bindings on the parent
            bool valid = true;
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    foreach (ValidationRule rule in binding.ValidationRules)
                    {
                        ValidationResult result = rule.Validate(parent.GetValue(entry.Property), null);
                        if (!result.IsValid)
                        {
                            BindingExpression expression = BindingOperations.GetBindingExpression(parent, entry.Property);
                            System.Windows.Controls.Validation.MarkInvalid(expression, new ValidationError(rule, expression, result.ErrorContent, null));
                            valid = false;
                        }
                    }
                }
            }

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid2(child)) { valid = false; }
            }

            return valid;
        }

        public static T FindVisualParentOfType<T>(this DependencyObject dp)
            where T : DependencyObject
        {
            for (DependencyObject d = dp; d != null; d = VisualTreeHelper.GetParent(d))
                if (d is T)
                    return (T)d;
            return null;
        }

        static public IEnumerable<T> FindVisualChildrenOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildrenOfType<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public static IEnumerable<T> FindChildrenOfType<T>(this DependencyObject ob)
            where T : DependencyObject
        {
            foreach (var child in ob.GetChildren())
            {
                T castedChild = child as T;
                if (castedChild != null)
                {
                    yield return castedChild;
                }
                else
                {
                    foreach (var internalChild in FindChildrenOfType<T>(child))
                    {
                        yield return internalChild;
                    }
                }
            }
        }

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject ob)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(ob);
            for (int i = 0; i < childCount; i++)
                yield return VisualTreeHelper.GetChild(ob, i);
        }

        public static T GetVisualChild<T>(this Visual parent) where T : Visual
        {
            T child = default(T);
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int index = 0; index < childCount; index++)
            {
                Visual visualChild = (Visual)VisualTreeHelper.GetChild(parent, index);
                child = visualChild as T;
                if (child != null)
                    return child;
                child = GetVisualChild<T>(visualChild);//Find Recursively
            }
            return child;
        }
        
        /// <summary>
         /// Get next tab order element.
         /// </summary>
         /// <param name="e">The element to get next tab order</param>
         /// <param name="container">The container element owning 'e'. Make sure this is a container of 'e'.</param>
         /// <param name="goDownOnly">True if search only itself and inside of 'container'; otherwise false.
         /// If true and next tab order element is outside of 'container', result in null.</param>
         /// <returns>Next tab order element or null if not found</returns>
        static public DependencyObject GetNextTabElement(this DependencyObject e, DependencyObject container, bool goDownOnly)
        {
            var navigation = typeof(FrameworkElement)
                .GetProperty("KeyboardNavigation", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);

            var method = navigation
                .GetType()
                .GetMethod("GetNextTab", BindingFlags.NonPublic | BindingFlags.Instance);

            return method.Invoke(navigation, new object[] { e, container, goDownOnly }) as DependencyObject;
        }
    }

    public static class WpfManualValidation
    {
        // this dummy attached property is used as a source
        // of the binding.
        static readonly DependencyProperty DummyProperty =
                DependencyProperty.RegisterAttached("DummyProperty", typeof(object), typeof(WpfManualValidation), new PropertyMetadata(null));

        // this class implements a dummy validation rule without behaviour.
        private class DummyValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Manually mark a validation error on the given framework element.
        /// </summary>
        /// <param name="element">The instance of <see cref="FrameworkElement"/> on which to mark the validation error.</param>
        /// <param name="errorContent">An object representing the content of the error.</param>
        public static void MarkInvalid(this FrameworkElement element, object errorContent)
        {
            // create a dummy binding. Conveniently, we bind to the tag of the FrameworkElement,
            // so as to minimise the potential interaction with other code.
            var binding = new Binding("Tag") { Source = element, Mode = BindingMode.OneWayToSource };

            // set the binding on to our dummy property.
            BindingOperations.SetBinding(element, DummyProperty, binding);

            // we now get the live binding expression.
            var bindingExpression = element.GetBindingExpression(DummyProperty);

            // create a dummy binding error, with the specified error content.
            var validationError = new ValidationError(new DummyValidationRule(), binding, errorContent, null);

            // and manually set the validation error on the binding.
            Validation.MarkInvalid(bindingExpression, validationError);
        }

        /// <summary>
        /// Clears all manually assigned errors on the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The instance of <see cref="FrameworkElement"/> on which to clear the validation.</param>
        public static void MarkValid(this FrameworkElement element)
        {
            // to clear an error, we simply remove all bindings to our dummy property.
            BindingOperations.ClearBinding(element, DummyProperty);
        }

        public static DataGridCell GetCell(this DataGrid grid, DataGridRow row, int columnIndex = 0)
        {
            if (row == null)
                return null;

            var presenters = row.FindVisualChildrenOfType<System.Windows.Controls.Primitives.DataGridCellsPresenter>().ToList();
            if (presenters.Count < 1)
                return null;

            var cell = (DataGridCell)presenters[0].ItemContainerGenerator.ContainerFromIndex(columnIndex);
            if (cell != null)
                return cell;

            // now try to bring into view and retreive the cell
            grid.ScrollIntoView(row, grid.Columns[columnIndex]);
            cell = (DataGridCell)presenters[0].ItemContainerGenerator.ContainerFromIndex(columnIndex);

            return cell;
        }
    }
}
