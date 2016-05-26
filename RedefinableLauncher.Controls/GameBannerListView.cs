using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.Applications.Launcher.Controls.Design;
using Redefinable.Applications.Launcher.Controls.DrawingExtensions;


namespace Redefinable.Applications.Launcher.Controls
{
    using GameBannerListViewElements;

    public class GameBannerListView : VariableScaleableControl
    {
        // 非公開フィールド
        private GameBannerCollection items;
        private EventHandler elementClickEventHandler;


        // 非公開フィールド :: コントロール
        private ScaleablePanel bannersTray;
        private LauncherScrollBar scrollBar;
        private bool suspendRefreshItemNow;


        // 非公開静的フィールド
        private static int bannerWidth = 220;
        private static int bannerMargin = 5;
        
        private static int _TrayWidth
        {
            get { return bannerWidth + (bannerMargin * 2); }
        }

        private static int scrollBarWidth = 15;

        private static int _DefaultWidth
        {
            get { return _TrayWidth + scrollBarWidth; }
        }


        // 公開フィールド

        /// <summary>
        /// 現在このコントロールで表示しているGameBannerのコレクションを取得します。
        /// </summary>
        public GameBannerCollection Items
        {
            get { return this.items; }
        }


        // イベント

        public event GameBannerListViewClickEventHandler BannerClick;


        // コンストラクタ

        public GameBannerListView(Point location, int height)
            : base(location, new Size(_DefaultWidth, height))
        {
            // データフィールドの初期化
            this.items = new GameBannerCollection();
            this.suspendRefreshItemNow = false;
            
            // コントロールの初期化
            this._initializeControls();


            // イベントデリゲートの初期化
            this.BannerClick = (sender, e) => { };
            this.elementClickEventHandler = (sender, e) =>
            {
                int index = -1;
                for (int i = 0; i < this.items.Count; i++)
                {
                    if (sender == this.items[i])
                        index = i;
                }

                this.BannerClick(this, new GameBannerListViewClickEventArgs((GameBanner) sender, index));
            };


            // イベントの追加
            GameBannerCollection.ItemEventHandler itemEvent = (sender, e) => { this._refreshItems(); };
            this.items.ItemAdded += itemEvent;
            this.items.ItemRemoved += itemEvent;
            this.scrollBar.ValueChanged += (sender, e) => { this.bannersTray.Top = this.scrollBar.Value * (-1); };

            this.items.ItemAdded += (sender, e) => { e.Item.Click += this.elementClickEventHandler; };
            this.items.ItemRemoved += (sender, e) => { e.Item.Click -= this.elementClickEventHandler; };
        }

        
        // 非公開メソッド

        /// <summary>
        /// このコントロール上のコントロールを初期化します。
        /// </summary>
        private void _initializeControls()
        {
            //this.BackColor = Color.FromArgb(100, Color.DimGray);

            this.bannersTray = new ScaleablePanel();
            this.bannersTray.Location = new Point(0, 0);
            this.bannersTray.Size = new Size(_TrayWidth, 10);
            this.bannersTray.BackColor = Color.FromArgb(60, 60, 60);
            this.Controls.Add(this.bannersTray);

            this.scrollBar = new LauncherScrollBar(new Point(_TrayWidth, 0), new Size(scrollBarWidth, this.DefaultControlSize.Height));
            this.scrollBar.TargetControl = this.bannersTray;
            this.Controls.Add(this.scrollBar);
        }

        /// <summary>
        /// 現在のitemsを元にすべてのコントロールを追加し直します。
        /// </summary>
        private void _refreshItems()
        {
            if (this.suspendRefreshItemNow)
                return;

            this.bannersTray.Controls.Clear();
            
            int scaledMargin = (int) ((double)bannerMargin * this.currentScale);
            int currentTop = scaledMargin;
            for (int i = 0; i < this.items.Count; i++)
            {
                GameBanner b = this.items[i];
                b.CurrentTop = currentTop;
                b.RefreshTheme();
                b.ChangeScale(this.currentScale);
                
                // 近隣コントロールの設定
                if (i >= 1)
                    // 上のコントロールが存在する
                    b.SetNeighborControls(this.items[i - 1], b.DownControl, null, null);
                if (i + 1 < this.items.Count)
                    // 下のコントロールが存在する
                    b.SetNeighborControls(b.UpControl, this.items[i + 1], null, null);

                // デバッグ
                //b.Click += (sender, e) => { MessageBox.Show(b.Text); };

                this.bannersTray.Controls.Add(b);

                currentTop += b.Height;
                currentTop += scaledMargin;
            }

            //currentTop += this.items[this.items.Count - 1].Height;
            //currentTop += scaledMargin;

            this.bannersTray.Height = currentTop;
            this.scrollBar.Value = 0;
        }


        // 公開メソッド

