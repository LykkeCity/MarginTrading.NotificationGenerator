using System;

namespace MarginTrading.NotificationGenerator.Core.Domain.Interfaces
{
	public interface IOvernightSwapHistory : IOvernightSwapState
	{
		bool IsSuccess { get; }
		Exception Exception { get; }
	}
}