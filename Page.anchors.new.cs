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
        AnchorMatchEnumerator getAnchorMatchEnumerator(int anchorId)
        {
            Template.Anchor a = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[id=" + anchorId + "] does not exist.");
            if (!a.IsSet())
                throw new Exception("Anchor[id=" + anchorId + "] is not set.");

            if (!anchorIds2anchorMatchEnumerator.TryGetValue(a.Id, out AnchorMatchEnumerator anchorMatchEnumerator))
            {
                anchorMatchEnumerator = new AnchorMatchEnumerator(this, a);
                anchorIds2anchorMatchEnumerator[a.Id] = anchorMatchEnumerator;
            }
            return anchorMatchEnumerator;
        }
        Dictionary<int, AnchorMatchEnumerator> anchorIds2anchorMatchEnumerator = new Dictionary<int, AnchorMatchEnumerator>();

        internal class AnchorMatchEnumerator
        {
            internal AnchorMatchEnumerator(Page page, Template.Anchor anchor)
            {
                Anchor = anchor;
            }
            readonly internal Template.Anchor Anchor;

            internal IEnumerable<AnchorActualInfo1> GetMatchs()
            {
                //foreach (RectangleF ar in getFieldMatchRectangles(f))
                throw new Exception("TBD");
            }
        }

        internal class AnchorActualInfo1
        {
            readonly internal Template.Anchor Anchor;
            public PointF Position
            {
                get
                {
                    return position;
                }
                private set
                {
                    position = value;
                    Shift = new SizeF(Position.X - Anchor.Position.X, Position.Y - Anchor.Position.Y);
                }
            }
            PointF position = new PointF(-1, -1);
            internal SizeF Shift { get; private set; }
            internal AnchorActualInfo1 ParentAnchorActualInfo { get; private set; }

            internal AnchorActualInfo1(Template.Anchor anchor, Page page)
            {
                Anchor = anchor;

                for (int? id = anchor.ParentAnchorId; id != null;)
                {
                    Template.Anchor pa = page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                    if (anchor == pa)
                        throw new Exception("Reference loop: anchor[Id=" + anchor.Id + "] is linked by an ancestor anchor.");
                    id = pa.ParentAnchorId;
                }

                findAnchor(page, (PointF p) =>
                {
                    Position = p;
                    return false;
                });
            }

            void findAnchor(Page page, Func<PointF, bool> findNext)
            {
                if (Anchor.ParentAnchorId != null)
                {
                    Template.Anchor pa = page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == Anchor.ParentAnchorId);
                    AnchorActualInfo1 paai = new AnchorActualInfo1(pa);
                    paai.findAnchor(page, (PointF p) =>
                    {
                        bool found = page.findAnchor(Anchor, p, findNext);
                        if (found)
                        {
                            paai.Position = p;
                            ParentAnchorActualInfo = paai;
                        }
                        return !found;
                    });
                }
                else
                    page.findAnchor(Anchor, new PointF(), findNext);
            }

            AnchorActualInfo1(Template.Anchor anchor)
            {
                Anchor = anchor;
            }
        }
    }
}