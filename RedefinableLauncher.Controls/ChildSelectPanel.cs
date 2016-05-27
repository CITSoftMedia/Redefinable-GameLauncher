using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.ChildSelectPanelElements;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;
using Redefinable.Collections;

using GraphicsPath = System.Drawing.Drawing2D.GraphicsPath;

namespace Redefinable.Applications.Launcher.Controls
{
    public class ChildSelectPanel : ChildPanel
    {
        // 非公開フィールド
        private ChildSelectPanelItemCollection items;
        private bool suspendRefresh;


        // 非公開フィールド :: コントロール
        

        // 公開フィールド

        /// <summary>
        /// このコントロールで表示する項目のコレクションを取得します。
        /// </summary>
        public ChildSelectPanelItemCollection Items
        {
            get { return this.items; }
        }


        // コンストラクタ

        public ChildSelectPanel()
        {
            // データフィールドの初期化
            this.items = new ChildSelectPanelItemCollection();
            this.suspendRefresh = false;

            // コントロールの初期化
            //this._redrawText();


            // イベントデリゲートの初期化


            // イベントの追加
            //this.DrawnBackground += (sender, e) => { this._redrawText(); };
            //this.TextChanged += (sender, e) => { this._redrawText(); };
            this.items.ItemAdded += (sender, e) => { this._refreshControls(); };
            this.items.ItemRemoved += (sender, e) => { this._refreshControls(); };
        }


        // 非公開メソッド
        
        private void _refreshControls()
        {
            // 再描画処理が凍結されていないか確認
            if (this.suspendRefresh)
                return;

            // このコントロール上のChildSelectPanelButtonコントロールをすべて削除
            foreach (Control c in this.Controls)
                if (c is ChildSelectPanelButton)
                    this.Controls.Remove(c);

            // 作成して再配置
            Point upperLeftPoint = new Point(40, 60);
            int span_w = 20; // ボタン同士の横間隔
            int span_h = 20; // ボタン同士の縦間隔
            int line_b = 0; // 1段のボタン数
            int line_c = 1; // 段数(最終加算漏れ分を事前に加算)
            Size buttonDefSize = ChildSelectPanelButton.DefaultButtonSize;
            List<ChildSelectPanelButton> buttons = new List<ChildSelectPanelButton>();
            for (int i = 0; i < this.items.Count; i++)
            {
                // 位置を算出
                Point p = upperLeftPoint;
                if (i != 0)
                {
                    // 前のコントロールがある
                    p = buttons[i - 1].DefaultControlLocation;
                    p.X += span_w + buttonDefSize.Width;

                    if (p.X + buttonDefSize.Width + upperLeftPoint.X > this.DefaultControlSize.Width)
                    {
                        // 右側に行き過ぎィ！
                        // → 次の段へ

                        p.X = upperLeftPoint.X;
                        p.Y = p.Y + buttonDefSize.Height + span_h;

                        line_c++;
                    }
                }

                buttons.Add(new ChildSelectPanelButton(p.GetScaledPoint(this.currentScale), this.items[i]));

                if (line_c == 1)
                    // line_cが初期値 → １段のボタンの数を記録
                    line_b++;
            }

            Console.WriteLine("1段: {0} / {1}段", line_b, line_c);

            // 隣接コントロールの設定
            /*
            for (int i = 0, currentLine = 0, currentButton = 0; i < buttons.Count; i++)
            {

                if (currentButton = )
            }
            */


            foreach (ChildSelectPanelButton b in buttons)
                this.Controls.Add(b);
        }

        private void _redrawText()
        {
            Point upperLeftPoint = new Point(40, 40);
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            Font font = new Font("MS UI Gohic", 15f * this.currentScale);

            string[] text = this.Text.Split(',');
            for (int i = 0; i < text.Length; i++)
            {
                Point p = upperLeftPoint;
                p.Y += i * 10;
                p = p.GetScaledPoint(this.currentScale);
                
                g.DrawString(text[i], font, new SolidBrush(Color.FromArgb(100, Color.Black)), new Point(p.X + 2, p.Y + 2));
                g.DrawString(text[i], font, new SolidBrush(Color.FromArgb(100, Color.White)), p);
            }

            g.Dispose();
            font.Dispose();
        }


        // 公開メソッド

        /// <summary>
        /// Itemsに項目の追加処理をおこなってもコントロールの再描画、再配置処理を行わないようにします。
        /// ResumeRefreshControls()で再開させることができます。
        /// </summary>
        public void SuspendRefreshControls()
        {
            this.suspendRefresh = true;
        }

        /// <summary>
        /// SuspendRefreshControls()により凍結された処理を再開します。
        /// </summary>
        public void ResumeRefreshControls()
        {
            this.suspendRefresh = false;
            this._refreshControls();
        }
    }
    
    public class ChildSelectPanelItemCollection : NativeEventDefinedList<ChildSelectPanelItem>
    {
        // 実装なし
    }

    public class ChildSelectPanelItem
    {
        // 非公開メンバ
        private string text;
        private bool _checked;


        // 公開メンバ

