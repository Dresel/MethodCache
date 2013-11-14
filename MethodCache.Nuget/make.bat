mkdir input\lib\net40
del /Q input\lib\net40\*.*

msbuild ..\MethodCache.Attributes\MethodCache.Attributes.csproj /p:Configuration=Release;OutputPath=..\MethodCache.Nuget\input\lib\net40
msbuild ..\MethodCache.Fody\MethodCache.Fody.csproj /p:Configuration=Release;OutputPath=..\MethodCache.Nuget\input\lib\net40

mkdir output
..\.nuget\nuget.exe pack /o output .\MethodCache.nuspec