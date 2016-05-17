using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Redefinable.Applications.Launcher.Controls
{
    /// <summary>
    /// 比率指定で自動スケーリング処理が可能なコントロールを表します。
    /// </summary>
    public interface IScaleableControl
    {
        // 公開プロパティ

        /// <summary>
        /// コントロールの初期の大きさを取得します。
        /// </summary>
        Point DefaultControlLocation { get; }

        /// <summary>
        /// コントロールの初期の大きさを取得します。
        /// </summary>
        Size DefaultControlSize { get; }

        /// <summary>
        /// コントロールの現在の比率を取得します。
        /// </summary>
        float CurrentScale { get; }

        /// <summary>
        /// このコントロールの上側に隣接しているコントロールを取得します。
        /// </summary>
        IScaleableControl UpControl { get; }

        /// <summary>
        /// このコントロールの下側に隣接しているコントロールを取得します。
        /// </summary>
        IScaleableControl DownControl { get; }

        /// <summary>
        /// このコントロールの左側に隣接しているコントロールを取得します。
        /// </summary>
        IScaleableControl LeftControl { get; }

        /// <summary>
        /// このコントロールの右側に隣接しているコントロールを取得します。
        /// </summary>
        IScaleableControl RightControl { get; }
        
        /// <summary>
        /// LauncherPanelのControlsFocusedIndexの値から、現在このコントロールにフォーカスがあるかどうかを取得します。
        /// </summary>
        bool Focused { get; }


        // 公開イベント

        /// <summary>
        /// ChangeScaleが実行された後に発生します。
        /// </summary>
        event ScaleChangedEventHandler ScaleChanged;
        
        
        // 公開メソッド

        /// <summary>
        /// DefaultPoint、DefaultSizeを元に、scaleへスケールを変更します。
        /// 更に子コントロールのコレクションが存在する場合はコレクションを走査し、IScaleable実装のコントロールを検出したらChangeScaleを実行します。
        /// NativeControlを含む場合は、独自にスケール対応用のプログラムを実装してください。
        /// </summary>
        /// <param name="scale"></param>
        void ChangeScale(float scale);

        /// <summary>
        /// コントロールのテーマを現在のテーマで再描画します。
        /// </summary>
        void RefreshTheme();

        /// <summary>
        /// LauncherPanelの
        /// </summary>
        void RefreshFocusState();
    }

    /// <summary>
    /// IScaleableControlでChangeScaleが実行されたときに発生するイベントを処理するメソッドを表します。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ScaleChangedEventHandler(Object sender, ScaleChangedEventArgs e);

    /// <summary>
    /// IScaleableControlでChangeScaleが実行されたときに発生するイベントにデータを提供します。
    /// </summary>
    public class ScaleChangedEventArgs : EventArgs
    {
        // 実装なし
    }
}
