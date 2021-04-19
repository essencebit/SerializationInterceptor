using SerializationInterceptor.Benchmark.Attributes;
using SerializationInterceptor.Benchmark.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializationInterceptor.Benchmark
{
    internal class SystemTextJsonBenchmark
    {
        private const int Padding = 30;
        private const int Iterations = 1000;

        public static void DoBenchmark()
        {
            Console.WriteLine(new string('-', 42));
            Console.WriteLine(new string('-', 8) + "<<<<< SystemTextJson >>>>>" + new string('-', 8));
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
            watch.Start();
            for (int i = 0; i < Iterations; i++) JsonSerializer.Serialize(simpleObj, simpleObj.GetType(), new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            watch.Stop();
            simpleBefore = watch.Elapsed.TotalMilliseconds;
            watch.Restart();
            for (int i = 0; i < Iterations; i++) SystemTextJsonSerializationUtils.InterceptSerialization(simpleObj);
            watch.Stop();
            simpleAfter = watch.Elapsed.TotalMilliseconds;
        }

        private static void DoComplexSerializationBenchmark(out double complexBefore, out double complexAfter)
        {
            var watch = new Stopwatch();
            var complexObj = GetComplexObj();
            watch.Restart();
            for (int i = 0; i < Iterations; i++) JsonSerializer.Serialize(complexObj, complexObj.GetType(), new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            watch.Stop();
            complexBefore = watch.Elapsed.TotalMilliseconds;
            watch.Restart();
            for (int i = 0; i < Iterations; i++) SystemTextJsonSerializationUtils.InterceptSerialization(complexObj);
            watch.Stop();
            complexAfter = watch.Elapsed.TotalMilliseconds;
        }

        private static void DoSimpleDeserializationBenchmark(out double simpleBefore, out double simpleAfter)
        {
            var watch = new Stopwatch();
            var simpleString = GetSimpleString();
            watch.Start();
            for (int i = 0; i < Iterations; i++) JsonSerializer.Deserialize(simpleString, typeof(SimpleRoot), new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            watch.Stop();
            simpleBefore = watch.Elapsed.TotalMilliseconds;
            watch.Restart();
            for (int i = 0; i < Iterations; i++) SystemTextJsonSerializationUtils.InterceptDeserialization<SimpleRoot>(simpleString);
            watch.Stop();
            simpleAfter = watch.Elapsed.TotalMilliseconds;
        }

        private static void DoComplexDeserializationBenchmark(out double complexBefore, out double complexAfter)
        {
            var watch = new Stopwatch();
            var complexString = GetComplexString();
            watch.Restart();
            for (int i = 0; i < Iterations; i++) JsonSerializer.Deserialize(complexString, typeof(ComplexRoot), new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            watch.Stop();
            complexBefore = watch.Elapsed.TotalMilliseconds;
            watch.Restart();
            for (int i = 0; i < Iterations; i++) SystemTextJsonSerializationUtils.InterceptDeserialization<ComplexRoot>(complexString);
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
        [JsonPropertyInterceptor("")]
        public class SimpleRoot
        {
            [JsonPropertyName("data")]
            public Data Data { get; set; }
        }

        public class Data
        {
            [JsonPropertyName("channels")]
            public IEnumerable<Channel> Channels { get; set; }
        }

        public class Channel
        {
            [JsonPropertyName("channelName")]
            public string ChannelName { get; set; }

            [JsonPropertyName("channelType")]
            public string ChannelType { get; set; }

            [JsonPropertyName("unit")]
            public string Unit { get; set; }

            [JsonPropertyName("values")]
            public Values Values { get; set; }
        }

        public class Values
        {
            [JsonPropertyName("value")]
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
            self.SelfProp = self;
            self.FirstProp = new X<int, float, char>();
            self.FirstProp.SelfProp = self.FirstProp;
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
                                                    ArrayOfStructSProp =structs
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
            return "{\"$id\":\"1\",\"enum\":1,\"struct_S\":{\"enumerable_of_struct_S\":{\"$id\":\"2\",\"$values\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":{\"$id\":\"3\",\"$values\":[]},\"generic_class_C_of_struct_S\":{\"$id\":\"4\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}]},\"generic_class_C_of_struct_S\":{\"$id\":\"5\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$id\":\"6\",\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[{\"$id\":\"7\",\"$values\":[[[{\"$id\":\"8\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$id\":\"9\",\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[{\"$id\":\"10\",\"$values\":[[[{\"$id\":\"11\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"$id\":\"12\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":{\"$id\":\"13\",\"StringProp\":\"string1\",\"InterfaceGenericArgProp\":null,\"number\":1,\"char\":\"x\",\"abstract_class_generic_arg\":{\"$id\":\"14\",\"StringProp\":null,\"InterfaceGenericArgProp\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},\"number\":0,\"char\":\"\\u0000\",\"abstract_class_generic_arg\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}}},\"generic_interface\":{\"$id\":\"15\",\"StringProp\":\"string2\",\"InterfaceGenericArgProp\":{\"$id\":\"16\",\"StringProp\":\"string3\",\"InterfaceGenericArgProp\":{\"$id\":\"17\",\"StringProp\":null,\"InterfaceGenericArgProp\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},\"number\":0,\"char\":\"\\u0000\",\"abstract_class_generic_arg\":{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}},\"number\":3,\"char\":\"z\",\"abstract_class_generic_arg\":null},\"number\":2,\"char\":\"y\",\"abstract_class_generic_arg\":null}}]]]}],\"generic_class_X\":{\"$id\":\"18\",\"self\":{\"$ref\":\"18\"},\"first\":{\"$id\":\"19\",\"self\":{\"$ref\":\"19\"},\"first\":0,\"second\":0,\"third\":\"\\u0000\"},\"second\":0,\"third\":null},\"generic_abstract_class\":null,\"generic_interface\":null}]]]},null,{\"$id\":\"20\",\"$values\":[[null,[{\"$id\":\"21\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$id\":\"22\",\"array_of_struct_S\":[],\"bomb\":[],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}}],\"bomb\":[{\"$id\":\"23\",\"$values\":[[[{\"$id\":\"24\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},{\"$id\":\"25\",\"array_of_struct_S\":null,\"bomb\":null,\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null],null],null]}],\"generic_class_X\":{\"$ref\":\"18\"},\"generic_abstract_class\":null,\"generic_interface\":null},null,null]]]}],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null}},\"class_A\":{\"$id\":\"26\",\"generic_class_A_of_A\":{\"$id\":\"27\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"28\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"29\",\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"$id\":\"30\",\"generic_class_A_of_A\":{\"$id\":\"31\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"32\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"33\",\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"$id\":\"34\",\"generic_class_A_of_A\":null}},\"class_A\":null},\"class_A\":{\"$id\":\"35\",\"generic_class_A_of_A\":null}}}},\"class_A\":null},\"class_A\":{\"$id\":\"36\",\"generic_class_A_of_A\":{\"$id\":\"37\",\"generic_class_A_of_generic_class_A_of_A\":{\"$id\":\"38\",\"generic_class_A_of_generic_class_A_of_A\":null,\"class_A\":{\"$id\":\"39\",\"generic_class_A_of_A\":null}},\"class_A\":{\"$id\":\"40\",\"generic_class_A_of_A\":null}}}}},\"dict\":{\"$id\":\"41\",\"key1\":[{\"$id\":\"42\",\"$values\":[]}],\"key2\":[],\"key3\":[{\"$id\":\"43\",\"$values\":[]},null,null,{\"$id\":\"44\",\"$values\":[[],null,[null,[[[{\"$id\":\"45\",\"array_of_struct_S\":null,\"bomb\":[{\"$ref\":\"23\"}],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null,null,{\"$id\":\"46\",\"array_of_struct_S\":[{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":{\"$ref\":\"6\"}},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null},{\"enumerable_of_struct_S\":null,\"generic_class_C_of_struct_S\":null}],\"bomb\":[{\"$ref\":\"23\"}],\"generic_class_X\":null,\"generic_abstract_class\":null,\"generic_interface\":null},null],[null]],null,null,null,null]]]}]}}";
        }
        #endregion

        #region complex type
        private class ComplexRoot
        {
            [JsonPropertyNameInterceptor("enum")]
            [JsonPropertyName("enum")]
            public E EnumProp { get; set; }

            [JsonPropertyNameInterceptor("struct_S")]
            [JsonPropertyName("struct_S")]
            public S StructProp { get; set; }

            [JsonPropertyNameInterceptor("class_A")]
            [JsonPropertyName("class_A")]
            public A ClassAProp { get; set; }

            [JsonPropertyNameInterceptor("dict")]
            [JsonPropertyName("dict")]
            public IDictionary<string, IEnumerable<C<int>[][][][]>[]> DictProp { get; set; }
        }

        private struct S
        {
            [JsonPropertyNameInterceptor("enumerable_of_struct_S")]
            [JsonPropertyName("enumerable_of_struct_S")]
            public IEnumerable<S> EnumerableOfStructSProp { get; set; }

            [JsonPropertyNameInterceptor("generic_class_C_of_struct_S")]
            [JsonPropertyName("generic_class_C_of_struct_S")]
            public C<S> GenericClassCOfStructSProp { get; set; }
        }

        private class C<T>
        {
            [JsonPropertyNameInterceptor("array_of_struct_S")]
            [JsonPropertyName("array_of_struct_S")]
            public S[] ArrayOfStructSProp { get; set; }

            [JsonPropertyNameInterceptor("bomb")]
            [JsonPropertyName("bomb")]
            public IEnumerable<C<S>[][]>[] BombProp { get; set; }

            [JsonPropertyNameInterceptor("generic_class_X")]
            [JsonPropertyName("generic_class_X")]
            public X<X<int, float, char>, double, string> GenericClassXProp { get; set; }

            [JsonPropertyNameInterceptor("generic_abstract_class")]
            [JsonPropertyName("generic_abstract_class")]
            public Concrete<Concrete<T>> GenericAbstractClassProp { get; set; }

            [JsonPropertyNameInterceptor("generic_interface")]
            [JsonPropertyName("generic_interface")]
            public Concrete<Concrete<Concrete<T>>> GenericInterfaceProp { get; set; }
        }

        private class A
        {
            [JsonPropertyNameInterceptor("generic_class_A_of_A")]
            [JsonPropertyName("generic_class_A_of_A")]
            public A<A> GenericClassAOfAProp { get; set; }
        }

        private class A<T>
        {
            [JsonPropertyNameInterceptor("generic_class_A_of_generic_class_A_of_A")]
            [JsonPropertyName("generic_class_A_of_generic_class_A_of_A")]
            public A<A<A>> GenericClassAOfGenericClassAOfAProp { get; set; }

            [JsonPropertyNameInterceptor("class_A")]
            [JsonPropertyName("class_A")]
            public A ClassAProp { get; set; }
        }

        private class X<T1, T2, T3>
        {
            [JsonPropertyNameInterceptor("self")]
            [JsonPropertyName("self")]
            public X<T1, T2, T3> SelfProp { get; set; }

            [JsonPropertyNameInterceptor("first")]
            [JsonPropertyName("first")]
            public T1 FirstProp { get; set; }

            [JsonPropertyNameInterceptor("second")]
            [JsonPropertyName("second")]
            public T2 SecondProp { get; set; }

            [JsonPropertyNameInterceptor("third")]
            [JsonPropertyName("third")]
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
            [JsonPropertyNameInterceptor("number")]
            [JsonPropertyName("number")]
            public int NumberProp { get; set; }

            [JsonPropertyNameInterceptor("char")]
            [JsonPropertyName("char")]
            public char CharProp { get; set; }

            [JsonPropertyNameInterceptor("abstract_class_generic_arg")]
            [JsonPropertyName("abstract_class_generic_arg")]
            public T AbstractClassGenericArgProp { get; set; }
        }

        private interface IInterface<T>
        {
            [JsonPropertyNameInterceptor("interface_generic_arg")]
            [JsonPropertyName("interface_generic_arg")]
            public T InterfaceGenericArgProp { get; set; }

            [JsonPropertyNameInterceptor("string")]
            [JsonPropertyName("string")]
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
