using Newtonsoft.Json;
using SerializationInterceptor.Benchmark.Attributes;
using SerializationInterceptor.Benchmark.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SerializationInterceptor.Benchmark
{
	public class NewtonsoftJsonBenchmark
	{
		private const int Padding = 30;
		private const int Iterations = 100;

		public static void DoBenchmark()
		{
			Console.WriteLine(new string('-', 42));
			Console.WriteLine(new string('-', 8) + "<<<<< NewtonsoftJson >>>>>" + new string('-', 8));
			Console.WriteLine(new string('-', 42));
			DoSerializationBenchmark();
			DoDeserializationBenchmark();
		}

		private static void DoSerializationBenchmark()
		{
			Console.WriteLine(new string(' ', 8) + "S E R I A L I Z A T I O N" + new string(' ', 8));
			Console.WriteLine();
			DoSimpleSerializationBenchmark(out double simpleBefore, out double simpleAfter);
			DoComplexSerializationBenchmark(out double complexBefore, out double complexAfter);
			Console.WriteLine("s i m p l e".PadRight(Padding) + "c o m p l e x".PadRight(Padding));
			Console.WriteLine();
			Console.WriteLine($"Before {simpleBefore:N}ms".PadRight(Padding) + $"Before {complexBefore:N}ms".PadRight(Padding));
			Console.WriteLine($"After {simpleAfter:N}ms".PadRight(Padding) + $"After {complexAfter:N}ms".PadRight(Padding));
			Console.WriteLine($"Rate {(simpleAfter / simpleBefore):.00}".PadRight(Padding) + $"Rate {(complexAfter / complexBefore):.00}".PadRight(Padding));
			Console.WriteLine();
		}

		private static void DoDeserializationBenchmark()
		{
			Console.WriteLine(new string('-', 42));
			Console.WriteLine(new string(' ', 7) + "D E S E R I A L I Z A T I O N" + new string(' ', 7));
			Console.WriteLine();
			DoSimpleDeserializationBenchmark(out double simpleBefore, out double simpleAfter);
			DoComplexDeserializationBenchmark(out double complexBefore, out double complexAfter);
			Console.WriteLine("s i m p l e".PadRight(Padding) + "c o m p l e x".PadRight(Padding));
			Console.WriteLine();
			Console.WriteLine($"Before {simpleBefore:N}ms".PadRight(Padding) + $"Before {complexBefore:N}ms".PadRight(Padding));
			Console.WriteLine($"After {simpleAfter:N}ms".PadRight(Padding) + $"After {complexAfter:N}ms".PadRight(Padding));
			Console.WriteLine($"Rate {(simpleAfter / simpleBefore):.00}".PadRight(Padding) + $"Rate {(complexAfter / complexBefore):.00}".PadRight(Padding));
			Console.WriteLine();
		}

		private static void DoSimpleSerializationBenchmark(out double simpleBefore, out double simpleAfter)
		{
			var watch = new Stopwatch();
			var simpleObj = GetSimpleObj();
			JsonConvert.SerializeObject(simpleObj, simpleObj.GetType(), null); // warm up
			watch.Start();
			for (int i = 0; i < Iterations; i++) JsonConvert.SerializeObject(simpleObj, simpleObj.GetType(), null);
			watch.Stop();
			simpleBefore = watch.Elapsed.TotalMilliseconds;
			watch.Restart();
			for (int i = 0; i < Iterations; i++) NewtonsoftJsonSerializationUtils.InterceptSerialization(simpleObj);
			watch.Stop();
			simpleAfter = watch.Elapsed.TotalMilliseconds;
		}

		private static void DoComplexSerializationBenchmark(out double complexBefore, out double complexAfter)
		{
			var watch = new Stopwatch();
			var complexObj = GetComplexObj();
			JsonConvert.SerializeObject(complexObj, complexObj.GetType(), null); // warm up
			watch.Restart();
			for (int i = 0; i < Iterations; i++) JsonConvert.SerializeObject(complexObj, complexObj.GetType(), null);
			watch.Stop();
			complexBefore = watch.Elapsed.TotalMilliseconds;
			watch.Restart();
			for (int i = 0; i < Iterations; i++) NewtonsoftJsonSerializationUtils.InterceptSerialization(complexObj);
			watch.Stop();
			complexAfter = watch.Elapsed.TotalMilliseconds;
		}

		private static void DoSimpleDeserializationBenchmark(out double simpleBefore, out double simpleAfter)
		{
			var watch = new Stopwatch();
			var simpleString = GetSimpleString();
			JsonConvert.DeserializeObject(simpleString, typeof(SimpleRoot)); // warm up
			watch.Start();
			for (int i = 0; i < Iterations; i++) JsonConvert.DeserializeObject(simpleString, typeof(SimpleRoot));
			watch.Stop();
			simpleBefore = watch.Elapsed.TotalMilliseconds;
			watch.Restart();
			for (int i = 0; i < Iterations; i++) NewtonsoftJsonSerializationUtils.InterceptDeserialization<SimpleRoot>(simpleString);
			watch.Stop();
			simpleAfter = watch.Elapsed.TotalMilliseconds;
		}

		private static void DoComplexDeserializationBenchmark(out double complexBefore, out double complexAfter)
		{
			var watch = new Stopwatch();
			var complexString = GetComplexString();
			JsonConvert.DeserializeObject(complexString, typeof(ComplexRoot)); // warm up
			watch.Restart();
			for (int i = 0; i < Iterations; i++) JsonConvert.DeserializeObject(complexString, typeof(ComplexRoot));
			watch.Stop();
			complexBefore = watch.Elapsed.TotalMilliseconds;
			watch.Restart();
			for (int i = 0; i < Iterations; i++) NewtonsoftJsonSerializationUtils.InterceptDeserialization<ComplexRoot>(complexString);
			watch.Stop();
			complexAfter = watch.Elapsed.TotalMilliseconds;
		}

		#region simple data
		private static SimpleRoot GetSimpleObj()
		{
			return new SimpleRoot
			{
				Data = new Data
				{
					Channels = new List<Channel>
					{
						new Channel
						{
							ChannelName = "name",
							ChannelType = "type",
							Unit = "unit",
							Values = new Values
							{
								Value=122.45
							}
						}
					}
				}
			};
		}

		private static string GetSimpleString()
		{
			return "{\"data\":{\"channels\":[{\"channelName\":\"name\",\"channelType\":\"type\",\"unit\":\"unit\",\"values\":{\"value\":122.45}}]}}";
		}
		#endregion

		#region simple type
		public class SimpleRoot
		{
			[JsonPropertyInterceptor("data")]
			[JsonProperty("data")]
			public Data Data { get; set; }
		}

		public class Data
		{
			[JsonPropertyInterceptor("channels")]
			[JsonProperty("channels")]
			public IEnumerable<Channel> Channels { get; set; }
		}

		public class Channel
		{
			[JsonPropertyInterceptor("channelName")]
			[JsonProperty("channelName")]
			public string ChannelName { get; set; }

			[JsonPropertyInterceptor("channelType")]
			[JsonProperty("channelType")]
			public string ChannelType { get; set; }

			[JsonPropertyInterceptor("unit")]
			[JsonProperty("unit")]
			public string Unit { get; set; }

			[JsonPropertyInterceptor("values")]
			[JsonProperty("values")]
			public Values Values { get; set; }
		}

		public class Values
		{
			[JsonPropertyInterceptor("value")]
			[JsonProperty("value")]
			public double? Value { get; set; }
		}
		#endregion

		#region complex data
		private static ComplexRoot GetComplexObj()
		{
			var structs = new S[]
			{
				new S
				{
					GenericClassCOfStructSProp = new C<S>
					{
						ArrayOfStructSProp = Array.Empty<S>(),
						BombProp = Array.Empty<IEnumerable<C<S>[][]>>()
					}
				},
				default,
				new S()
			};
			var self = new X<X<int, float, char>, double, string>();
			self.FirstProp = new X<int, float, char>();
			var bomb = new HashSet<C<S>[][]>[]
			{
				new HashSet<C<S>[][]>
				{
					new C<S>[][]
					{
						new C<S>[]
						{
							new C<S>
							{
							},
							new C<S>
							{
							},
							null,
							null
						},
						null
					},
					null
				}
			};
			var obj = new ComplexRoot
			{
				EnumProp = E.A,
				StructProp = new S
				{
					EnumerableOfStructSProp = new List<S>
					{
						default,
						new S
						{
							EnumerableOfStructSProp = new List<S>(),
							GenericClassCOfStructSProp = new C<S>()
						}
					},
					GenericClassCOfStructSProp = new C<S>
					{
						ArrayOfStructSProp = structs,
						BombProp = new ISet<C<S>[][]>[]
						{
							new HashSet<C<S>[][]>
							{
								new C<S>[][]
								{
									new C<S>[]
									{
										new C<S>
										{
											ArrayOfStructSProp = new S[]
											{
												new S
												{
													GenericClassCOfStructSProp = new C<S>
													{
														ArrayOfStructSProp = Array.Empty<S>(),
														BombProp = Array.Empty<IEnumerable<C<S>[][]>>()
													}
												}
											},
											BombProp = new IEnumerable<C<S>[][]>[]
											{
												new List<C<S>[][]>
												{
													new C<S>[][]
													{
														new C<S>[]
														{
															new C<S>
															{
															},
															new C<S>
															{
																GenericAbstractClassProp = new Concrete<Concrete<S>>
																{
																	CharProp = 'x',
																	NumberProp = 1,
																	StringProp = "string1",
																	AbstractClassGenericArgProp = new Concrete<S>
																	{
																		AbstractClassGenericArgProp = new S()
																	}
																},
																GenericInterfaceProp = new Concrete<Concrete<Concrete<S>>>
																{
																	CharProp = 'y',
																	NumberProp = 2,
																	StringProp = "string2",
																	InterfaceGenericArgProp = new Concrete<Concrete<S>>
																	{
																		CharProp = 'z',
																		NumberProp = 3,
																		StringProp = "string3",
																		InterfaceGenericArgProp = new Concrete<S>
																		{
																			AbstractClassGenericArgProp = new S()
																		}
																	},
																}
															}
														}
													}
												}
											},
											GenericClassXProp = self,
										}
									}
								}
							},
							null,
							new HashSet<C<S>[][]>
							{
								new C<S>[][]
								{
									null,
									new C<S>[]
									{
										new C<S>
										{
											ArrayOfStructSProp = new S[]
											{
												new S
												{
													GenericClassCOfStructSProp = new C<S>
													{
														ArrayOfStructSProp = Array.Empty<S>(),
														BombProp = Array.Empty<IEnumerable<C<S>[][]>>()
													}
												}
											},
											BombProp = bomb,
											GenericClassXProp = self,
										},
										null,
										null
									}
								}
							}
						}
					},
				},
				ClassAProp = new A
				{
					GenericClassAOfAProp = new A<A>
					{
						GenericClassAOfGenericClassAOfAProp = new A<A<A>>
						{
							GenericClassAOfGenericClassAOfAProp = new A<A<A>>
							{
								ClassAProp = new A
								{
									GenericClassAOfAProp = new A<A>
									{
										GenericClassAOfGenericClassAOfAProp = new A<A<A>>
										{
											GenericClassAOfGenericClassAOfAProp = new A<A<A>>
											{
												ClassAProp = new A
												{
												}
											}
										},
										ClassAProp = new A
										{
										}
									}
								}
							}
						},
						ClassAProp = new A
						{
							GenericClassAOfAProp = new A<A>
							{
								GenericClassAOfGenericClassAOfAProp = new A<A<A>>
								{
									ClassAProp = new A
									{
									}
								},
								ClassAProp = new A
								{
								}
							}
						}
					}
				},
				DictProp = new Dictionary<string, IEnumerable<C<int>[][][][]>[]>
				{
					{
						"key1",
						new IEnumerable<C<int>[][][][]>[]
						{
							Enumerable.Empty<C<int>[][][][]>()
						}
					},
					{
						"key2",
						Array.Empty<IEnumerable<C<int>[][][][]>>()
					},
					{
						"key3",
						new List<C<int>[][][][]>[]
						{
							new List<C<int>[][][][]>
							{
							},
							null,
							null,
							new List<C<int>[][][][]>
							{
								Array.Empty<C<int>[][][]>(),
								null,
								new C<int>[][][][]
								{
									null,
									new C<int>[][][]
									{
										new C<int>[][]
										{
											new C<int>[]
											{
												new C<int>
												{
													BombProp = bomb
												},
												null,
												null,
												new C<int>
												{
													BombProp = bomb,
													ArrayOfStructSProp = structs
												},
												null,
											},
											new C<int>[]
											{
												null
											}
										},
										null,
										null,
										null,
										null
									}
								}
							}
						}
					}
				}
			};
			return obj;
		}

		private static string GetComplexString()
		{
			return "{\"enum\":1,\"struct_S\":{\"enumerable_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":[],\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[[[[{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":{\"StringProp\":\"string1\",\"InterfaceGenericArgProp\":null,\"number\":1,\"char\":\"x\",\"abstract_class_generic_arg\":{\"StringProp\":null,\"InterfaceGenericArgProp\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},\"number\":0,\"char\":\"\\u0000\",\"abstract_class_generic_arg\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}}},\"generic_interface\":{\"StringProp\":\"string2\",\"InterfaceGenericArgProp\":{\"StringProp\":\"string3\",\"InterfaceGenericArgProp\":{\"StringProp\":null,\"InterfaceGenericArgProp\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},\"number\":0,\"char\":\"\\u0000\",\"abstract_class_generic_arg\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}},\"number\":3,\"char\":\"z\",\"abstract_class_generic_arg\":null},\"number\":2,\"char\":\"y\",\"abstract_class_generic_arg\":null}}]]]],\"generic_class_X\":{\"self\":null,\"first\":{\"self\":null,\"first\":0,\"second\":0,\"third\":\"\\u0000\"},\"second\":0,\"third\":null},\"generic_abstract_class\":null,\"generic_interface\":null}]]],null,[[null,[{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null],null],null]],\"generic_class_X\":{\"self\":null,\"first\":{\"self\":null,\"first\":0,\"second\":0,\"third\":\"\\u0000\"},\"second\":0,\"third\":null},\"generic_abstract_class\":null,\"generic_interface\":null},null,null]]]],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},\"class_A\":{\"generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"generic_class_A_of_A\":null}},\"class_A\":null},\"class_A\":{\"generic_class_A_of_A\":null}}}},\"class_A\":null},\"class_A\":{\"generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":{\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"generic_class_A_of_A\":null}},\"class_A\":{\"generic_class_A_of_A\":null}}}}},\"dict\":{\"key1\":[[]],\"key2\":[],\"key3\":[[],null,null,[[],null,[null,[[[{\"array_of_struct_S\":null,\"bomb\":[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null],null],null]],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null,{\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[[[[{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null],null],null]],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null],[null]],null,null,null,null]]]]}}";
		}
		#endregion

		#region complex type
		private class ComplexRoot
		{
			[JsonPropertyInterceptor("enum")]
			[JsonProperty("enum")]
			public E EnumProp { get; set; }

			[JsonPropertyInterceptor("struct_S")]
			[JsonProperty("struct_S")]
			public S StructProp { get; set; }

			[JsonPropertyInterceptor("class_A")]
			[JsonProperty("class_A")]
			public A ClassAProp { get; set; }

			[JsonPropertyInterceptor("dict")]
			[JsonProperty("dict")]
			public IDictionary<string, IEnumerable<C<int>[][][][]>[]> DictProp { get; set; }
		}

		private struct S
		{
			[JsonPropertyInterceptor("enumerable_of_struct_S")]
			[JsonProperty("enumerable_of_struct_S")]
			public IEnumerable<S> EnumerableOfStructSProp { get; set; }

			[JsonPropertyInterceptor("generic_class_C_of_struct_S")]
			[JsonProperty("generic_class_C_of_struct_S")]
			public C<S> GenericClassCOfStructSProp { get; set; }
		}

		private class C<T>
		{
			[JsonPropertyInterceptor("array_of_struct_S")]
			[JsonProperty("array_of_struct_S")]
			public S[] ArrayOfStructSProp { get; set; }

			[JsonPropertyInterceptor("bomb")]
			[JsonProperty("bomb")]
			public IEnumerable<C<S>[][]>[] BombProp { get; set; }

			[JsonPropertyInterceptor("generic_class_X")]
			[JsonProperty("generic_class_X")]
			public X<X<int, float, char>, double, string> GenericClassXProp { get; set; }

			[JsonPropertyInterceptor("generic_abstract_class")]
			[JsonProperty("generic_abstract_class")]
			public Concrete<Concrete<T>> GenericAbstractClassProp { get; set; }

			[JsonPropertyInterceptor("generic_interface")]
			[JsonProperty("generic_interface")]
			public Concrete<Concrete<Concrete<T>>> GenericInterfaceProp { get; set; }
		}

		private class A
		{
			[JsonPropertyInterceptor("generic_class_A_of_A")]
			[JsonProperty("generic_class_A_of_A")]
			public A<A> GenericClassAOfAProp { get; set; }
		}

		private class A<T>
		{
			[JsonPropertyInterceptor("generic_class_A_of_generic_class_A_of_A")]
			[JsonProperty("generic_class_A_of_generic_class_A_of_A")]
			public A<A<A>> GenericClassAOfGenericClassAOfAProp { get; set; }

			[JsonPropertyInterceptor("class_A")]
			[JsonProperty("class_A")]
			public A ClassAProp { get; set; }
		}

		private class X<T1, T2, T3>
		{
			[JsonPropertyInterceptor("self")]
			[JsonProperty("self")]
			public X<T1, T2, T3> SelfProp { get; set; }

			[JsonPropertyInterceptor("first")]
			[JsonProperty("first")]
			public T1 FirstProp { get; set; }

			[JsonPropertyInterceptor("second")]
			[JsonProperty("second")]
			public T2 SecondProp { get; set; }

			[JsonPropertyInterceptor("third")]
			[JsonProperty("third")]
			public T3 ThirdProp { get; set; }
		}

		private enum E : int
		{
			Default = 0,
			A = 1,
			B = 2,
		}

		private class Abstract<T>
		{
			[JsonPropertyInterceptor("number")]
			[JsonProperty("number")]
			public int NumberProp { get; set; }

			[JsonPropertyInterceptor("char")]
			[JsonProperty("char")]
			public char CharProp { get; set; }

			[JsonPropertyInterceptor("abstract_class_generic_arg")]
			[JsonProperty("abstract_class_generic_arg")]
			public T AbstractClassGenericArgProp { get; set; }
		}

		private interface IInterface<T>
		{
			[JsonPropertyInterceptor("interface_generic_arg")]
			[JsonProperty("interface_generic_arg")]
			public T InterfaceGenericArgProp { get; set; }

			[JsonPropertyInterceptor("string")]
			[JsonProperty("string")]
			public string StringProp { get; set; }
		}

		private class Concrete<T> : Abstract<T>, IInterface<T>
		{
			public string StringProp { get; set; }
			public T InterfaceGenericArgProp { get; set; }
		}
		#endregion
	}
}
