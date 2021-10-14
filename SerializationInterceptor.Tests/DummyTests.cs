using SerializationInterceptor.Tests.Attributes;
using SerializationInterceptor.Tests.Utils;
using Xunit;

namespace SerializationInterceptor.Tests
{
	public class DummyTests
	{
		[Fact]
		public void Dummy()
		{
			var obj = GetObj();
			DummyInterceptor.Serialize(obj);
			var adasdasdas = TypeDefinitionSerializer.Serialize<DummyType>();
		}

		private static DummyType GetObj()
		{
			return new DummyType();
		}
	}

	public class DummyType
	{
		[DummyInterceptor]
		[Dummy(new Dummy[] { (Dummy)9, Dummy.V1, Dummy.V2, default }, (Dummy)(-8), null)]
		public object DummyProp { private get; set; }
	}
}
