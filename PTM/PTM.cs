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

        [Parameter("Risk to Reward Ratio", DefaultValue = 1.0)]
        public double RiskRewardRatio { get; set; }

        [Parameter("Line Width", DefaultValue = 1)]
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

        TextBlock text;
        bool isReady = false;

        protected override void OnStart()
        {
            text = new TextBlock();
            text.IsHitTestVisible = false;
            text.Text = "PTM v1.0.0";
            Chart.AddControl(text);
            Chart.MouseMove += OnMouseMove;
            Chart.MouseUp += OnMouseUp;
        }

        protected override void OnStop()
        {
            Chart.MouseMove -= OnMouseMove;
        }

        private void OnMouseMove(ChartMouseEventArgs args)
        {
            if (!(args.CtrlKey && args.ShiftKey))
            {
                Chart.RemoveObject("StopLoss");
                Chart.RemoveObject("TakeProfit");
                isReady = false;
                return;
            }

            // 1️⃣ Determine trade direction & entry price
            bool isSell = args.YValue > Symbol.Ask;
            double entryPrice = isSell ? Symbol.Bid : Symbol.Ask;

            // 2️⃣ SL price (mouse-controlled)
            double slPrice = args.YValue;

            // 3️⃣ Risk in price units
            double risk = Math.Abs(entryPrice - slPrice);

            if (risk <= Symbol.TickSize)
                return;

            // 4️⃣ TP price (derived, not mouse-controlled)
            double tpPrice = isSell
                ? entryPrice - (risk * RiskRewardRatio)
                : entryPrice + (risk * RiskRewardRatio);

            // 5️⃣ Draw lines
            Chart.DrawHorizontalLine("StopLoss", slPrice, StopLossLineColor, LineWidth, LineStyle);
            Chart.DrawHorizontalLine("TakeProfit", tpPrice, TakeProfitLineColor, LineWidth, LineStyle);

            isReady = true;
        }

        private void OnMouseUp(ChartMouseEventArgs args)
        {
            if (!(args.CtrlKey && args.ShiftKey && isReady))
                return;

            TradeType tradeType;
            double entryPrice;

            // 1. Decide trade direction
            if (args.YValue > Symbol.Ask)
            {
                tradeType = TradeType.Sell;
                entryPrice = Symbol.Bid;
            }
            else
            {
                tradeType = TradeType.Buy;
                entryPrice = Symbol.Ask;
            }

            // 2. Calculate SL distance in pips
            var slPips = Math.Abs(entryPrice - args.YValue) / Symbol.PipSize;

            if (slPips <= 0)
                return;


            // 3. Calculate TP distance (2R)
            var tpPips = slPips * RiskRewardRatio;

            // 4. Calculate volume using correct risk
            var volume = Symbol.VolumeForProportionalRisk(
                RiskType,
                RiskPercentage,
                slPips,
                RoundingMode.ToNearest
            );

            // 5. Execute order
            ExecuteMarketOrder(
                tradeType,
                Symbol.Name,
                volume,
                Identifier,
                slPips,
                tpPips
            );

            text.Text =
             $"SL: {slPips:F1} pips\n" +
             $"TP: {tpPips:F1} pips\n" +
             $"Vol: {volume}\n" +
             $"{tradeType}";

            isReady = false;
            Chart.RemoveObject("StopLoss");
        }
    }
}