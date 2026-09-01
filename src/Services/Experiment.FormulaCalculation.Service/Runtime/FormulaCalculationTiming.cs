namespace Experiment.FormulaCalculation.Service
{
	internal static class FormulaCalculationTiming
	{
		/// <summary>
		/// How many ms to sleep in total
		/// </summary>
		public const int SleepTime = 1 * 60 * 1000;

		/// <summary>
		/// Single sleep moment, between which we will check do service started
		/// </summary>
		public const int SleepSingle = 250;

		/// <summary>
		/// Amount of single sleep times at the end of the single heartbeat
		/// </summary>
		public const int SleepTiles = SleepTime / SleepSingle;

		/// <summary>
		/// Delay in milisseconds after each datapoint processing
		/// </summary>
		public const int SleepAfterDatapointProcessing = 0x80; // 128

		/// <summary>
		/// Delay after next order's step, when in cycle order's number increase
		/// </summary>
		public const int SleepBeforeNextOrder = 0x800; // 2048
	}
}
