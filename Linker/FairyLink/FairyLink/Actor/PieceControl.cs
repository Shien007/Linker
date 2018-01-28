using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FairyLink.Device;

namespace FairyLink.Actor
{
    class PieceControl
    {
        private List<Piece> pieces;     //分割された部分を保存する用

        public PieceControl() {
            pieces = new List<Piece>();
        }

        public void Add(Piece piece) {
            pieces.Add(piece);
        }

        public void Update() {
            foreach (var p in pieces) {
                p.Update();
            }
            pieces.RemoveAll(p => p.IsDeath);
        }



        public void Draw(Renderer renderer) {
            foreach (var p in pieces) {
                p.Draw(renderer);
            }
        }

    }
}
