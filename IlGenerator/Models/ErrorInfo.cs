using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlGenerator.Models
{
    public class ErrorInfo
    {
        public bool IsWarning;
        public string HighlightMessage;
        public string TextMessage;
        public int Row;
        public int Column;
        public ErrorInfo(bool isWarning, string highlight, string text, int row, int col)
        {
            IsWarning = isWarning;
            HighlightMessage = highlight;
            TextMessage = text;
            Row = row;
            Column = col;
        }
        public ErrorInfo(CompilerError error)
        {
            IsWarning = error.IsWarning;
            Row = error.Line;
            Column = error.Column;
            HighlightMessage = error.ErrorText;
            TextMessage = $"Compilation error {error.ErrorNumber} (line {error.Line}, col {error.Column}): {HighlightMessage}";
        } 
    }
}