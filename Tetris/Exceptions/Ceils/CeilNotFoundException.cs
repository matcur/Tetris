using System;
using System.Collections.Generic;
using System.Text;
using Tetris.Ui.UiElements;

namespace Tetris.Exceptions.Ceils
{
    class CeilNotFoundException : Exception
    {
        public override string Message => $"Figure is not found. For ceil row = {ceil.Row}, column = {ceil.Column}";

        private readonly Ceil ceil;

        public CeilNotFoundException(Ceil ceil)
        {
            this.ceil = ceil;
        }
    }
}