        /// <summary>
        /// Itemsのコレクションに操作をおこなったときに発生するUIの更新処理を停止します。
        /// </summary>
        public void SuspendRefreshItem()
        {
            this.suspendRefreshItemNow = true;
        }

        /// <summary>
        /// Itemsのコレクションに操作をおこなったときに発生するUIの更新処理を再開します。
        /// </summary>
        public void ResumeRefreshItem()
        {
            this.suspendRefreshItemNow = false;
            this._refreshItems();
        }

        /// <summary>
        /// DefaultPoint、DefaultSizeを元に、scaleへスケールを変更します。
        /// </summary>
        /// <param name="scale"></param>
        public override void ChangeScale(float scale)
        {
            this.currentScale = scale;

            // トレイの中身のサイズ変更
            this._refreshItems();
            
            // 正規のChangeScaleの呼び出し
            base.ChangeScale(scale);

            // トレイの幅の変更
            this.bannersTray.Width = this.scrollBar.Left;
        }

        public override void RefreshTheme()
        {
            this.scrollBar.RefreshTheme();
            base.RefreshTheme();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GameBannerListViewClickEventArgs : EventArgs
    {
        // 非公開フィールド
        private GameBanner selectedBanner;
        private int index;


        // 公開フィールド

        public GameBanner SelectedBanner
        {
            get { return this.selectedBanner; }
            set { this.selectedBanner = value; }
        }
        
        public int Index
        {
            get { return this.index; }
            set { this.index = value; }
        }


        // コンストラクタ

        public GameBannerListViewClickEventArgs(GameBanner selectedBanner, int index)
        {
            this.selectedBanner = selectedBanner;
            this.index = index;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void GameBannerListViewClickEventHandler(Object sender, GameBannerListViewClickEventArgs e);
}

namespace Redefinable.Applications.Launcher.Controls.GameBannerListViewElements
{
    /// <summary>
    /// IScaleableControlを実装するもののその中身はPanelと変わらない謎のコントロール
    /// </summary>
    public class ScaleablePanel : Panel, IScaleableControl
    {

        // 公開フィールド :: インターフェイスの明示的実装

        /// <summary>
        /// 利用できません。
        /// </summary>
        float IScaleableControl.CurrentScale
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        Point IScaleableControl.DefaultControlLocation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        Size IScaleableControl.DefaultControlSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        IScaleableControl IScaleableControl.DownControl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        bool IScaleableControl.LauncherControlFocused
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        IScaleableControl IScaleableControl.LeftControl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        IScaleableControl IScaleableControl.RightControl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 利用できません。
        /// </summary>
        IScaleableControl IScaleableControl.UpControl
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        // 公開イベント :: インターフェイスの明示的実装

        /// <summary>
        /// 利用できません。
        /// </summary>
        event ScaleChangedEventHandler IScaleableControl.ScaleChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }


        // コンストラクタ

        public ScaleablePanel()
        {
            this.DoubleBuffered = true;
            this.PreviewKeyDown += ScaleablePanel_PreviewKeyDown;
        }


        // 非公開メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScaleablePanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Object parent = this.Parent;

            if (parent != null)
            {
                // リフレクションを使用して強制的にイベント処理を実行
                // → 非常に強引な方法です。良い子の皆は真似しないでね！

                try
                {
                    // Controls.OnPreviewKeyDown (protected) を呼び出す
                    // その際、メソッドを呼び出すメソッドには parent を指定する
                    // → parent.OnPreviewKeyDown() を本来呼び出せない場所 (クラスの外側) から無理やり呼び出す
                    parent.GetType().InvokeMember(
                        "OnPreviewKeyDown",                             // メソッド名
                        System.Reflection.BindingFlags.InvokeMethod |   // 呼び出しの種類 (実行、非公開、インスタンスから呼び出す動的メソッド)
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance,
                        null,                                           // バインダ (デフォルトのためnull)
                        parent,                                         // 動的メソッドを呼び出すインスタンス
                        new object[] { e });                            // 引数 (このPreviewKeyDownの引数をそのまま渡す)
                }
                catch (MissingMethodException)
                {
                    // メソッドが見つからなかった
                    // →諦める
                }
            }
        }


        // 公開メソッド :: インターフェイスの明示的実装

        /// <summary>
        /// 無視されます。
        /// </summary>
        void IScaleableControl.ChangeScale(float scale)
        {
            // 何もしない
        }

        /// <summary>
        /// 子コントロールでRefreshFocusState()を実行します。
        /// </summary>
        void IScaleableControl.RefreshFocusState()
        {
            foreach (Control control in this.Controls)
                if (control is IScaleableControl)
                    ((IScaleableControl) control).RefreshFocusState();
        }

        /// <summary>
        /// 無視されます。
        /// </summary>
        void IScaleableControl.RefreshTheme()
        {
            // 何もしない
        }
    }
}