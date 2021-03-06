﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Redefinable;
using Redefinable.IO;
using Redefinable.Applications.Launcher.Controls.Design;

using Stream = System.IO.Stream;
using StreamWriter = System.IO.StreamWriter;


namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// ランチャーのメインパネル機能を提供します。
    /// </summary>
    public class LauncherPanel : UserControl, IScaleableControl
    {
        // 非公開フィールド
        private float currentScale;
        private LauncherTheme theme;
        private IScaleableControl focusedControl;

        private DebugConsoleHelper consoleHelper;
        private StreamWriter debugOut;


        // 非公開フィールド :: コントロール
        private GameBannerListView gameBannerListView;
        private LauncherButton genreSelectButton;
        private LauncherButton controllerSelectButton;
        private LauncherButton helpButton;
        private TitleBar titleBar;
        private MiddlePanel middlePanel;
        private SlideshowPanel slideshowPanel;
        private DescriptionPanel descriptionPanel;
        private LauncherButton playButton;
        private LauncherButton operationButton;
        private LauncherButton movieButton;
        private ChildSelectPanel genreSelectPanel;
        private ChildSelectPanel controllerSelectPanel;
        private ChildOperationPanel launcherHelpPanel;

        #region 公開フィールド
        // 公開フィールド

        /// <summary>
        /// デフォルトスケール時の位置を取得します。
        /// </summary>
        public Point DefaultControlLocation
        {
            get { return new Point(0, 0); }
        }

        /// <summary>
        /// デフォルトスケール時のサイズを取得します。
        /// </summary>
        public Size DefaultControlSize
        {
            get { return LauncherPanelSize; }
        }

        /// <summary>
        /// スケールを取得・設定します。
        /// </summary>
        public float CurrentScale
        {
            get { return this._getScale(); }
            set { this._setScale(value); }
        }

        /// <summary>
        /// テーマを取得・設定します。
        /// </summary>
        public LauncherTheme Theme
        {
            get { return this._getLaucncherTheme(); }
            set { this._setLauncherTheme(value); }
        }

        /// <summary>
        /// 現在LauncherPanel上でフォーカスがあるIScaleableControlを取得します。
        /// </summary>
        public IScaleableControl FocuedControl
        {
            get { return this.focusedControl; }
        }

        /// <summary>
        /// ConsoleHelperを設定・取得します。
        /// </summary>
        public DebugConsoleHelper ConsoleHelper
        {
            get { return this.consoleHelper; }
            set { this.consoleHelper = value; }
        }
        

        // 公開フィールド・プロパティ :: 明示的な実装

        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.UpControl
        {
            get { return this; }
        }
        
        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.DownControl
        {
            get { return this; }
        }
        
        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.LeftControl
        {
            get { return this; }
        }
        
        /// <summary>
        /// このLauncherPanel自身を返します。
        /// </summary>
        IScaleableControl IScaleableControl.RightControl
        {
            get { return this; }
        }

        /// <summary>
        /// 使用できません。
        /// </summary>
        bool IScaleableControl.LauncherControlFocused
        {
            get { throw new NotImplementedException(); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public GameBannerListView GameBannerListView
        {
            get { return this.gameBannerListView; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LauncherButton GenreSelectButton
        {
            get { return this.genreSelectButton; }
        }

        public LauncherButton ControllerSelectButton
        {
            get { return this.controllerSelectButton; }
        }

        public LauncherButton HelpButton
        {
            get { return this.helpButton; }
        }

        public TitleBar TitleBar
        {
            get { return this.titleBar; }
        }

        public MiddlePanel MiddlePanel
        {
            get { return this.middlePanel; }
        }

        public SlideshowPanel SlideshowPanel
        {
            get { return this.slideshowPanel; }
        }

        public DescriptionPanel DescriptionPanel
        {
            get { return this.descriptionPanel; }
        }

        public LauncherButton PlayButton
        {
            get { return this.playButton; }
        }
        #endregion

        public LauncherButton OperationButton
        {
            get { return this.operationButton; }
        }

        public LauncherButton MovieButton
        {
            get { return this.movieButton; }
        }

        public ChildSelectPanel GenreSelectPanel
        {
            get { return this.genreSelectPanel; }
        }

        public ChildSelectPanel ControllerSelectPanel
        {
            get { return this.controllerSelectPanel; }
        }
        
        public ChildOperationPanel LauncherHelpPanel
        {
            get { return this.launcherHelpPanel; }
        }
        

        // 公開静的フィールド

        public static Size LauncherPanelSize
        {
            get { return new Size(1080, 810); }
        }


        // 公開イベント

        /// <summary>
        /// スケールが変更された際に発生します。
        /// </summary>
        public event ScaleChangedEventHandler ScaleChanged;


        // コンストラクタ

        /// <summary>
        /// 新しいLauncherPanelクラスのインスタンスを初期化します。
        /// </summary>
        public LauncherPanel()
        {
            // データフィールドの初期化
            this.currentScale = 1.0f;
            this.focusedControl = null;
            this._initializeDebugOutput();

            // コントロールの初期化
            this._initializeControls();

            // イベントハンドラの初期化
            this.ScaleChanged = (sender, e) => { };

            // イベントの追加
            //this.Click += (sender, e) => { this.ChangeScale(this.currentScale -= 0.1f); };
            this.PreviewKeyDown += LauncherPanel_PreviewKeyDown;
            //this.KeyDown += (sender, e) => { Console.WriteLine("a"); };
            this.genreSelectButton.Click += (sender, e) => { this.genreSelectPanel.ChildPanelShow(); };
            this.controllerSelectButton.Click += (sender, e) => { this.controllerSelectPanel.ChildPanelShow(); };
            //this.helpButton.Click += (sender, e) => { this.launcherHelpPanel.ChildPanelShow(); };
            //this.operationButton.Click += (sender, e) => { this.launcherHelpPanel.ChildPanelShow(); };
        }


        // 非公開メソッド

        private void _initializeDebugOutput()
        {
            this.consoleHelper = null;
            
            StringEventStream stream = new StringEventStream(Encoding.UTF8);
            stream.Wrote += (sender, e) =>
            {
                if (this.consoleHelper == null)
                    return;

                this.consoleHelper.Out.Write(e.Text);
            };
            
            this.debugOut = new StreamWriter(stream);
            this.debugOut.AutoFlush = true;
        }

        private void _initializeControls()
        {
            this.debugOut.WriteLine("LauncherPanel上のコントロールを初期化します。");

            // ランチャーパネル
            this.Location = this.DefaultControlLocation;
            this.Size = this.DefaultControlSize;
            this.DoubleBuffered = true;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // テーマ
            this.theme = LauncherTheme.GetDefaultTheme(); // デバッグ用, サンプルテーマ
            
            this.gameBannerListView = new GameBannerListView(new Point(20, 20), 600);
            this.Controls.Add(this.gameBannerListView);

            this.genreSelectButton = new LauncherButton(new Point(20, 640), new Size(this.gameBannerListView.Width, 40));
            this.genreSelectButton.Text = "ジャンル";
            this.genreSelectButton.ApplyThemesHeight = true;
            this.Controls.Add(this.genreSelectButton);

            this.controllerSelectButton = new LauncherButton(new Point(20, 690), new Size(this.genreSelectButton.Width, 40));
            this.controllerSelectButton.Text = "コントローラ";
            this.controllerSelectButton.ApplyThemesHeight = true;
            this.Controls.Add(this.controllerSelectButton);
            
            this.helpButton = new LauncherButton(new Point(20, 740), new Size(this.genreSelectButton.Width, 40));
            this.helpButton.Text = "ヘルプ";
            this.helpButton.ApplyThemesHeight = true;
            this.Controls.Add(this.helpButton);

            this.titleBar = new TitleBar(new Point(280, 20));
            this.titleBar.RefreshFields("P-000", "Not specified.");
            this.Controls.Add(this.titleBar);
            
            this.middlePanel = new MiddlePanel(new Point(280, 100));
            this.Controls.Add(this.middlePanel);
            
            this.slideshowPanel = this.middlePanel.SlideshowPanel;

            this.descriptionPanel = new DescriptionPanel(new Point(280, 520));
            this.descriptionPanel.Message = "This is a sample message.\nLine 2\nLine 3";
            this.Controls.Add(this.descriptionPanel);

            this.movieButton = new LauncherButton(new Point(370, 650), new Size(180, 40));
            this.movieButton.Text = "デモ動画を見る";
            this.movieButton.ApplyThemesHeight = true;
            this.Controls.Add(this.movieButton);

            this.operationButton = new LauncherButton(new Point(570, 650), new Size(180, 40));
            this.operationButton.Text = "操作説明";
            this.operationButton.ApplyThemesHeight = true;
            this.Controls.Add(this.operationButton);

            this.playButton = new LauncherButton(new Point(770, 650), new Size(180, 40));
            this.playButton.Text = "プレイする";
            this.playButton.ApplyThemesHeight = true;
            this.Controls.Add(this.playButton);

            this.genreSelectPanel = new ChildSelectPanel();
            this.Controls.Add(this.genreSelectPanel);

            this.controllerSelectPanel = new ChildSelectPanel();
            this.Controls.Add(this.controllerSelectPanel);

            this.launcherHelpPanel = new ChildOperationPanel();
            //this.launcherHelpPanel.SetFields("京成津田沼", "　京成津田沼駅（けいせいつだぬまえき）は、千葉県習志野市津田沼三丁目1番1号にある京成電鉄・新京成電鉄の駅である。京成の駅番号はKS26、新京成の駅番号はSL24。\n　京成電鉄が業務を行う同社と新京成電鉄の共同使用駅である。京成電鉄の本線と千葉線、新京成電鉄の新京成線が乗り入れている。東日本旅客鉄道（JR東日本）津田沼駅は、新京成線の新津田沼駅が接続駅である。日中、京成上野方面からの普通列車の半数は当駅で折り返す。京成千葉線の起点かつ新京成線の終点であるが、日中は新京成線の半数程度の列車が、当駅を介して京成千葉線へ片乗り入れ直通運転を行っている[1]。また、朝晩には千葉線から本線上野方面へ直通する列車も設定されている。");
            this.Controls.Add(this.launcherHelpPanel);


            // リストビューのデバッグ
            /*
            this.gameBannerListView.SuspendRefreshItem();
            this.gameBannerListView.Text = "表示する作品のジャンルを選択してください。\nすべて選択すると、すべて表示されます。";
            for (int i = 0; i < 40; i++)
            {
                this.gameBannerListView.Items.Add(new GameBanner() { Text = String.Format("{0:00}個目のバナー", i + 1) });
            }
            this.gameBannerListView.ResumeRefreshItem();
            this.gameBannerListView.BannerClick += (sender, e) =>
            {
                this.titleBar.RefreshFields(String.Format("P-{0:000}", e.Index + 1), e.SelectedBanner.Text);
            };

            // スライドショーパネルのデバッグ
            SlideshowPanelImageCollection simages = new SlideshowPanelImageCollection();
            Image simage;
            simage = new Bitmap(640, 360);
            using (var g = Graphics.FromImage(simage))
                g.FillRectangle(Brushes.Red, 0, 0, simage.Width, simage.Height);
            simages.Add(simage);
            simage = new Bitmap(360, 640);
            using (var g = Graphics.FromImage(simage))
                g.FillRectangle(Brushes.Blue, 0, 0, simage.Width, simage.Height);
            simages.Add(simage);
            this.slideshowPanel.SetImages(simages);
            this.slideshowPanel.Start();

            this.genreSelectPanel.SuspendRefreshControls();
            for (int i = 0; i < 20; i++)
            {
                this.genreSelectPanel.Items.Add(new ChildSelectPanelItem(String.Format("項目 ({0:00})", i + 1)));
            }
            this.genreSelectPanel.ResumeRefreshControls();
            */

            // テーマの適用 → コア内で正規テーマを描画
            //this.RefreshTheme();
        }
        
        private void _setScale(float value)
        {
            this.debugOut.WriteLine("LauncherPanelのスケールを変更します。");

            this.currentScale = value;

            // 自身のサイズ変更
            this.Width = (int)((float)this.DefaultControlSize.Width * value);
            this.Height = (int)((float)this.DefaultControlSize.Height * value);
            
            // 子コントロールのサイズ変更
            foreach (Control cont in this.Controls)
            {
                if (cont is IScaleableControl)
                {
                    IScaleableControl scont = (IScaleableControl) cont;
                    scont.ChangeScale(value);
                }
            }

            this.ScaleChanged(this, new ScaleChangedEventArgs());
        }
        
        private float _getScale()
        {
            return this.currentScale;
        }

        private LauncherTheme _getLaucncherTheme()
        {
            return this.theme;
        }

        private void _setLauncherTheme(LauncherTheme value)
        {
            this.theme = value;
            this.RefreshTheme();
        }


        // 非公開メソッド :: コントロールイベント
        
        private void LauncherPanel_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            this.debugOut.WriteLine("LauncherPanelでPreviewKeyDownを検知しました: {0}", e.KeyCode);
            
            switch (e.KeyCode)
            {
                case Keys.Up:
                    this.focusedControl = this.focusedControl.UpControl;
                    break;
                case Keys.Down:
                    this.focusedControl = this.focusedControl.DownControl;
                    break;
                case Keys.Left:
                    this.focusedControl = this.focusedControl.LeftControl;
                    break;
                case Keys.Right:
                    this.focusedControl = this.focusedControl.RightControl;
                    break;
                case Keys.Enter:
                    if (this.focusedControl is Control)
                    {
                        // コントロールがクリックされたことにする。
                        Control cont = (Control)this.focusedControl;

                        // Controls.OnPreviewKeyDown (protected) を呼び出す
                        // その際、メソッドを呼び出すメソッドには parent を指定する
                        // → parent.OnPreviewKeyDown() を本来呼び出せない場所 (クラスの外側) から無理やり呼び出す
                        cont.GetType().InvokeMember(
                            "OnClick",                                      // メソッド名
                            System.Reflection.BindingFlags.InvokeMethod |   // 呼び出しの種類 (実行、非公開、インスタンスから呼び出す動的メソッド)
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance,
                            null,                                           // バインダ (デフォルトのためnull)
                            cont,                                           // 動的メソッドを呼び出すインスタンス
                            new object[] { new EventArgs() });              // 引数
                    }
                    break;
            }

            this.RefreshFocusState();
        }


        // 限定公開メソッド

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            this.Focus();
        }

        protected StreamWriter GetDebugWriter()
        {
            return this.debugOut;
        }


        // 公開メソッド

        /// <summary>
        /// スケールを設定します。
        /// </summary>
        /// <param name="scale"></param>
        public void ChangeScale(float scale)
        {
            this._setScale(scale);
        }

        /// <summary>
        /// コントロールのテーマを現在のテーマで再描画します。
        /// </summary>
        public void RefreshTheme()
        {
            this.debugOut.WriteLine("LauncherPanelのコントロールテーマを再描画します。");

            // 自身のテーマを更新
            this.BackgroundImage = this.theme.PanelTheme.BackgroundImage;

            // チャイルドのテーマを更新
            foreach (Control cont in this.Controls)
                if (cont is IScaleableControl)
                    ((IScaleableControl) cont).RefreshTheme();

            GC.Collect();
        }

        /// <summary>
        /// FocusedControlが変化した際に実行され、このLauncherPanel上のすべてのコントロールにおいて、RefreshFocusState()を実行します。
        /// </summary>
        public void RefreshFocusState()
        {
            this.debugOut.WriteLine("フォーカスが変更されました。: {0}", this.FocuedControl);

            foreach (var c in this.Controls)
                if (c is IScaleableControl)
                {
                    this.debugOut.WriteLine(c.ToString());
                    ((IScaleableControl)c).RefreshFocusState();
                }

            GC.Collect();
        }

        /// <summary>
        /// FocusedControlを設定します。
        /// </summary>
        /// <param name="cont"></param>
        public void SetFocus(IScaleableControl cont)
        {
            this.focusedControl = cont;
            this.RefreshFocusState();
        }
        


        // 公開メソッド :: インタフェースの明示的な実装
        

    }
}
