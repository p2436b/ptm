using System;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None, AddIndicators = true)]
    public class PTM : Robot
    {
        [Parameter("Identifier", DefaultValue = "PTrader")]
        public string Identifier { get; set; }

        [Parameter("Risk Percentage", DefaultValue = 1)]
        public double RiskPercentage { get; set; }

        [Parameter("Risk Type", DefaultValue = 1)]
        public ProportionalAmountType RiskType { get; set; }

        [Parameter("Risk to Reward Ratio", DefaultValue = 2.0)]
        public double RiskRewardRatio { get; set; }

        [Parameter("Default Stop Loss Points", DefaultValue = 2)]
        public double DefaultStopLossPoints { get; set; }

        [Parameter("Line Width", DefaultValue = 2)]
        public int LineWidth { get; set; }

        [Parameter("Line Style", DefaultValue = LineStyle.Solid)]
        public LineStyle LineStyle { get; set; }

        // [Parameter("Info Text Color", DefaultValue = "#FF000000")]
        // public Color InfoTextColor { get; set; }

        [Parameter("Take Profit Line Color", DefaultValue = "#FF00BF00")]
        public Color TakeProfitLineColor { get; set; }

        [Parameter("Stop Loss Line Color", DefaultValue = "#FFFF3334")]
        public Color StopLossLineColor { get; set; }

        [Parameter("Left Margin", DefaultValue = 10)]
        public double LeftMargin { get; set; }

        [Parameter("Top Margin", DefaultValue = 0)]
        public double TopMargin { get; set; }

        [Parameter("Right Margin", DefaultValue = 0)]
        public double RightMargin { get; set; }

        [Parameter("Bottom Margin", DefaultValue = 40)]
        public double BottomMargin { get; set; }

        [Parameter("Horizontal Alignment", DefaultValue = HorizontalAlignment.Left)]
        public HorizontalAlignment HorizontalAlignment { get; set; }
        [Parameter("Vertical Alignment", DefaultValue = VerticalAlignment.Bottom)]
        public VerticalAlignment VerticalAlignment { get; set; }

        protected override void OnStart()
        {
            Chart.MouseMove += OnMouseMove;
        }

        protected override void OnTick()
        {
            // Handle price updates here
        }

        protected override void OnStop()
        {
            // Handle cBot stop here
        }

        private void OnMouseMove(ChartMouseEventArgs args)
        {
            if(args.CtrlKey && args.ShiftKey)
            Chart.DrawHorizontalLine("Stop", args.YValue, StopLossLineColor);
            else
            Chart.RemoveObject("Stop");
        }
    }
}