
dir=$(dirname "$0")
cd  dir
dotnet publish lok.csproj -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
dotnet publish lok.csproj -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
dotnet publish lok.csproj -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

if [[ "$OSTYPE" == "linux-gnu"* ]]; then
  dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
elif [[ "$OSTYPE" == "darwin"* ]]; then
  dotnet publish -c Release -r osx-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
elif [[ "$OSTYPE" == "cygwin" || "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
  dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
else

fi


         