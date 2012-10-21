using System;
using System.Collections.Generic;
using System.Drawing;
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
        
        RectangleF _lastRect;
        readonly List<Block> _blocks;
        public TextBlock[] TextBlocks;
        Block _highlighted;

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
            TextBlocks = text;
            DefaultFont = UIFont.SystemFontOfSize(12f);
            DefaultColor = UIColor.Black;
            _blocks = new List<Block> ();
            _lastRect = RectangleF.Empty;
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
            _blocks.Clear();

            if (TextBlocks == null)
                return 0;

            Block currentBlock = null;
            foreach (var textBlock in TextBlocks)
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
                _blocks.Add(currentBlock);

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

                            currentBlock = new Block { 
                                Font = textBlock.Font ?? DefaultFont,
                                Tapped = textBlock.Tapped,
                                Bounds = new PointF(x, y),
                                Color = textBlock.Color ?? DefaultColor,
                                Width = 0,
                                Height = 17,
                            };
                            _blocks.Add(currentBlock);
                        }

                        currentBlock.Value += word + "".PadLeft(endingSpaces, ' ');
                        currentBlock.Width = StringSize(currentBlock.Value, textBlock.Font ?? DefaultFont).Width;
                        if (currentBlock.Bounds.X + currentBlock.Width > max)
                            currentBlock.Width = max - currentBlock.Bounds.X;
                        x = currentBlock.Bounds.X + currentBlock.Width;
                    }
                }
            }

            return y + 17f;
        }

        public override void Draw (RectangleF rect)
        {
            if (rect != _lastRect){
                Layout ();
                _lastRect = rect;
            }
            
            var context = UIGraphics.GetCurrentContext ();
            foreach (var block in _blocks){
                context.SetFillColor(block.Color.CGColor);

                var bounds = new RectangleF(block.Bounds, new SizeF(block.Width, block.Height));

                // selected?
                if (block == _highlighted)
                {
                    context.FillRect (bounds);
                    context.SetFillColor (1, 1, 1, 1);
                }

                DrawString (block.Value, bounds, block.Font, UILineBreakMode.TailTruncation, UITextAlignment.Left);
            }
        }


        bool Track (PointF pos)
        {
            foreach (var block in _blocks){
                var bounds = new RectangleF(block.Bounds, new SizeF(block.Width, block.Height));
                if (!bounds.Contains (pos) || block.Tapped == null)
                    continue;
                
                _highlighted = block;
                SetNeedsDisplay ();
                return true;
            }

            return false;
        }
        
        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            if (_highlighted != null && _highlighted.Tapped != null){
                _highlighted.Tapped();
            }
            
            _highlighted = null;
            SetNeedsDisplay ();
            base.TouchesEnded(touches, evt);
        }

        public override bool PointInside(PointF point, UIEvent uievent)
        {
            if (Bounds.Contains(point))
            {
                Track(point);
                return true;
            }

            return base.PointInside(point, uievent);
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            var uiTouch = touches.AnyObject as UITouch;
            if (uiTouch != null && !Track (uiTouch.LocationInView (this)))
                base.TouchesBegan(touches, evt);
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            _highlighted = null;
            SetNeedsDisplay ();
        }
        
        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            var uiTouch = touches.AnyObject as UITouch;
            if (uiTouch != null) Track (uiTouch.LocationInView (this));
        }
    }
}
