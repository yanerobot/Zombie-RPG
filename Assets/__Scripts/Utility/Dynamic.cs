namespace KK.Utility
{
    public struct Dynamic<T> : IDynamic
    {
        public T value;
        public T defaultVal;

        public Dynamic(T defaultVal)
        {
            value = defaultVal;
            this.defaultVal = defaultVal;
        }

        public void SetDefaultValue()
        {
            value = defaultVal;
        }
    }

    public interface IDynamic
    {
        void SetDefaultValue();
    }

}