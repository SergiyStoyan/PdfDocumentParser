//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey.stoyan@hotmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
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

            internal IEnumerator<SizeF> GetShifts()
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

                stop = false;
                AutoResetEvent m = new AutoResetEvent(false);
                int i = 0;
                SizeF? currentShift = null;
                t = ThreadRoutines.StartTry(//!!!how to stop the thread???
                    () =>
                    {
                        m.Reset();
                        findAnchor(Anchor, (PointF p) =>
                        {
                            if (i++ < shifts.Count)
                                return true;
                            m.WaitOne();
                            currentShift = new SizeF(p.X - Anchor.Position.X, p.Y - Anchor.Position.Y);
                            m.Set();
                            return t == null;
                        });
                        if (!stop)
                            foundAll = true;
                    },
                    (Exception e) =>
                    {
                        if (e is ThreadAbortException)
                            return;
                        throw e;
                    },
                    () =>
                    {
                        currentShift = null;
                        m.Set();
                    }
                );

                while (t?.IsAlive == true)
                {
                    m.WaitOne();
                    if (currentShift != null)
                        yield return currentShift.Value;
                    m.Set();
                }
            }
            Thread t;
            bool stop = false;

            internal void StopGetShifts()
            {
                if (t?.IsAlive == true)
                {
                    stop = true;
                    t.Abort();
                }
            }

            void findAnchor(Template.Anchor anchor, Func<PointF, bool> findNext)
            {
                if (stop)
                    return;
                if (anchor.ParentAnchorId != null)
                {
                    Template.Anchor pa = Page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchor.ParentAnchorId);
                    findAnchor(pa, (PointF p) =>
                    {
                        return !Page.findAnchor(pa, p, findNext);
                    });
                }
                else
                    Page.findAnchor(anchor, new PointF(), findNext);
            }
        }
    }
}