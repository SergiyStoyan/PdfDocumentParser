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
using System.Threading;

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
        HandyDictionary<int, AnchorMatchEnumerator> anchorIds2anchorMatchEnumerator = new HandyDictionary<int, AnchorMatchEnumerator>();

        internal class AnchorMatchEnumerator : IDisposable
        {
            ~AnchorMatchEnumerator()
            {
                Dispose();
            }

            public void Dispose()
            {
                StopGetShifts();
            }

            internal AnchorMatchEnumerator(Page page, Template.Anchor anchor)
            {
                Page = page;
                Anchor = anchor;
            }
            readonly internal Template.Anchor Anchor;
            readonly internal Page Page;

            List<SizeF> shifts = null;
            bool foundAll = false;

            internal IEnumerable<SizeF> GetShifts()
            {
                StopGetShifts();

                if (shifts == null)
                {
                    for (int? id = Anchor.ParentAnchorId; id != null;)
                    {
                        Template.Anchor pa = Page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                        if (Anchor == pa)
                            throw new Exception("Reference loop: anchor[Id=" + Anchor.Id + "] is linked by an ancestor anchor.");
                        id = pa.ParentAnchorId;
                    }
                    shifts = new List<SizeF>();
                }
                else
                {
                    foreach (SizeF shift in shifts)
                        yield return shift;
                    if (foundAll)
                        yield break;
                }

                ManualResetEvent m = new ManualResetEvent(true);
                bool getNext = true;
                int i = 0;
                SizeF currentShift = new SizeF();
                t = ThreadRoutines.Start(() =>//!!!how to stop the thread???
                {
                    m.Reset();
                    Page.findAnchor(Anchor, (PointF p) =>
                    {
                        if (i++ < shifts.Count)
                            return true;
                        m.WaitOne();
                        currentShift = new SizeF(p.X - Anchor.Position.X, p.Y - Anchor.Position.Y);
                        m.Set();
                        return t == null;
                    });
                    t = null;
                    m.Set();
                });
                foundAll = getNext;

                while (t?.IsAlive == true)
                {
                    m.WaitOne();
                    if (t == null)
                        break;
                    yield return currentShift;
                    m.Set();
                }
            }
            Thread t;

            internal void StopGetShifts()
            {
                if (t != null)
                {
                    t.Abort();
                    t = null;
                }
            }
        }

        void findAnchor(Template.Anchor anchor, Func<PointF, bool> findNext)
        {
            if (anchor.ParentAnchorId != null)
            {
                Template.Anchor pa = PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchor.ParentAnchorId);
                findAnchor(pa, (PointF p) =>
                {
                    bool found = findAnchor(pa, p, findNext);
                    if (found)
                    {
                        // paai.Position = p;
                    }
                    return !found;
                });
            }
            else
                findAnchor(anchor, new PointF(), findNext);
        }
    }
}