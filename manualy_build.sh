workspace=`pwd`
echo 'workspace: ${workspace}'

echo "start build asp.net core"
docker run --rm \
-v ~/.dotnet:/root/.dotnet \
-v ~/.nuget:/root/.nuget  \
-v ${workspace}:/src \
--workdir /src mcr.microsoft.com/dotnet/core/sdk:3.0 bash -c "dotnet --version && dotnet restore ./OpenBlog.sln && rm -rf ./OpenBlog.Web/obj/Docker/publish && dotnet publish ./OpenBlog.Web/OpenBlog.Web.csproj -c Release -o  ./OpenBlog.Web/obj/Docker/publish"

echo "current dir: `pwd`"
buildreport_path='./OpenBlog.Web/obj/Docker/publish/buildreport'
mkdir -p ${buildreport_path}
env > ${buildreport_path}/env.txt
echo "Image Version: ${imagesNames[number]}:${bulldversion}
    GIT COMMIT: $GIT_COMMIT
    GIT_PREVIOUS_COMMIT:$GIT_PREVIOUS_COMMIT
    GIT_PREVIOUS_SUCCESSFUL_COMMIT:$GIT_PREVIOUS_SUCCESSFUL_COMMIT
    GIT_BRANCH:$GIT_BRANCH
    GIT_LOCAL_BRANCH:$GIT_LOCAL_BRANCH
    GIT_URL:$GIT_URL
    GIT_COMMITTER_NAME:$GIT_COMMITTER_NAME
    GIT_AUTHOR_NAME:$GIT_AUTHOR_NAME
    GIT_COMMITTER_EMAIL:$GIT_COMMITTER_EMAIL
    GIT_AUTHOR_EMAIL:$GIT_AUTHOR_EMAIL
    " > ${buildreport_path}/buildversion.txt
