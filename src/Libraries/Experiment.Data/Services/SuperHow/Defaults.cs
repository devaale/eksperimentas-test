using System;
using System.Collections.Generic;
using System.Text;

namespace Experiment.Data.Services.SuperHow{
	public class Defaults
	{
		// Blockchain / SuperHow
		public const int DEFAULT_TOKENS = 5;
		public const int TOKENS_MULTIPLIER = 1000000;
		public const int DEFAULT_AMOUNT = TOKENS_MULTIPLIER * DEFAULT_TOKENS;

		public const string ENERGUS_MOSAIC_NAME = "symbol.xym";
		public const string ENERGUS_MOSAIC_ID = "6BED913FA20223F8";

		public const string BLOCKCHAIN_ACCOUNT_PATTERN = "https://symbol.fyi/accounts/{0}";
	}
}