        public string Text
        {
            get { return this._getText(); }
            set { this._setText(value); }
        }

        public bool Checked
        {
            get { return this._getChecked(); }
            set { this._setChecked(value); }
        }
        
        
        // 公開イベント

        /// <summary>
        /// Textの値が変化した際に発生します。
        /// </summary>
        public event EventHandler TextChanged;
        
        /// <summary>
        /// Checkedの値が変化した際に発生します。
        /// </summary>
        public event EventHandler CheckStateChanged;

        
        // コンストラクタ

        /// <summary>
        /// 使用できません
        /// </summary>
        private ChildSelectPanelItem()
        {
            // 実装なし
            this.TextChanged = (sender, e) => { };
            this.CheckStateChanged = (sender, e) => { };
        }

        /// <summary>
        /// 新しいChildSelectPanelItemクラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_checked"></param>
        public ChildSelectPanelItem(string text, bool _checked)
            : this()
        {
            this.text = text;
            this._checked = _checked;
        }

        /// <summary>
        /// 新しいChildSelectPanelItemクラスのインスタンスをCheckedがfalseの状態で初期化します。
        /// </summary>
        /// <param name="text"></param>
        public ChildSelectPanelItem(string text)
            : this(text, false)
        {
            // 実装なし
        }


        // 非公開メソッド

        private string _getText()
        {
            return this.text;
        }

        private void _setText(string value)
        {
            this.text = value;
            this.TextChanged(this, EventArgs.Empty);
        }

        private bool _getChecked()
        {
            return this._checked;
        }

        private void _setChecked(bool value)
        {
            this._checked = value;
            this.CheckStateChanged(this, EventArgs.Empty);
        }
    }

    namespace ChildSelectPanelElements
    {
        /// <summary>
        /// サイズは220x50
        /// </summary>
        public class ChildSelectPanelButton : VariableScaleableControl
        {
            // 非公開フィールド
            private ChildSelectPanelItem item;
            private EventHandler itemParamChangeEventDelegate;

            
            // 公開フィールド
            
            
            // 公開静的フィールド

            public static Size DefaultButtonSize
            {
                get { return new Size(220, 50); }
            }


            // コンストラクタ

            /// <summary>
            /// 新しいChildSelectPanelButtonクラスのインスタンスを初期化します。
            /// </summary>
            /// <param name="point"></param>
            /// <param name="item"></param>
            public ChildSelectPanelButton(Point point, ChildSelectPanelItem item)
                : base(point, DefaultButtonSize)
            {
                // データフィールドの初期化
                this.item = item;


                // コントロールの初期化
                this.Cursor = Cursors.Hand;
                this._redraw();


                // イベントデリゲートの初期化
                this.itemParamChangeEventDelegate = (sender, e) =>
                {
                    //Console.WriteLine("itemParamChangeEventDelegate");
                    this._redraw();
                };


                // イベントの追加
                this.VisibleChanged += ChildSelectPanelButton_VisibleChanged;
                this.MouseDown += ChildSelectPanelButton_Click;
            }


            // 非公開メソッド

            private void _redraw()
            {
                this.BackgroundImage = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(this.BackgroundImage);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                Font font = new Font("MS Gothic", 14f * this.currentScale);
                Size drawingSize;
                Point drawingLocation;


                // 背景
                g.FillRectangle(new SolidBrush(Color.Gray), 0, 0, this.Width, this.Height);
                
                // チェックボックス背景
                drawingSize = new Size(30, 30).GetScaledSize(this.currentScale);
                drawingLocation = new Point(10, 10).GetScaledPoint(this.currentScale);
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(drawingLocation, drawingSize));

                // チェック
                if (this.item.Checked)
                {
                    g.DrawString("✔", font, Brushes.Black, drawingLocation);
                }

                // テキスト
                drawingLocation = new Point(60, 10).GetScaledPoint(this.currentScale);
                g.DrawString(this.item.Text, font, Brushes.White, drawingLocation);

                g.Dispose();
                font.Dispose();
            }


            // 非公開メソッド :: イベント

            private void ChildSelectPanelButton_VisibleChanged(object sender, EventArgs e)
            {
                if (this.Created)
                {
                    // itemChangeEventDelegateをitemに追加
                    //Console.WriteLine("こんとろーるあでっど！");
                    this.item.TextChanged += this.itemParamChangeEventDelegate;
                    this.item.CheckStateChanged += this.itemParamChangeEventDelegate;
                }
                else
                {
                    // itemChangeEventDelegateをitemから削除
                    this.item.TextChanged -= this.itemParamChangeEventDelegate;
                    this.item.CheckStateChanged -= this.itemParamChangeEventDelegate;
                }
            }

            private void ChildSelectPanelButton_Click(object sender, EventArgs e)
            {
                // Checkedの値を変更
                // → 勝手にCheckStateChangedが発生し、最終的にredraw()が呼ばれる
                this.item.Checked = !this.item.Checked;
                //this._redraw();
            }


            public override void RefreshTheme()
            {
                base.RefreshTheme();
                this._redraw();
            }
        }
    }
}
