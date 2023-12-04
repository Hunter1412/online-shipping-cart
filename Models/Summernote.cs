using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingCart.Models
{
    public class Summernote
    {
        public Summernote(string iDEditor, bool loadLibrary = true)
        {
            IDEditor = iDEditor;
            LoadLibrary = loadLibrary;
        }

        public string IDEditor { get; set; }

        public bool LoadLibrary { get; set; }
        public int Height { get; set; } = 120;

        public string Toolbar { get; set; } = @"[
                ['style', ['bold', 'italic', 'underline', 'clear']],
                ['font', ['strikethrough', 'superscript', 'subscript']],
                ['fontsize', ['fontsize']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['height', ['height']]
            ]";





    }
}