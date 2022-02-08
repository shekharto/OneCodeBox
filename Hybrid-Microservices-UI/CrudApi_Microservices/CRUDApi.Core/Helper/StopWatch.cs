using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.Transaction.CRUDApi.Core.Helper
{
	public class StopWatch
	{
		private DateTime mdtStart, mdtEnd;
		private TimeSpan mts;

		public StopWatch() { }

		public static StopWatch StartNew()
		{
			StopWatch sw = new StopWatch();
			sw.Start();
			return sw;
		}

		private static List<double> dblRunningTotal = new List<double>();

		public static void Reset()
		{
			dblRunningTotal.Clear();
		}

		public static void AddToTotal(double seconds)
		{
			dblRunningTotal.Add(seconds);
		}

		public static double GetAverage()
		{
			return dblRunningTotal.Average();
		}

		public static double GetMin()
		{
			return dblRunningTotal.Min();
		}

		public static double GetMax()
		{
			return dblRunningTotal.Max();
		}

		public void Start()
		{
			mdtStart = DateTime.Now;
		}

		public void Stop()
		{
			mdtEnd = DateTime.Now;
		}

		public DateTime StartTime { get { return mdtStart; } }
		public DateTime EndTime { get { return mdtEnd; } }

		public TimeSpan Elapsed
		{
			get
			{
				return (mdtEnd - mdtStart);
			}
		}

		public double TotalSeconds
		{
			get
			{
				mts = mdtEnd - mdtStart;
				return mts.TotalSeconds;
			}
		}
	}
}
