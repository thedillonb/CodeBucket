using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeFramework.UI.Views
{
    /// <summary>
    ///   A UIView that renders a tweet in full, including highlighted #hash, @usernames
    ///   and urls
    /// </summary>
    public class InteractiveTextView : UIView 
    {
        public float Height { get; private set; }
        
        RectangleF lastRect;
        List<Block> blocks;
        public TextBlock[] textBlocks;
        Block highlighted = null;
        const int fontHeight = 17;

        public UIFont DefaultFont { get; set; }
        public UIColor DefaultColor { get; set; }

        public class TextBlock
        {
            public string Value;
            public Action Tapped;
            public UIFont Font;
            public UIColor Color;

            public TextBlock(string value, UIFont font = null, UIColor color = null, Action tapped = null)
            {
                Value = value; Font = font; Color = color; Tapped = tapped;
            }
        }
        
        class Block {
            public string Value;
            public PointF Bounds;
            public UIFont Font;
            public Action Tapped;
            public UIColor Color;
            public float Width;
            public float Height;
        }

        public InteractiveTextView (RectangleF frame, params TextBlock[] text) : base (frame)
        {
            textBlocks = text;
            DefaultFont = UIFont.SystemFontOfSize(12f);
            DefaultColor = UIColor.Black;
            blocks = new List<Block> ();
            lastRect = RectangleF.Empty;
            Height = Layout ();
            BackgroundColor = UIColor.Clear;

            this.UserInteractionEnabled = true;
            this.MultipleTouchEnabled = false;

            // Update our Frame size
            var f = Frame;
            f.Height = Height;
            Frame = f;
        }

        private ICollection<string> CreatedTokenizedString(string text)
        {
            ICollection<string> lines = new LinkedList<string>();
            int p = 0;
            while (p < text.Length)
            {
                var nextNewLine = text.IndexOf('\n', p);
                
                //There is no new line in this string...
                if (nextNewLine == -1)
                {
                    lines.Add(text.Substring(p));
                    break;
                }
                else
                {
                    if (p < nextNewLine)
                        lines.Add(text.Substring(p, nextNewLine));
                    lines.Add("\n");
                    p += nextNewLine + 1;
                }
            }
            return lines;
        }

        public void DoLayout()
        {
            Height = Layout();
            var f = Frame;
            f.Height = Height;
            Frame = f;
        }
        
        float Layout()
        {
            float max = Bounds.Width, x = 0, y = 0;
            blocks.Clear();

            if (textBlocks == null)
                return 0;

            Block currentBlock = null;
            foreach (var textBlock in textBlocks)
            {
                var text = textBlock.Value;
                currentBlock = new Block() { 
                    Font = textBlock.Font ?? DefaultFont,
                    Tapped = textBlock.Tapped,
                    Bounds = new PointF(x, y),
                    Color = textBlock.Color ?? DefaultColor,
                    Width = 0,
                    Height = 17,
                };
                blocks.Add(currentBlock);

                var lines = CreatedTokenizedString(text);

                foreach (var lineSplit in lines)
                {
                    if (lineSplit.Equals("\n"))
                    {
                        //Adjust the y coordinate!
                        y += 17f;
                        x = 0;
                        continue;
                    }

                    int s = 0;
                    while (s < lineSplit.Length)
                    {
                        var si = lineSplit.IndexOf(' ', s);
                        int endingSpaces = 0;
                        string word = null;

                        //There is no more..
                        if (si == -1)
                        {
                            word = lineSplit.Substring(s);
                            s = lineSplit.Length;
                        }
                        //There is a space!
                        else
                        {
                            word = lineSplit.Substring(s, si - s);
                            s = si;
                            while (s < lineSplit.Length)
                            {
                                if (lineSplit[s] == ' ')
                                    endingSpaces++;
                                else
                                    break;
                                s++;
                            }
                        }


                        var wordSize = StringSize(word, textBlock.Font ?? DefaultFont).Width;
                        
                        //Start a new line!
                        if (x + wordSize > max)
                        {
                            y += 17f;
                            x = 0;

                            currentBlock = new Block() { 
                                Font = textBlock.Font ?? DefaultFont,
                                Tapped = textBlock.Tapped,
                                Bounds = new PointF(x, y),
                                Color = textBlock.Color ?? DefaultColor,
                                Width = 0,
                                Height = 17,
                            };
                            blocks.Add(currentBlock);
                        }

                        currentBlock.Value += word + "".PadLeft(endingSpaces, ' ');
                        currentBlock.Width = StringSize(currentBlock.Value, textBlock.Font ?? DefaultFont).Width;
                        x = currentBlock.Bounds.X + currentBlock.Width;
                    }
                }
            }

            return y + 17f;
        }

        static Random rand = new Random();

        public override void Draw (RectangleF rect)
        {
            if (rect != lastRect){
                Layout ();
                lastRect = rect;
            }
            
            var context = UIGraphics.GetCurrentContext ();
            foreach (var block in blocks){
                context.SetFillColor(block.Color.CGColor);

                var bounds = new RectangleF(block.Bounds, new SizeF(block.Width, block.Height));

                // selected?
                if (block == highlighted)
                {
                    context.FillRect (bounds);
                    context.SetFillColor (1, 1, 1, 1);
                }

                //context.SetFillColor((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 0.3f);
                //context.FillRect(bounds);
                //context.SetFillColor (1, 1, 1, 1);
                DrawString (block.Value, bounds, block.Font, UILineBreakMode.Clip, UITextAlignment.Left);
            }
        }


        bool Track (PointF pos)
        {
            foreach (var block in blocks){
                var bounds = new RectangleF(block.Bounds, new SizeF(block.Width, block.Height));
                if (!bounds.Contains (pos) || block.Tapped == null)
                    continue;
                
                highlighted = block;
                SetNeedsDisplay ();
                return true;
            }

            return false;
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            if (highlighted != null && highlighted.Tapped != null){
                highlighted.Tapped();
            }
            
            highlighted = null;
            SetNeedsDisplay ();
        }

        public override bool PointInside(PointF point, UIEvent uievent)
        {
            if (this.Bounds.Contains(point))
            {
                Track(point);
                return true;
            }

            return false;
            //return base.PointInside(point, uievent);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (!Track ((touches.AnyObject as UITouch).LocationInView (this)))
                base.TouchesBegan(touches, evt);
        }
        
        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            highlighted = null;
            SetNeedsDisplay ();
        }
        
        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            Track ((touches.AnyObject as UITouch).LocationInView (this));
        }
    }
}
