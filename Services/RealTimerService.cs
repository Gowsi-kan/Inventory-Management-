using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Services
{
    public class RealTimerService : IDisposable
    {
        public event Action OnTimeChanged;
        private readonly Timer timer;
        public DateTime CurrentTime { get; private set; }

        public RealTimerService()
        {
            CurrentTime = DateTime.Now;
            timer = new Timer(UpdateTime, null, 0, 1000); // Runs every second
        }

        private void UpdateTime(object state)
        {
            CurrentTime = DateTime.Now;
            OnTimeChanged?.Invoke();
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
