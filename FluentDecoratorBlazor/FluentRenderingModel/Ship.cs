using System.Collections.Generic;
using System.Linq;

namespace FluentDecoratorBlazor.FluentRenderingModel
{

    public enum DisembarkStatus
    {
        Initial = 0,
        WaitOnCaptain = 1,
        WaitOnSteerman = 2,
        DisembarkCanStart = 3,
        DisembarkFinished = 4
    }
    public class Ship
    {
        public const double MaxDisembarkWeight = 24D;

        public Ship()
        {
            Status = DisembarkStatus.Initial;
        }
        public string Name { get; set; }
        public DisembarkStatus Status { get; set; }
        public List<StockItem> Stock { get; set; } = new List<StockItem>();
        public Captain Captain { get; set; }
        public Steersman Steersman { get; set; }

        public Person NextInRowToApproveDisembark()
        {
            if (!Captain.ApprovedDisembark)
            {
                return Captain;
            }
            return Steersman;
        }
        public void EvaluateStatus()
        {

            if (Captain == null || Steersman == null || Stock == null || !Stock.Any())
            {
                Status = DisembarkStatus.Initial;
                return;
            }

            if (Status == DisembarkStatus.DisembarkCanStart && !Stock.Any(itm => itm.Weight < MaxDisembarkWeight))
            {
                Status = DisembarkStatus.DisembarkFinished;
                return;
            }

            if (Steersman.ApprovedDisembark)
            {
                Status = DisembarkStatus.DisembarkCanStart;
                return;
            }

            if (Captain.ApprovedDisembark)
            {
                Status = DisembarkStatus.WaitOnSteerman;
                return;
            }
        }
    }
}
