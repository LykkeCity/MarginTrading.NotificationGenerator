using System;

namespace MarginTrading.NotificationGenerator.Core.Domain.Interfaces
{
	public interface IOvernightSwapState
	{
		string ClientId { get; }
		string AccountId { get; }
		string Instrument { get; }
		OrderDirection? Direction { get; }
		DateTime Time { get; }
	 	decimal Volume { get; }
		decimal Value { get; }
		decimal SwapRate { get; }
		string OpenOrderId { get; }
	}
}