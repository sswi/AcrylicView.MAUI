namespace Xe.AcrylicView.Platforms.Android
{
    internal class JniWeakReference<T>(T target) where T : Java.Lang.Object
    {
        private readonly WeakReference<T> _reference = new(target);

        public bool TryGetTarget(out T target)
        {
            target = null;
            if (_reference.TryGetTarget(out var innerTarget))
            {
                if (innerTarget.Handle != IntPtr.Zero)
                {
                    target = innerTarget;
                }
            }

            return target != null;
        }

        public override string ToString()
        {
            return $"[JniWeakReference] {_reference}";
        }
    }
}