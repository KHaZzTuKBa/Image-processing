using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public class ImageLayer
    {
        public Bitmap Image { get; set; }
        public bool IsRedChannel { get; set; } = true;
        public bool IsGreenChannel { get; set; } = true;
        public bool IsBlueChannel { get; set; } = true;
        public int Opacity { get; set; } = 100;
        public string Operation { get; set; } = "Normal";

        public ImageLayer(Bitmap image)
        {
            Image = image;
        }
    }
}
