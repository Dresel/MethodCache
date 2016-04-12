mkdir input\lib\
del /Q input\lib\*.*

msbuild ..\MethodCache.Attributes\MethodCache.Attributes.csproj /p:Configuration=Release;OutputPath=..\MethodCache.Nuget\input\lib
msbuild ..\MethodCache.Fody\MethodCache.Fody.csproj /p:Configuration=Release;OutputPath=..\MethodCache.Nuget\input\lib

mkdir output
..\.nuget\nuget.exe pack /o output .\MethodCache.nuspec