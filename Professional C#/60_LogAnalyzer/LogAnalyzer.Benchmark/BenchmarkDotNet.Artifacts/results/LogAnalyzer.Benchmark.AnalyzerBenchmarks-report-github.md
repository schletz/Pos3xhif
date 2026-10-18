``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19042.1415 (20H2/October2020Update)
Intel Core i7 CPU 980 3.33GHz (Nehalem), 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|          Method |      Mean |     Error |    StdDev |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|---------------- |----------:|----------:|----------:|---------:|---------:|---------:|----------:|
|      FindIpLinq | 13.586 ms | 0.2714 ms | 0.3333 ms | 937.5000 | 359.3750 | 109.3750 |  5,232 KB |
|    FindIpStream |  6.995 ms | 0.1236 ms | 0.1518 ms | 734.3750 | 203.1250 |        - |  4,545 KB |
|      FindIpSpan |  4.408 ms | 0.0860 ms | 0.0762 ms | 351.5625 | 140.6250 |        - |  2,175 KB |
| FindIpSpanBlock |  3.178 ms | 0.0132 ms | 0.0117 ms |  85.9375 |  39.0625 |        - |    535 KB |
