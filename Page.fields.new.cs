//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Cliver.PdfDocumentParser
{
    public partial class Page
    {
        FieldMatchEnumerator getFieldMatchEnumerator(string fieldName, Template.Field.Types? type)
        {
            if (!fieldNames2fieldMatchEnumerator.TryGetValue(fieldName, out FieldMatchEnumerator fme))
            {
                IEnumerable<Template.Field> fs = PageCollection.ActiveTemplate.Fields.Where(x => x.Name == fieldName);
                if (!fs.Any())
                    throw new Exception("Field[name=" + fieldName + "] does not exist.");

                foreach (Template.Field f in fs)
                {
                    fme = new FieldMatchEnumerator(this, f, type);
                    if (fme.GetMatches().Any())
                    {
                        fieldNames2fieldMatchEnumerator[fieldName] = fme;
                        break;
                    }
                }
            }
            return fme;
        }
        HandyDictionary<string, FieldMatchEnumerator> fieldNames2fieldMatchEnumerator = new HandyDictionary<string, FieldMatchEnumerator>();

        internal class FieldMatchEnumerator
        {
            internal FieldMatchEnumerator(Page page, Template.Field field, Template.Field.Types? type)
            {
                this.page = page;
                this.Field = field;
                if (type == null)
                    type = field.Type;
                this.type = type.Value;
            }
            Page page;
           internal readonly Template.Field Field;
            Template.Field.Types type;

            internal IEnumerable<object> GetMatches()
            {
                foreach (RectangleF ar in page.getFieldMatchRectangles(Field))
                {
                    throw new Exception("TBD");
                    yield return null;
                }
            }
        }

        IEnumerable<RectangleF> getFieldMatchRectangles(Template.Field field)
        {
            if (!field.IsSet())
                throw new Exception("Field is not set.");
            if (field.Rectangle.Width <= Settings.Constants.CoordinateDeviationMargin || field.Rectangle.Height <= Settings.Constants.CoordinateDeviationMargin)
                throw new Exception("Rectangle is malformed.");
            RectangleF r = field.Rectangle.GetSystemRectangleF();
            IEnumerator<SizeF> leftAnchorShifts = field.LeftAnchor != null ? getAnchorMatchEnumerator(field.LeftAnchor.Id).GetShifts().GetEnumerator() : null;
            IEnumerator<SizeF> topAnchorShifts = field.TopAnchor != null ? getAnchorMatchEnumerator(field.TopAnchor.Id).GetShifts().GetEnumerator() : null;
            IEnumerator<SizeF> rightAnchorShifts = field.RightAnchor != null ? getAnchorMatchEnumerator(field.RightAnchor.Id).GetShifts().GetEnumerator() : null;
            IEnumerator<SizeF> bottomAnchorShifts = field.BottomAnchor != null ? getAnchorMatchEnumerator(field.BottomAnchor.Id).GetShifts().GetEnumerator() : null;
            while (leftAnchorShifts?.Current != null || topAnchorShifts?.Current != null || rightAnchorShifts?.Current != null || bottomAnchorShifts?.Current != null)
            {
                if (leftAnchorShifts?.MoveNext() == true)
                {
                    float right = r.Right;
                    r.X += leftAnchorShifts.Current.Width - field.LeftAnchor.Shift;
                    r.Width = right - r.X;
                }
                if (topAnchorShifts?.MoveNext() == true)
                {
                    float bottom = r.Bottom;
                    r.Y += topAnchorShifts.Current.Height - field.TopAnchor.Shift;
                    r.Height = bottom - r.Y;
                }
                if (rightAnchorShifts?.MoveNext() == true)
                {
                    r.Width += rightAnchorShifts.Current.Width - field.RightAnchor.Shift;
                }
                if (bottomAnchorShifts?.MoveNext() == true)
                {
                    r.Height += bottomAnchorShifts.Current.Height - field.BottomAnchor.Shift;
                }
                //!!!???when all the anchors found then not null even if it is collapsed
                //if (r.Width <= 0 || r.Height <= 0)
                //    return null;
                yield return r;
            }
        }
    }
}