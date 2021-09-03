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
                Page = page;
                Anchor = anchor;
            }
            readonly internal Template.Anchor Anchor;
            readonly internal Page Page;

            internal IEnumerable<SizeF> GetShifts()
            {
                for (int? id = Anchor.ParentAnchorId; id != null;)
                {
                    Template.Anchor pa = Page.PageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                    if (Anchor == pa)
                        throw new Exception("Reference loop: anchor[Id=" + Anchor.Id + "] is linked by an ancestor anchor.");
                    id = pa.ParentAnchorId;
                }

                PointF position = new PointF();

                Page.findAnchor(Anchor, (PointF p) =>
                {
                    position = p;
                    return false;
                });

                yield return new SizeF(position.X - Anchor.Position.X, position.Y - Anchor.Position.Y);

                //return IObservable<SizeF>.Create<SizeF>(o =>
                //{
                //    // Schedule this onto another thread, otherwise it will block:
                //    Scheduler.Later.Schedule(() =>
                //    {
                //        functionReceivingCallback(o.OnNext);
                //        o.OnCompleted();
                //    });

                //    return () => { };
                //}).ToEnumerable();
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