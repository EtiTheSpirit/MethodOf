using System.Reflection;

namespace Test {
	internal class Program {
		static void Main(string[] args) {
			MethodBase method = MethodOf.Test.methodof(DoThing);
			method.Invoke(null, null);
			Console.ReadKey();
		}

		public static void DoThing() {
			Console.WriteLine("Wow.");
		}
	}
}