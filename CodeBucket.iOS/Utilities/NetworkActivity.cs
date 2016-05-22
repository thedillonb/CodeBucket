using CodeBucket.Services;

namespace CodeBucket.Utilities
{
    public class LoadingIndicator
    {
        private readonly LoadingIndicatorService _loading = new LoadingIndicatorService();
        private int _value;

        public void Up()
        {
            _value++;
            _loading.Up();
        }

        public void Down()
        {
            if (_value == 0)
                return;
            _value--;
            _loading.Down();
        }

        ~LoadingIndicator()
        {
            for (var i = 0; i < _value; i++)
            {
                _loading.Down();
            }
        }
    }
}

