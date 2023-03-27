using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xe.AcrylicView.Platforms.Android
{
    public interface IBlurImpl
    {
        bool Prepare(Context context, Bitmap buffer, float radius);

        void Release();

        void Blur(Bitmap input, Bitmap output);
    }

    public class EmptyBlurImpl : IBlurImpl
    {
        public bool Prepare(Context context, Bitmap buffer, float radius)
        {
            return false;
        }

        public void Release()
        {
        }

        public void Blur(Bitmap input, Bitmap output)
        {
        }
    }
}
