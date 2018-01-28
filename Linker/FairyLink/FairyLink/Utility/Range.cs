using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Utility
{
    class Range
    {
        private int first;  //表示始める長方形の番号
        private int end;   //表示終わる長方形の番号

        public Range(int first, int end) {
            this.first = first;
            this.end = end;
        }

        public int First {
            get { return first; }
        }

        public int End {
            get { return end; }
        }

        //今表示したい長方形の番号をチェックする
        private bool IsWithIn(int num) {
            if (num < first) { return false; }
            if (num > end) { return false; }
            return true;
        }

        //表示したい範囲は違法かどうかをチェックする
        public bool IsOutOfRange() {
            return first > end;
        }

        //今表示したい長方形は範囲内かどうかをチェックする
        public bool IsOutOfRange(int num) {
            return !IsWithIn(num);
        }

    }
}
