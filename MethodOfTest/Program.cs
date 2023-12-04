using MethodOf;
using System.Reflection;
using static MethodOf.MethodOfProvider;

namespace MethodOfTest {
	internal class Program {
		static void Main(string[] args) {
			MethodBase mtd = methodof(DoVeryCoolThing);
			Console.WriteLine(mtd.ToString());

			// Test the cache.
			mtd = methodof(DoVeryCoolThing);

			methodof((int t) => false);

			Console.ReadKey();
		}

		public static bool DoVeryCoolThing(string rofl, int g) {
			return false;
		}
	}
}