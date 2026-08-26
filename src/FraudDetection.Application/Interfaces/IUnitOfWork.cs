using System;
using System.Collections.Generic;
using System.Text;

namespace FraudDetection.Application.Interfaces
{
	public interface IUnitOfWork
	{
		Task SaveChangesAsync();
	}
}
